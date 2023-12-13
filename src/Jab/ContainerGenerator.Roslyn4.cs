#if ROSLYN4_0_OR_GREATER
namespace Jab
{
#pragma warning disable RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    public partial class ContainerGenerator : IIncrementalGenerator
#pragma warning restore RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<TypeDeclarationSyntax> providerTypes = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => SyntaxCollector.IsCandidateType(node),
                (syntaxContext, _) => (TypeDeclarationSyntax)syntaxContext.Node);

            IncrementalValuesProvider<InvocationExpressionSyntax> getServiceCalls = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => SyntaxCollector.IsGetServiceExpression(node),
                (syntaxContext, _) => (InvocationExpressionSyntax)syntaxContext.Node);

            var collectedServiceCalls = getServiceCalls.Collect();

            var providers = providerTypes.Combine(collectedServiceCalls).Combine(context.CompilationProvider);

            context.RegisterSourceOutput(providers, (productionContext, inputs) =>
                Execute(new GeneratorContext(
                    productionContext,
                    ImmutableArray.Create(inputs.Item1.Item1),
                    inputs.Item1.Item2,
                    inputs.Item2)));

            context.RegisterPostInitializationOutput(c =>
            {
                c.AddSource("Attributes.cs", ReadAttributesFile());
            });
        }
    }
}

#endif