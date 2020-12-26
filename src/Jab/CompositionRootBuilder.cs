using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Jab
{
    internal class CompositionRootBuilder
    {
        public const string TransientAttributeMetadataName = "Jab.TransientAttribute";
        public const string SingletonAttributeMetadataName = "Jab.SingletonAttribute";
        public const string CompositionRootAttributeMetadataName = "Jab.CompositionRootAttribute";

        private readonly GeneratorExecutionContext _context;
        private readonly INamedTypeSymbol _compositionRootAttributeType;
        private readonly INamedTypeSymbol _transientAttributeType;
        private readonly INamedTypeSymbol _singletonAttribute;

        public CompositionRootBuilder(GeneratorExecutionContext context)
        {
            _context = context;
            _compositionRootAttributeType = context.Compilation.Assembly.GetTypeByMetadataName(CompositionRootAttributeMetadataName);
            _transientAttributeType = context.Compilation.Assembly.GetTypeByMetadataName(TransientAttributeMetadataName);
            _singletonAttribute = context.Compilation.Assembly.GetTypeByMetadataName(SingletonAttributeMetadataName);
        }

        public CompositionRoot[] BuildRoots()
        {
            List<CompositionRoot> compositionRoots = new();

            void ProcessType(INamedTypeSymbol typeSymbol)
            {
                if (TryCreateCompositionRoot(typeSymbol, out var compositionRoot))
                {
                    compositionRoots.Add(compositionRoot);
                }

                foreach (var typeMember in typeSymbol.GetTypeMembers())
                {
                    ProcessType(typeMember);
                }
            }

            void ProcessNamespace(INamespaceSymbol ns)
            {
                foreach (var namespaceSymbol in ns.GetNamespaceMembers())
                {
                    ProcessNamespace(namespaceSymbol);
                }

                foreach (var typeSymbol in ns.GetTypeMembers())
                {
                    ProcessType(typeSymbol);
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

            var description = GetDescription(typeSymbol.GetAttributes());
            if (description == null)
            {
                return false;
            }

            Dictionary<ITypeSymbol, ServiceCallSite> callSites = new(SymbolEqualityComparer.Default);
            List<ServiceCallSite> services = new();

            ServiceCallSite? GetCallSite(ITypeSymbol serviceType)
            {
                if (callSites.TryGetValue(serviceType, out var cachedCallSite))
                {
                    return cachedCallSite;
                }

                for (int i = description.ServiceRegistrations.Count - 1; i >= 0; i--)
                {
                    var registration = description.ServiceRegistrations[i];

                    if (SymbolEqualityComparer.Default.Equals(registration.ServiceType, serviceType))
                    {
                        var implementationType = registration.ImplementationType ??
                                                 registration.ServiceType;
                        var ctor = SelectConstructor(implementationType);

                        var parameters = new List<ServiceCallSite>();
                        foreach (var parameterSymbol in ctor.Parameters)
                        {
                            parameters.Add(GetCallSite(parameterSymbol.Type));
                        }

                        ConstructorCallSite callSite = new(
                            registration.ServiceType,
                            implementationType,
                            parameters.ToArray(),
                            registration.Lifetime == ServiceLifetime.Singleton);

                        callSites.Add(registration.ServiceType, callSite);
                        services.Add(callSite);

                        return callSite;
                    }
                }

                return null;
            }

            foreach (var registration in description.ServiceRegistrations)
            {
                GetCallSite(registration.ServiceType);
            }

            compositionRoot = new CompositionRoot(typeSymbol, services.ToArray());
            return true;
        }

        private IMethodSymbol? SelectConstructor(INamedTypeSymbol implementationType)
        {
            IMethodSymbol? selectedCtor = null;
            foreach (var constructor in implementationType.Constructors)
            {
                if (constructor.DeclaredAccessibility == Accessibility.Public &&
                    constructor.Parameters.Length > (selectedCtor?.Parameters.Length ?? -1))
                {
                    selectedCtor = constructor;
                }
            }

            return selectedCtor;
        }

        private CompositionRootDescription? GetDescription(ImmutableArray<AttributeData> attributes)
        {
            bool isCompositionRoot = false;
            List<ServiceRegistration> registrations = new();
            foreach (var attributeData in attributes)
            {
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _compositionRootAttributeType))
                {
                    isCompositionRoot = true;
                }
                else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _transientAttributeType))
                {
                    registrations.Add(new ServiceRegistration(
                        ServiceLifetime.Transient,
                        ExtractType(attributeData.ConstructorArguments[0]),
                        attributeData.ConstructorArguments.Length == 2? ExtractType(attributeData.ConstructorArguments[1]) : null)
                    );
                }
                else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _singletonAttribute))
                {
                    registrations.Add(new ServiceRegistration(
                        ServiceLifetime.Singleton,
                        ExtractType(attributeData.ConstructorArguments[0]),
                        attributeData.ConstructorArguments.Length == 2? ExtractType(attributeData.ConstructorArguments[1]) : null)
                    );
                }
            }

            if (isCompositionRoot)
            {
                return new CompositionRootDescription(registrations);
            }
            else
            {
                // TODO: Diagnostic
                return null;
            }
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

    internal class CompositionRootDescription
    {
        public CompositionRootDescription(IReadOnlyList<ServiceRegistration> serviceRegistrations)
        {
            ServiceRegistrations = serviceRegistrations;
        }

        public IReadOnlyList<ServiceRegistration> ServiceRegistrations { get; }
    }
}