#if !ROSLYN4_0_OR_GREATER
namespace Jab
{
#pragma warning disable RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    public partial class ContainerGenerator : ISourceGenerator
#pragma warning restore RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxCollector());
            context.RegisterForPostInitialization(c =>
            {
                c.AddSource("Attributes.cs", ReadAttributesFile());
            });
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Execute(new GeneratorContext(context));
        }
    }
}

#endif