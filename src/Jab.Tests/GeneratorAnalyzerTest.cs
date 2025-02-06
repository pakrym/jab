using System.IO;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace JabTests
{
    public class GeneratorAnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public GeneratorAnalyzerTest()
        {
            TestState.Sources.Add(File.ReadAllText("Attributes.cs"));
        }
    }
}