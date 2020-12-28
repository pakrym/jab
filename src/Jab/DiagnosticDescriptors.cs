using Microsoft.CodeAnalysis;

namespace Jab
{
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor UnexpectedErrorDescriptor = new DiagnosticDescriptor("JAB0001",
            "Unexpected error during generation",
            "Unexpected error occurred during code generation: {0}", "Usage", DiagnosticSeverity.Error, true);
    }
}