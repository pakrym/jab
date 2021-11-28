#if !ROSLYN4_0_OR_GREATER
namespace Jab
{
    public partial class ContainerGenerator: ISourceGenerator
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