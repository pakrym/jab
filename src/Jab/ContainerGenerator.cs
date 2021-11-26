using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jab
{
    [Generator]
    #pragma warning disable RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    public partial class ContainerGenerator : DiagnosticAnalyzer
    #pragma warning restore RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    {
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
                case ServiceProviderCallSite:
                    valueCallback(codeWriter, w => w.AppendRaw("this"));
                    break;
                case ScopeFactoryCallSite:
                    valueCallback(codeWriter, w => w.AppendRaw(rootReference));
                    break;
            }
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
                    codeWriter.UseNamespace("System.Diagnostics");
                    using (codeWriter.Namespace($"{root.Type.ContainingNamespace.ToDisplayString()}"))
                    {
                        // TODO: implement infinite nesting
                        using CodeWriter.CodeWriterScope? parentTypeScope = root.Type.ContainingType is {} containingType ?
                            codeWriter.Scope($"{SyntaxFacts.GetText(containingType.DeclaredAccessibility)} partial class {containingType.Name}") :
                            null;
                        codeWriter.Append($"{SyntaxFacts.GetText(root.Type.DeclaredAccessibility)} partial class {root.Type.Name}");
                        WriteInterfaces(codeWriter, root, false);
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

                            codeWriter.Line($"[DebuggerHidden]");
                            codeWriter.Line($"public T GetService<T>() => ((IServiceProvider<T>)this).GetService();");
                            codeWriter.Line();

                            codeWriter.Line($"public Scope CreateScope() => new Scope(this);");
                            codeWriter.Line();

                            if (root.KnownTypes.IServiceScopeFactoryType != null)
                            {
                                codeWriter.Line($"{root.KnownTypes.IServiceScopeType} {root.KnownTypes.IServiceScopeFactoryType}.CreateScope() => this.CreateScope();");
                                codeWriter.Line();
                            }

                            codeWriter.Append($"public partial class Scope");
                            WriteInterfaces(codeWriter, root, true);
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

                                codeWriter.Line($"[DebuggerHidden]");
                                codeWriter.Line($"public T GetService<T>() => ((IServiceProvider<T>)this).GetService();");
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

                                WriteServiceProvider(codeWriter, root);

                                if (root.KnownTypes.IServiceScopeType != null)
                                {
                                    codeWriter.Line($"{root.KnownTypes.IServiceProviderType} {root.KnownTypes.IServiceScopeType}.ServiceProvider => this;");
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
            using (codeWriter.Scope($"{typeof(object)} {typeof(IServiceProvider)}.GetService({typeof(Type)} type)"))
            {
                foreach (var rootRootCallSite in root.RootCallSites)
                {
                    if (rootRootCallSite.IsMainImplementation)
                    {
                        codeWriter.Append($"if (type == typeof({rootRootCallSite.ServiceType})) return ");
                        WriteResolutionCall(codeWriter, rootRootCallSite, "this");
                        codeWriter.Line($";");
                    }
                }

                codeWriter.Line($"return null;");
            }

            codeWriter.Line();
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

        private static void WriteInterfaces(CodeWriter codeWriter, ServiceProvider root, bool isScope)
        {
            codeWriter.Line($" : {typeof(IDisposable)},");
            codeWriter.Line($"   {typeof(IServiceProvider)},");

            if (!isScope && root.KnownTypes.IServiceScopeFactoryType != null)
            {
                codeWriter.Line($"   {root.KnownTypes.IServiceScopeFactoryType},");
            }

            if (isScope && root.KnownTypes.IServiceScopeType != null)
            {
                codeWriter.Line($"   {root.KnownTypes.IServiceScopeType},");
            }

            foreach (var serviceCallSite in root.RootCallSites)
            {
                if (serviceCallSite.IsMainImplementation)
                {
                    codeWriter.Append($"   IServiceProvider<{serviceCallSite.ServiceType}>,");
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
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var syntaxCollector = new SyntaxCollector();
                compilationStartAnalysisContext.RegisterSyntaxNodeAction(analysisContext =>
                {
                    syntaxCollector.OnVisitSyntaxNode(analysisContext.Node);
                }, SyntaxKind.Attribute, SyntaxKind.InvocationExpression);

                compilationStartAnalysisContext.RegisterCompilationEndAction(compilationContext =>
                {
                    Execute(new GeneratorContext(compilationContext, syntaxCollector));
                });
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
            DiagnosticDescriptors.CyclicDependencyDetected,
            DiagnosticDescriptors.MissingServiceProviderAttribute,
        }.ToImmutableArray();
    }
}