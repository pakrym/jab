using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jab
{
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor UnexpectedErrorDescriptor = new DiagnosticDescriptor("JAB0001",
            "Unexpected error during generation",
            "Unexpected error occurred during code generation: {0}", "Usage", DiagnosticSeverity.Error, true);
    }

    [Generator]
    #pragma warning disable RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    public class DependencyInjectionContainerGenerator : DiagnosticAnalyzer, ISourceGenerator
    #pragma warning restore RS1001 // We don't want this to be discovered as analyzer but it simplifies testing
    {
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
                    valueCallback(codeWriter, w =>
                    {
                        w.AppendRaw(memberCallSite.Member.Name);
                        if (memberCallSite.Member is IMethodSymbol)
                        {
                            w.AppendRaw("()");
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
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.UnexpectedErrorDescriptor, Location.None, e.ToString()));
            }
        }

        private string GetResolutionServiceName(INamedTypeSymbol rootServiceType) => $"Get{rootServiceType.Name}";

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