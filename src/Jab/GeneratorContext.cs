using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jab
{
    internal readonly struct GeneratorContext
    {
        private readonly GeneratorExecutionContext? _generatorExecutionContext;
        private readonly CompilationAnalysisContext? _compilationAnalysisContext;

        public GeneratorContext(CompilationAnalysisContext compilationAnalysisContext)
        {
            _compilationAnalysisContext = compilationAnalysisContext;
            _generatorExecutionContext = null;
        }

        public GeneratorContext(GeneratorExecutionContext generatorExecutionContext)
        {
            _compilationAnalysisContext = null;
            _generatorExecutionContext = generatorExecutionContext;
        }

        public Compilation Compilation => _generatorExecutionContext?.Compilation ?? _compilationAnalysisContext?.Compilation ?? throw new InvalidOperationException();

        public void ReportDiagnostic(Diagnostic diagnostic)
        {
            _generatorExecutionContext?.ReportDiagnostic(diagnostic);
            _compilationAnalysisContext?.ReportDiagnostic(diagnostic);
        }

        public void AddSource(string nameHint, string code)
        {
            _generatorExecutionContext?.AddSource(nameHint, code);
        }
    }
}