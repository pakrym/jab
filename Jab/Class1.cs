using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoRest.CSharp.Generation.Writers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Jab
{
    internal class CallSiteBuilder
    {
        private readonly GeneratorExecutionContext _context;
        private readonly INamedTypeSymbol _compositionRootAttributeType;
        private readonly INamedTypeSymbol _transientAttributeType;

        public CallSiteBuilder(GeneratorExecutionContext context)
        {
            _context = context;
            _compositionRootAttributeType = context.Compilation.Assembly.GetTypeByMetadataName(CompositionRootKnownAttribute.MetadataName);
            _transientAttributeType = context.Compilation.Assembly.GetTypeByMetadataName(TransientKnownAttribute.MetadataName);
        }

        public CompositionRoot[] BuildRoots()
        {
            List<CompositionRoot> compositionRoots = new();

            void ProcessNamespace(INamespaceSymbol ns)
            {
                foreach (var namespaceSymbol in ns.GetNamespaceMembers())
                {
                    ProcessNamespace(namespaceSymbol);
                }

                foreach (var typeSymbol in ns.GetTypeMembers())
                {
                    if (TryCreateCompositionRoot(typeSymbol, out var compositionRoot))
                    {
                        compositionRoots.Add(compositionRoot);
                    }
                }
            }
            foreach (var assemblyModule in _context.Compilation.Assembly.Modules)
            {
                ProcessNamespace(assemblyModule.GlobalNamespace);
            }

            return compositionRoots.ToArray();
        }

        private bool TryCreateCompositionRoot(INamedTypeSymbol typeSymbol, [NotNullWhen(true)] out CompositionRoot? compositionRoot)
        {
            compositionRoot = null;

            var attributes = CollectKnownAttributes(typeSymbol.GetAttributes());
            if (!attributes.Any(a => a is CompositionRootKnownAttribute))
            {
                return false;
            }

            Dictionary<INamedTypeSymbol, ServiceCallSite> callSites = new(SymbolEqualityComparer.Default);
            List<INamedTypeSymbol> services = new();


            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case TransientKnownAttribute transientKnownAttribute:
                        callSites.Add(transientKnownAttribute.ServiceType,
                            new TransientCallSite(transientKnownAttribute.ServiceType, transientKnownAttribute.ImplementationType ?? transientKnownAttribute.ServiceType));
                        services.Add(transientKnownAttribute.ServiceType);
                        break;
                }
            }

            compositionRoot = new CompositionRoot(typeSymbol, callSites.Values.ToArray(), services.ToArray());
            return true;
        }

        private object[] CollectKnownAttributes(ImmutableArray<AttributeData> attributes)
        {
            List<object> knownAttributes = new();
            foreach (var attributeData in attributes)
            {
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _compositionRootAttributeType))
                {
                    knownAttributes.Add(new CompositionRootKnownAttribute(attributeData));
                }
                else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _transientAttributeType))
                {
                    knownAttributes.Add(new TransientKnownAttribute(attributeData,
                        ExtractType(attributeData.ConstructorArguments[0]),
                        attributeData.ConstructorArguments.Length == 2? ExtractType(attributeData.ConstructorArguments[1]) : null)
                        );
                }
            }

            return knownAttributes.ToArray();
        }

        private INamedTypeSymbol ExtractType(TypedConstant typedConstant)
        {
            if (typedConstant.Kind != TypedConstantKind.Type)
            {
                throw new InvalidOperationException($"Unexpected constant kind '{typedConstant.Kind}', expected 'Type'");
            }

            if (typedConstant.Value == null)
            {
                throw new InvalidOperationException($"Unexpected null value for type constant.");
            }

            return (INamedTypeSymbol) typedConstant.Value;
        }
    }

    internal record TransientCallSite : ServiceCallSite
    {
        public TransientCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType) : base(serviceType, implementationType)
        {
        }
    }

    internal record CompositionRoot
    {
        public CompositionRoot(ITypeSymbol type, ServiceCallSite[] callSites, ITypeSymbol[] services)
        {
            CallSites = callSites;
            Services = services;
            Type = type;
        }

        public ITypeSymbol Type { get; }

        public ITypeSymbol[] Services { get; }

        public ServiceCallSite[] CallSites { get; }

        public ServiceCallSite GetCallSite(ITypeSymbol service)
        {
            return CallSites.First(s => SymbolEqualityComparer.Default.Equals(s.ServiceType, service));
        }
    }

    internal abstract record ServiceCallSite
    {
        protected ServiceCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }

        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol ImplementationType { get; }
    }

    [Generator]
    public class MySourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }


        private void GenerateCallSite(CodeWriter codeWriter, ServiceCallSite serviceCallSite, Action<CodeWriter, CodeWriterDelegate> valueCallback)
        {
            switch (serviceCallSite)
            {
                case TransientCallSite transientCallSite:
                    valueCallback(codeWriter, w => w.Append($"new {transientCallSite.ImplementationType}()"));
                    break;
            }
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var roots = new CallSiteBuilder(context).BuildRoots();

            foreach (var root in roots)
            {
                var codeWriter = new CodeWriter();
                using (codeWriter.Namespace($"{root.Type.ContainingNamespace.ToDisplayString()}"))
                {
                    using (codeWriter.Scope($"{SyntaxFacts.GetText(root.Type.DeclaredAccessibility)} partial class {root.Type.Name}"))
                    {
                        foreach (var rootService in root.Services)
                        {
                            using (codeWriter.Scope($"{SyntaxFacts.GetText(rootService.DeclaredAccessibility)} {rootService} Get{root.Type.Name}()"))
                            {
                                GenerateCallSite(codeWriter,
                                    root.GetCallSite(rootService),
                                    (w, v) => w.Line($"return {v};"));
                            }
                        }

                    }
                }
                context.AddSource($"{root.Type.Name}.Generated.cs", codeWriter.ToString());
            }
        }
    }
}