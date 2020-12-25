using System;
using System.Collections.Generic;
using AutoRest.CSharp.Generation.Writers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Jab
{
    [Generator]
    public class DependencyInjectionContainerGenerator : ISourceGenerator
    {
        private static DiagnosticDescriptor UnexpectedErrorDescriptor = new DiagnosticDescriptor("JAB0001",
            "Unexpected error during generation.",
            "Unexpected error occured during code generation: {0}", "", DiagnosticSeverity.Error, true);

        public void Initialize(GeneratorInitializationContext context)
        {
        }


        private void GenerateCallSite(CodeWriter codeWriter, ServiceCallSite serviceCallSite, Action<CodeWriter, CodeWriterDelegate> valueCallback)
        {
            switch (serviceCallSite)
            {
                case TransientCallSite transientCallSite:
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
                                var rootServiceType = rootService.ServiceType;
                                using (codeWriter.Scope($"{SyntaxFacts.GetText(rootServiceType.DeclaredAccessibility)} {rootServiceType} {GetResolutionServiceName(rootServiceType)}()"))
                                {
                                    GenerateCallSite(codeWriter,
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