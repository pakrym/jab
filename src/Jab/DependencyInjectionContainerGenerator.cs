using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Jab
{
    [Generator]
    public class DependencyInjectionContainerGenerator : ISourceGenerator
    {
        private static readonly DiagnosticDescriptor UnexpectedErrorDescriptor = new DiagnosticDescriptor("JAB0001",
            "Unexpected error during generation",
            "Unexpected error occurred during code generation: {0}", "Usage", DiagnosticSeverity.Error, true);

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        private void GenerateCallSiteWithCache(CodeWriter codeWriter, ServiceCallSite serviceCallSite, Action<CodeWriter, CodeWriterDelegate> valueCallback)
        {
            if (serviceCallSite.Singleton)
            {
                var cacheLocation = GetCacheLocation(serviceCallSite);
                using (codeWriter.Scope($"if ({cacheLocation} == default)"))
                using (codeWriter.Scope($"lock (this)"))
                using (codeWriter.Scope($"if ({cacheLocation} == default)"))
                {
                    GenerateCallSite(
                        codeWriter,
                        serviceCallSite,
                        (w, v) =>
                        {
                            w.Line($"{cacheLocation} = {v};");
                        });
                }

                valueCallback(codeWriter, w => w.Append($"{cacheLocation}"));
            }
            else
            {
                GenerateCallSite(codeWriter, serviceCallSite, valueCallback);
            }
        }

        private string GetCacheLocation(ServiceCallSite serviceCallSite)
        {
            return "_" + serviceCallSite.ServiceType.Name;
        }

        private void GenerateCallSite(CodeWriter codeWriter, ServiceCallSite serviceCallSite, Action<CodeWriter, CodeWriterDelegate> valueCallback)
        {
            switch (serviceCallSite)
            {
                case ConstructorCallSite transientCallSite:
                    valueCallback(codeWriter, w =>
                    {
                        w.Append($"new {transientCallSite.ImplementationType}(");
                        foreach (var parameter in transientCallSite.Parameters)
                        {
                            w.Append($"{GetResolutionServiceName(parameter.ServiceType)}(), ");
                        }
                        w.RemoveTrailingComma();
                        w.Append($")");
                    });
                    break;
                case MemberCallSite memberCallSite:
                    // TODO: support methods
                    valueCallback(codeWriter, w =>
                    {
                        w.AppendRaw(memberCallSite.Member.Name);
                    });
                    break;
            }
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var roots = new CompositionRootBuilder(context).BuildRoots();

                foreach (var root in roots)
                {
                    var codeWriter = new CodeWriter();
                    using (codeWriter.Namespace($"{root.Type.ContainingNamespace.ToDisplayString()}"))
                    {
                        // TODO: implement infinite nesting
                        using CodeWriter.CodeWriterScope? parentTypeScope = root.Type.ContainingType is {} containingType ?
                            codeWriter.Scope($"{SyntaxFacts.GetText(containingType.DeclaredAccessibility)} partial class {containingType.Name}") :
                            null;

                        using (codeWriter.Scope($"{SyntaxFacts.GetText(root.Type.DeclaredAccessibility)} partial class {root.Type.Name}"))
                        {
                            foreach (var rootService in root.RootCallSites)
                            {
                                if (rootService.Singleton)
                                {
                                    codeWriter.Line($"private {rootService.ServiceType} {GetCacheLocation(rootService)};");
                                }
                            }

                            foreach (var rootService in root.RootCallSites)
                            {
                                var rootServiceType = rootService.ServiceType;
                                using (codeWriter.Scope($"{SyntaxFacts.GetText(rootServiceType.DeclaredAccessibility)} {rootServiceType} {GetResolutionServiceName(rootServiceType)}()"))
                                {
                                    GenerateCallSiteWithCache(codeWriter,
                                        rootService,
                                        (w, v) => w.Line($"return {v};"));
                                }

                                codeWriter.Line();
                            }

                        }
                    }
                    context.AddSource($"{root.Type.Name}.Generated.cs", codeWriter.ToString());
                }
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(UnexpectedErrorDescriptor, Location.None, e.ToString()));
            }
        }

        private string GetResolutionServiceName(INamedTypeSymbol rootServiceType) => $"Get{rootServiceType.Name}";
    }
}