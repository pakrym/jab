#if ROSLYN4_0_OR_GREATER
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jab
{
    public partial class ContainerGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<TypeDeclarationSyntax> providerTypes = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => SyntaxCollector.IsKnownAttribute(node),
                (syntaxContext, _) => SyntaxCollector.GetCandidateType(syntaxContext.Node));

            IncrementalValuesProvider<InvocationExpressionSyntax> getServiceCalls = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => SyntaxCollector.IsGetServiceExpression(node),
                (syntaxContext, _) => (InvocationExpressionSyntax)syntaxContext.Node);

            IncrementalValueProvider<((ImmutableArray<TypeDeclarationSyntax>, ImmutableArray<InvocationExpressionSyntax>), Compilation)> allInputs =
                providerTypes.Collect().Combine(getServiceCalls.Collect()).Combine(context.CompilationProvider);

            context.RegisterSourceOutput(allInputs, (productionContext, inputs) =>
                Execute(new GeneratorContext(productionContext, inputs.Item1.Item1, inputs.Item1.Item2, inputs.Item2)));

            context.RegisterPostInitializationOutput(c =>
            {
                c.AddSource("Attributes.cs", ReadAttributesFile());
            });
        }
    }
}

#endif