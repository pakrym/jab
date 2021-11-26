#if !ROSLYN4_0_OR_GREATER

using Microsoft.CodeAnalysis;

namespace Jab
{
    public partial class ContainerGenerator: ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxCollector());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Execute(new GeneratorContext(context));
        }
    }
}

#endif