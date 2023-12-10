namespace Jab
{
    /// <summary>
    /// In order to use inside Unity follow:
    /// https://docs.unity3d.com/Manual/roslyn-analyzers.html
    /// Also requires the Jab.Attributes.dll
    /// </summary>
#pragma warning disable RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    public partial class ContainerGenerator: ISourceGenerator
#pragma warning restore RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxCollector());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var moduleName = context.Compilation.SourceModule.Name;
            if (moduleName.StartsWith("UnityEngine.")) return;
            if (moduleName.StartsWith("UnityEditor.")) return;
            if (moduleName.StartsWith("Unity.")) return;

            Execute(new GeneratorContext(context));
        }
    }
}