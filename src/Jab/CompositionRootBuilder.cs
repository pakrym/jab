using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Jab
{
    internal class CompositionRootBuilder
    {
        private const string TransientAttributeMetadataName = "Jab.TransientAttribute";
        private const string SingletonAttributeMetadataName = "Jab.SingletonAttribute";
        private const string CompositionRootAttributeMetadataName = "Jab.CompositionRootAttribute";
        private const string InstanceAttributePropertyName = "Instance";
        private const string FactoryAttributePropertyName = "Factory";

        private readonly GeneratorExecutionContext _context;
        private readonly INamedTypeSymbol _compositionRootAttributeType;
        private readonly INamedTypeSymbol _transientAttributeType;
        private readonly INamedTypeSymbol _singletonAttribute;

        public CompositionRootBuilder(GeneratorExecutionContext context)
        {
            static INamedTypeSymbol GetTypeByMetadataNameOrThrow(GeneratorExecutionContext context, string fullyQualifiedMetadataName) =>
                context.Compilation.Assembly.GetTypeByMetadataName(fullyQualifiedMetadataName)
                    ?? throw new InvalidOperationException($"Type with metadata '{fullyQualifiedMetadataName}' not found");

            _context = context;
            _compositionRootAttributeType = GetTypeByMetadataNameOrThrow(context, CompositionRootAttributeMetadataName);
            _transientAttributeType = GetTypeByMetadataNameOrThrow(context, TransientAttributeMetadataName);
            _singletonAttribute = GetTypeByMetadataNameOrThrow(context, SingletonAttributeMetadataName);
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

            var description = GetDescription(typeSymbol);
            if (description == null)
            {
                return false;
            }

            Dictionary<ITypeSymbol, ServiceCallSite> callSites =
#pragma warning disable RS1024 // Comparer check currently not working properly
                new(SymbolEqualityComparer.Default);
#pragma warning restore RS1024

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
                        ServiceCallSite callSite;

                        switch (registration)
                        {
                            case { InstanceMember: {} instanceMember }:
                                callSite = new MemberCallSite(registration.ServiceType, instanceMember, false);
                                break;
                            case { FactoryMember: {} factoryMember }:
                                callSite = new MemberCallSite(registration.ServiceType, factoryMember, registration.Lifetime == ServiceLifetime.Singleton);
                                break;
                            default:
                                var implementationType = registration.ImplementationType ??
                                                         registration.ServiceType;

                                var ctor = SelectConstructor(implementationType)
                                           ?? throw new InvalidOperationException($"Public constructor not found for type '{implementationType.Name}'");

                                var parameters = new List<ServiceCallSite>();
                                foreach (var parameterSymbol in ctor.Parameters)
                                {
                                    var parameterCallSite = GetCallSite(parameterSymbol.Type)
                                                            ?? throw new InvalidOperationException($"Failed to resolve parameter of type '{parameterSymbol.Type}'");

                                    parameters.Add(parameterCallSite);
                                }

                                callSite = new ConstructorCallSite(
                                    registration.ServiceType,
                                    implementationType,
                                    parameters.ToArray(),
                                    registration.Lifetime == ServiceLifetime.Singleton);
                                break;
                        }


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

        private CompositionRootDescription? GetDescription(ITypeSymbol typeSymbol)
        {
            bool isCompositionRoot = false;
            List<ServiceRegistration> registrations = new();
            ImmutableArray<AttributeData> attributes = typeSymbol.GetAttributes();
            foreach (var attributeData in attributes)
            {
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _compositionRootAttributeType))
                {
                    isCompositionRoot = true;
                }
                else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _transientAttributeType))
                {
                    registrations.Add(CreateRegistration(typeSymbol, attributeData, ServiceLifetime.Transient));
                }
                else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _singletonAttribute))
                {
                    registrations.Add(CreateRegistration(typeSymbol, attributeData, ServiceLifetime.Singleton));
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

        private ServiceRegistration CreateRegistration(ITypeSymbol typeSymbol, AttributeData attributeData, ServiceLifetime serviceLifetime)
        {
            string? instanceMemberName = null;
            string? factoryMemberName = null;
            foreach (var namedArgument in attributeData.NamedArguments)
            {
                if (namedArgument.Key == InstanceAttributePropertyName)
                {
                    instanceMemberName = (string?)namedArgument.Value.Value;
                }
                else if (namedArgument.Key == FactoryAttributePropertyName)
                {
                    factoryMemberName = (string?)namedArgument.Value.Value;
                }
            }

            ISymbol? instanceMember = null;
            if (instanceMemberName != null)
            {
                var members = typeSymbol.GetMembers(instanceMemberName);
                if (members.Length == 0)
                {
                    throw new InvalidOperationException($"Unable to find a member '{instanceMemberName}' referenced as an instance.");
                }
                if (members.Length > 1)
                {
                    throw new InvalidOperationException($"Found multiple members with the '{instanceMemberName}' name, referenced as an instance.");
                }

                instanceMember = members[0];
            }

            ISymbol? factoryMember = null;
            if (factoryMemberName != null)
            {
                var members = typeSymbol.GetMembers(factoryMemberName);
                if (members.Length == 0)
                {
                    throw new InvalidOperationException($"Unable to find a member '{instanceMemberName}' referenced as a factory.");
                }
                if (members.Length > 1)
                {
                    throw new InvalidOperationException($"Found multiple members with the '{instanceMemberName}' name, referenced as a factory.");
                }

                factoryMember = members[0];
            }

            var serviceType = ExtractType(attributeData.ConstructorArguments[0]);
            var implementationType = attributeData.ConstructorArguments.Length == 2 ? ExtractType(attributeData.ConstructorArguments[1]) : null;

            return new ServiceRegistration(
                serviceLifetime,
                serviceType,
                implementationType,
                instanceMember,
                factoryMember);
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
}