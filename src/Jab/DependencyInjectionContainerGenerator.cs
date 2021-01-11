using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
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
            if (serviceCallSite is ErrorCallSite errorCallSite)
            {
                codeWriter.Line($"// There was an error while building the container, please refer to the compiler diagnostics");
                codeWriter.Line($"// {errorCallSite.Diagnostic}");
                codeWriter.Line($"return default;");
                return;
            }

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
            else if (serviceCallSite.IsDisposable != false)
            {
                GenerateCallSite(codeWriter, rootReference, serviceCallSite, (w, v) =>
                {
                    w.Line($"{serviceCallSite.ImplementationType} service = {v};");
                });
                codeWriter.Line($"TryAddDisposable(service);");
                valueCallback(codeWriter, w => w.Append($"service"));
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

                        foreach (var pair in transientCallSite.OptionalParameter)
                        {
                            w.Append($"{pair.Key.Name}: ");
                            WriteResolutionCall(codeWriter, pair.Value, "this");
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

                            WriteServiceProvider(codeWriter, root);

                            WriteDispose(codeWriter, root, onlyScoped: false);

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

                                WriteDispose(codeWriter, root, onlyScoped: true);
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

        private void WriteServiceProvider(CodeWriter codeWriter, ServiceProvider root)
        {
            throw new NotImplementedException();
        }

        private void WriteDispose(CodeWriter codeWriter, ServiceProvider root, bool onlyScoped)
        {
            codeWriter.Line($"private {typeof(List<object>)} _disposables;");
            codeWriter.Line();

            using (codeWriter.Scope($"private void TryAddDisposable(object value)"))
            {
                codeWriter.Line($"if (value is {typeof(IDisposable)} || value is System.IAsyncDisposable)");
                using (codeWriter.Scope($"lock (this)"))
                {
                    codeWriter.Line($"(_disposables ??= new {typeof(List<object>)}()).Add(value);");
                }
            }
            codeWriter.Line();

            using (codeWriter.Scope($"public void Dispose()"))
            {
                codeWriter.LineRaw("void TryDispose(object value) => (value as IDisposable)?.Dispose();");
                codeWriter.Line();

                foreach (var rootService in root.RootCallSites)
                {
                    if (rootService.IsDisposable == false ||
                        rootService.Lifetime == ServiceLifetime.Singleton && onlyScoped ||
                        rootService.Lifetime == ServiceLifetime.Transient) continue;

                    codeWriter.Line($"TryDispose({GetCacheLocation(rootService)});");
                }

                using (codeWriter.Scope($"if (_disposables != null)"))
                using (codeWriter.Scope($"foreach (var service in _disposables)"))
                {
                    codeWriter.Line($"TryDispose(service);");
                }
            }

            codeWriter.Line();

            using (codeWriter.Scope($"public async {typeof(ValueTask)} DisposeAsync()"))
            {
                using (codeWriter.Scope($"{typeof(ValueTask)} TryDispose(object value)"))
                {
                    using (codeWriter.Scope($"if (value is System.IAsyncDisposable asyncDisposable)"))
                    {
                        codeWriter.Line($"return asyncDisposable.DisposeAsync();");
                    }
                    using (codeWriter.Scope($"else if (value is {typeof(IDisposable)} disposable)"))
                    {
                        codeWriter.Line($"disposable.Dispose();");
                    }
                    codeWriter.Line($"return default;");
                }
                codeWriter.Line();

                foreach (var rootService in root.RootCallSites)
                {
                    if (rootService.IsDisposable == false ||
                        rootService.Lifetime == ServiceLifetime.Singleton && onlyScoped ||
                        rootService.Lifetime == ServiceLifetime.Transient) continue;

                    codeWriter.Line($"await TryDispose({GetCacheLocation(rootService)});");
                }

                using (codeWriter.Scope($"if (_disposables != null)"))
                using (codeWriter.Scope($"foreach (var service in _disposables)"))
                {
                    codeWriter.Line($"await TryDispose(service);");
                }
            }

            codeWriter.Line();
        }

        private static void WriteInterfaces(CodeWriter codeWriter, ServiceProvider root)
        {
            codeWriter.Append($" : {typeof(IDisposable)}, {typeof(IServiceProvider)},");
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
        }

        private void WriteCacheLocations(ServiceProvider root, CodeWriter codeWriter, bool onlyScoped)
        {
            foreach (var rootService in root.RootCallSites)
            {
                if ((rootService.Lifetime == ServiceLifetime.Singleton && onlyScoped) ||
                     rootService.Lifetime == ServiceLifetime.Transient) continue;

                codeWriter.Line($"private {rootService.ImplementationType} {GetCacheLocation(rootService)};");
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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new[]
        {
            DiagnosticDescriptors.UnexpectedErrorDescriptor,
            DiagnosticDescriptors.ServiceRequiredToConstructNotRegistered,
            DiagnosticDescriptors.MemberReferencedByInstanceOrFactoryAttributeNotFound,
            DiagnosticDescriptors.MemberReferencedByInstanceOrFactoryAttributeAmbiguous,
            DiagnosticDescriptors.ServiceProviderTypeHasToBePartial,
            DiagnosticDescriptors.ImportedTypeNotMarkedWithModuleAttribute,
            DiagnosticDescriptors.ImplementationTypeRequiresPublicConstructor,
        }.ToImmutableArray();
    }
}