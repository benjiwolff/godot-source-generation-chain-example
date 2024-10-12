using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Godot.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using GeneratorExecutionContext = Microsoft.CodeAnalysis.GeneratorExecutionContext;

namespace zombie_shooter.SourceGeneration;

public class GeneratorExecutionContextWithNewCompilation(
    GeneratorExecutionContext context,
    Compilation compilation,
    IReadOnlyCollection<SyntaxTree> newSyntaxTrees)
    : IGeneratorExecutionContext
{
    public void AddSource(string hintName, string source)
    {
        context.AddSource(hintName, source);
    }

    public void AddSource(string hintName, SourceText sourceText)
    {
        context.AddSource(hintName, sourceText);
    }

    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        // fixes things like this
        // CSC: Warning CS8785 : Generator 'GeneratorCoordinator' failed to generate source. It will not contribute to the output and compilation errors may occur as a result. Exception was of type 'AggregateException' with message 'One or more errors occurred. (Reported diagnostic 'GD0102' has a source location in file 'ArrayPlacer.g', which is not part of the compilation being analyzed. (Parameter 'diagnostic'))'.
        if (newSyntaxTrees.Contains(diagnostic.Location.SourceTree))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic);
    }

    public Compilation Compilation => compilation;

    public ParseOptions ParseOptions => context.ParseOptions;

    public ImmutableArray<AdditionalText> AdditionalFiles => context.AdditionalFiles;

    public AnalyzerConfigOptionsProvider AnalyzerConfigOptions => context.AnalyzerConfigOptions;

    public ISyntaxReceiver SyntaxReceiver => context.SyntaxReceiver;

    public ISyntaxContextReceiver SyntaxContextReceiver => context.SyntaxContextReceiver;

    public CancellationToken CancellationToken => context.CancellationToken;
}