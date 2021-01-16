using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jab
{
    internal readonly struct GeneratorContext
    {
        private readonly GeneratorExecutionContext? _generatorExecutionContext;
        private readonly CompilationAnalysisContext? _compilationAnalysisContext;
        private readonly SyntaxCollector _syntaxCollector;

        public GeneratorContext(CompilationAnalysisContext compilationAnalysisContext, SyntaxCollector syntaxCollector)
        {
            _compilationAnalysisContext = compilationAnalysisContext;
            _syntaxCollector = syntaxCollector;
            _generatorExecutionContext = null;
        }

        public GeneratorContext(GeneratorExecutionContext generatorExecutionContext)
        {
            _syntaxCollector = (SyntaxCollector) generatorExecutionContext.SyntaxReceiver!;
            _compilationAnalysisContext = null;
            _generatorExecutionContext = generatorExecutionContext;
        }

        public Compilation Compilation => _generatorExecutionContext?.Compilation ?? _compilationAnalysisContext?.Compilation ?? throw new InvalidOperationException();
        public IEnumerable<InvocationExpressionSyntax> CandidateGetServiceCalls => _syntaxCollector.InvocationExpressions;
        public IEnumerable<TypeDeclarationSyntax> CandidateTypes => _syntaxCollector.CandidateTypes;

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