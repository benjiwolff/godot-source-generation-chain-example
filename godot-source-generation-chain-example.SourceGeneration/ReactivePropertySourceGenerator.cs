using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot.SourceGenerators.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using GeneratorExecutionContext = Microsoft.CodeAnalysis.GeneratorExecutionContext;

namespace zombie_shooter.SourceGeneration
{
    [Generator]
    public class ReactivePropertySourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        [SuppressMessage(
            "MicrosoftCodeAnalysisCorrectness",
            "RS1035:Do not use APIs banned for analyzers",
            Justification = "My source generator, my rules")]
        public void Execute(GeneratorExecutionContext context)
        {
            var attributeSymbol = typeof(ExportReactiveAttribute).Name;

            var newSyntaxTrees = new List<SyntaxTree>();
            foreach (var syntaxTree in context.Compilation.SyntaxTrees)
            {
                var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
                var fields = syntaxTree.GetRoot()
                    .DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .Where(
                        f => f.AttributeLists.SelectMany(al => al.Attributes)
                            .Any(
                                a => semanticModel.GetTypeInfo(a).Type!.Name ==
                                     attributeSymbol)).ToArray();

                var relatedClass = (ClassDeclarationSyntax)fields.FirstOrDefault()?.Parent;
                if (relatedClass is null)
                {
                    continue;
                }

                var generatedClass = GenerateClass(relatedClass, semanticModel);
                foreach (var field in fields)
                {
                    GenerateProperty(field, ref generatedClass);
                }

                CloseClass(generatedClass);
                var sourceText = SourceText.From(generatedClass.ToString(), Encoding.UTF8);
                context.AddSource(
                    relatedClass.Identifier.ValueText,
                    sourceText);
                newSyntaxTrees.Add(
                    CSharpSyntaxTree.ParseText(
                        sourceText,
                        context.ParseOptions as CSharpParseOptions,
                        path: relatedClass.Identifier.ValueText));
            }

            var newCompilation = context.Compilation.AddSyntaxTrees(newSyntaxTrees);
            var newContext = new GeneratorExecutionContextWithNewCompilation(context, newCompilation, newSyntaxTrees);

            GodotGenerators.RunAll(newContext);
        }

        private void GenerateProperty(FieldDeclarationSyntax fieldDeclaration, ref StringBuilder builder)
        {
            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                var fieldName = variable.GetText().ToString();
                var propertyName = fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
                builder.Append(
                    @"
        [Export]
        private " + fieldDeclaration.Declaration.Type.GetText() + propertyName + @"
        {
            get => " + fieldName + @";
            set
            {
                " + fieldName + @" = value;
                React();
            }
        }");
            }
        }

        private StringBuilder GenerateClass(ClassDeclarationSyntax relatedClass, SemanticModel semanticModel)
        {
            var sb = new StringBuilder();

            sb.Append(
                @"using System;
using Godot;

namespace " + semanticModel.GetDeclaredSymbol(relatedClass)!.ContainingNamespace + @"
{
    public partial class " + relatedClass.Identifier.ValueText + @"
    {");

            return sb;
        }

        private void CloseClass(StringBuilder generatedClass)
        {
            generatedClass.Append(
                @"
    }
}");
        }
    }
}