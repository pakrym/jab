using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Jab.Tests
{
    public class GeneratorAnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier> where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public GeneratorAnalyzerTest()
        {
            TestState.Sources.Add(File.ReadAllText("Attributes.cs"));
        }
    }
}