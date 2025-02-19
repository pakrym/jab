﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;

namespace JabTests
{
    public static class GeneratorAnalyzerVerifier<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] diagnostics)
        {
            source = @"
using System;
using System.Collections.Generic;
using Jab;
" + source;
            var test = new GeneratorAnalyzerTest<TAnalyzer>
            {
                TestCode = source
            };

            test.ExpectedDiagnostics.AddRange(diagnostics);
            return test.RunAsync(CancellationToken.None);
        }

        public static DiagnosticResult Diagnostic(string expectedDescriptor) => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(expectedDescriptor);
    }
}