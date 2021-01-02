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

        private void GenerateCallSiteWithCache(CodeWriter codeWriter, ServiceCallSite serviceCallSite, Action<CodeWriter, CodeWriterDelegate> valueCallback)
        {
            if (serviceCallSite.Singleton)
            {
                var cacheLocation = GetCacheLocation(serviceCallSite);
                codeWriter.Line($"if ({cacheLocation} == default)");
                codeWriter.Line($"lock (this)");
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

        private void GenerateCallSite(CodeWriter codeWriter, ServiceCallSite serviceCallSite, Action<CodeWriter, CodeWriterDelegate> valueCallback)
        {
            void AppendResolutionCall(ServiceCallSite other)
            {
                if (other.IsMainImplementation)
                {
                    codeWriter.Append($"this.GetService<{other.ServiceType}>()");
                }
                else
                {
                    codeWriter.Append($"{GetResolutionServiceName(other)}()");
                }
            }

            switch (serviceCallSite)
            {
                case ConstructorCallSite transientCallSite:
                    valueCallback(codeWriter, w =>
                    {
                        w.Append($"new {transientCallSite.ImplementationType}(");
                        foreach (var parameter in transientCallSite.Parameters)
                        {
                            AppendResolutionCall(parameter);
                            w.AppendRaw(", ");
                        }
                        w.RemoveTrailingComma();
                        w.Append($")");
                    });
                    break;
                case MemberCallSite memberCallSite:
                    valueCallback(codeWriter, w =>
                    {
                        w.AppendRaw(memberCallSite.Member.Name);
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
                                AppendResolutionCall(item);
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
                    using (codeWriter.Namespace($"{root.Type.ContainingNamespace.ToDisplayString()}"))
                    {
                        // TODO: implement infinite nesting
                        using CodeWriter.CodeWriterScope? parentTypeScope = root.Type.ContainingType is {} containingType ?
                            codeWriter.Scope($"{SyntaxFacts.GetText(containingType.DeclaredAccessibility)} partial class {containingType.Name}") :
                            null;
                        codeWriter.Append($"{SyntaxFacts.GetText(root.Type.DeclaredAccessibility)} partial class {root.Type.Name} : ");
                        foreach (var serviceCallSite in root.RootCallSites)
                        {
                            if (serviceCallSite.IsMainImplementation)
                            {
                                codeWriter.Line();
                                codeWriter.Append($"    IServiceProvider<{serviceCallSite.ServiceType}>,");
                            }
                        }

                        codeWriter.RemoveTrailingComma();
                        codeWriter.Line();
                        using (codeWriter.Scope())
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
                                if (rootService.IsMainImplementation)
                                {
                                    using (codeWriter.Scope($"{rootServiceType} IServiceProvider<{rootServiceType}>.GetService()"))
                                    {
                                        GenerateCallSiteWithCache(codeWriter,
                                            rootService,
                                            (w, v) => w.Line($"return {v};"));
                                    }
                                }
                                else
                                {
                                    using (codeWriter.Scope($"private {rootServiceType} {GetResolutionServiceName(rootService)}()"))
                                    {
                                        GenerateCallSiteWithCache(codeWriter,
                                            rootService,
                                            (w, v) => w.Line($"return {v};"));
                                    }

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
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.UnexpectedErrorDescriptor, Location.None, e.ToString()));
            }
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