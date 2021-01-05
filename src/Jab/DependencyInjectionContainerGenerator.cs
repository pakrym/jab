using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jab
{
    [Generator]
    #pragma warning disable RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    public class DependencyInjectionContainerGenerator : DiagnosticAnalyzer, ISourceGenerator
    #pragma warning restore RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new GetServiceSyntaxCollector());
        }

        private void GenerateCallSiteWithCache(CodeWriter codeWriter, string rootReference, ServiceCallSite serviceCallSite, Action<CodeWriter, CodeWriterDelegate> valueCallback)
        {
            if (serviceCallSite.Lifetime != ServiceLifetime.Transient)
            {
                var cacheLocation = GetCacheLocation(serviceCallSite);
                codeWriter.Line($"if ({cacheLocation} == default)");
                codeWriter.Line($"lock (this)");
                using (codeWriter.Scope($"if ({cacheLocation} == default)"))
                {
                    GenerateCallSite(
                        codeWriter,
                        rootReference,
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
                GenerateCallSite(codeWriter, rootReference, serviceCallSite, valueCallback);
            }
        }

        private void WriteResolutionCall(CodeWriter codeWriter, ServiceCallSite other, string reference)
        {
            if (other.IsMainImplementation)
            {
                codeWriter.Append($"{reference}.GetService<{other.ServiceType}>()");
            }
            else
            {
                codeWriter.Append($"{reference}.{GetResolutionServiceName(other)}()");
            }
        }


        private void GenerateCallSite(CodeWriter codeWriter, string rootReference, ServiceCallSite serviceCallSite, Action<CodeWriter, CodeWriterDelegate> valueCallback)
        {
            switch (serviceCallSite)
            {
                case ConstructorCallSite transientCallSite:
                    valueCallback(codeWriter, w =>
                    {
                        w.Append($"new {transientCallSite.ImplementationType}(");
                        foreach (var parameter in transientCallSite.Parameters)
                        {
                            WriteResolutionCall(codeWriter, parameter, "this");
                            w.AppendRaw(", ");
                        }
                        w.RemoveTrailingComma();
                        w.Append($")");
                    });
                    break;
                case MemberCallSite memberCallSite:
                    valueCallback(codeWriter, w =>
                    {
                        w.Append($"{rootReference}.{memberCallSite.Member.Name}");
                        if (memberCallSite.Member is IMethodSymbol)
                        {
                            w.AppendRaw("()");
                        }
                    });
                    break;
                case ArrayServiceCallSite arrayServiceCallSite:
                    valueCallback(codeWriter, w =>
                    {
                        using (w.Scope($"new {arrayServiceCallSite.ItemType}[]", newLine: false))
                        {
                            foreach (var item in arrayServiceCallSite.Items)
                            {
                                WriteResolutionCall(codeWriter, item, "this");
                                w.LineRaw(", ");
                            }
                        }
                    });
                    break;
            }
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Execute(new GeneratorContext(context));
        }

        private void Execute(GeneratorContext context)
        {
            try
            {
                var roots = new ServiceProviderBuilder(context).BuildRoots();

                foreach (var root in roots)
                {
                    var codeWriter = new CodeWriter();
                    codeWriter.UseNamespace("Jab");
                    using (codeWriter.Namespace($"{root.Type.ContainingNamespace.ToDisplayString()}"))
                    {
                        // TODO: implement infinite nesting
                        using CodeWriter.CodeWriterScope? parentTypeScope = root.Type.ContainingType is {} containingType ?
                            codeWriter.Scope($"{SyntaxFacts.GetText(containingType.DeclaredAccessibility)} partial class {containingType.Name}") :
                            null;
                        codeWriter.Append($"{SyntaxFacts.GetText(root.Type.DeclaredAccessibility)} partial class {root.Type.Name}");
                        WriteInterfaces(codeWriter, root);
                        using (codeWriter.Scope())
                        {
                            WriteCacheLocations(root, codeWriter, onlyScoped: false);

                            foreach (var rootService in root.RootCallSites)
                            {
                                var rootServiceType = rootService.ServiceType;
                                using (rootService.IsMainImplementation ?
                                    codeWriter.Scope($"{rootServiceType} IServiceProvider<{rootServiceType}>.GetService()") :
                                    codeWriter.Scope($"private {rootServiceType} {GetResolutionServiceName(rootService)}()"))
                                {
                                    GenerateCallSiteWithCache(codeWriter,
                                        "this",
                                        rootService,
                                        (w, v) => w.Line($"return {v};"));
                                }
                                codeWriter.Line();
                            }

                            using (codeWriter.Scope($"public Scope CreateScope()"))
                            {
                                codeWriter.Line($"return new Scope(this);");
                            }
                            codeWriter.Line();

                            codeWriter.Append($"public partial class Scope");
                            WriteInterfaces(codeWriter, root);
                            using (codeWriter.Scope())
                            {
                                WriteCacheLocations(root, codeWriter, onlyScoped: true);
                                codeWriter.Line($"private {root.Type} _root;");
                                codeWriter.Line();

                                using (codeWriter.Scope($"public Scope({root.Type} root)"))
                                {
                                    codeWriter.Line($"_root = root;");
                                }
                                codeWriter.Line();

                                foreach (var rootService in root.RootCallSites)
                                {
                                    var rootServiceType = rootService.ServiceType;

                                    using (rootService.IsMainImplementation ?
                                        codeWriter.Scope($"{rootServiceType} IServiceProvider<{rootServiceType}>.GetService()") :
                                        codeWriter.Scope($"private {rootServiceType} {GetResolutionServiceName(rootService)}()"))
                                    {
                                        if (rootService.Lifetime == ServiceLifetime.Singleton)
                                        {
                                            codeWriter.Append($"return ");
                                            WriteResolutionCall(codeWriter, rootService, "_root");
                                            codeWriter.Line($";");
                                        }
                                        else
                                        {
                                            GenerateCallSiteWithCache(codeWriter,
                                                "_root",
                                                rootService,
                                                (w, v) => w.Line($"return {v};"));
                                        }
                                    }
                                    codeWriter.Line();
                                }
                            }
                        }
                    }
                    context.AddSource($"{root.Type.Name}.Generated.cs", codeWriter.ToString());
                }
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.UnexpectedErrorDescriptor, Location.None, e.ToString()));
            }
        }

        private static void WriteInterfaces(CodeWriter codeWriter, ServiceProvider root)
        {
            bool first = true;
            foreach (var serviceCallSite in root.RootCallSites)
            {
                if (serviceCallSite.IsMainImplementation)
                {
                    if (first)
                    {
                        codeWriter.Append($" : ");
                        first = false;
                    }
                    codeWriter.Line();
                    codeWriter.Append($"    IServiceProvider<{serviceCallSite.ServiceType}>,");
                }
            }

            codeWriter.RemoveTrailingComma();
            codeWriter.Line();
        }

        private void WriteCacheLocations(ServiceProvider root, CodeWriter codeWriter, bool onlyScoped)
        {
            foreach (var rootService in root.RootCallSites)
            {
                switch (rootService.Lifetime)
                {
                    case ServiceLifetime.Singleton when onlyScoped:
                    case ServiceLifetime.Transient:
                        continue;
                }

                codeWriter.Line($"private {rootService.ServiceType} {GetCacheLocation(rootService)};");
            }
            codeWriter.Line();
        }

        private string GetResolutionServiceName(ServiceCallSite serviceCallSite)
        {
            if (!serviceCallSite.IsMainImplementation)
            {
                return $"Get{serviceCallSite.ServiceType.Name}_{serviceCallSite.ReverseIndex}";
            }

            throw new InvalidOperationException("Main implementation should be resolved via GetService<T> call");
        }

        private string GetCacheLocation(ServiceCallSite serviceCallSite)
        {
            if (!serviceCallSite.IsMainImplementation)
            {
                return $"_{serviceCallSite.ServiceType.Name}_{serviceCallSite.ReverseIndex}";
            }

            return $"_{serviceCallSite.ServiceType.Name}";
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(compilationContext =>
            {
                Execute(new GeneratorContext(compilationContext));
            });
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new[] {DiagnosticDescriptors.UnexpectedErrorDescriptor}.ToImmutableArray();
    }
}