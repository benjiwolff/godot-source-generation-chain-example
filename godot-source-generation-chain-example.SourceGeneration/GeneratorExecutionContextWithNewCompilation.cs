using System.Collections.Immutable;
using System.Threading;
using Godot.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using GeneratorExecutionContext = Microsoft.CodeAnalysis.GeneratorExecutionContext;

namespace zombie_shooter.SourceGeneration;

public class GeneratorExecutionContextWithNewCompilation(GeneratorExecutionContext context, Compilation compilation)
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