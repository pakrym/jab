using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jab
{
    internal readonly struct GeneratorContext
    {
        private readonly SourceProductionContext? _sourceProductionContext;
        private readonly CompilationAnalysisContext? _compilationAnalysisContext;

        public GeneratorContext(CompilationAnalysisContext compilationAnalysisContext, SyntaxCollector syntaxCollector)
        {
            _compilationAnalysisContext = compilationAnalysisContext;

            CandidateGetServiceCalls = syntaxCollector.InvocationExpressions;
            CandidateTypes = syntaxCollector.CandidateTypes;
            Compilation = compilationAnalysisContext.Compilation;
            _sourceProductionContext = null;
        }

        public Compilation Compilation { get; }

        public GeneratorContext(
            SourceProductionContext sourceProductionContext,
            ImmutableArray<TypeDeclarationSyntax> candidateTypes,
            ImmutableArray<InvocationExpressionSyntax> candidateGetServiceCalls,
            Compilation compilation)
        {
            _sourceProductionContext = sourceProductionContext;
            CandidateTypes = candidateTypes;
            CandidateGetServiceCalls = candidateGetServiceCalls;
            Compilation = compilation;
            _compilationAnalysisContext = null;
        }

        public IEnumerable<InvocationExpressionSyntax> CandidateGetServiceCalls { get; }
        public IEnumerable<TypeDeclarationSyntax> CandidateTypes { get; }

        public void ReportDiagnostic(Diagnostic diagnostic)
        {
            _sourceProductionContext?.ReportDiagnostic(diagnostic);
            _compilationAnalysisContext?.ReportDiagnostic(diagnostic);
        }

        public void AddSource(string nameHint, string code)
        {
            _sourceProductionContext?.AddSource(nameHint, code);
        }
    }
}