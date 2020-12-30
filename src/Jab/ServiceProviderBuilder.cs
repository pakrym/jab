using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Jab
{
    internal class ServiceProviderBuilder
    {
        private const string TransientAttributeMetadataName = "Jab.TransientAttribute";
        private const string SingletonAttributeMetadataName = "Jab.SingletonAttribute";
        private const string CompositionRootAttributeMetadataName = "Jab.ServiceProviderAttribute";
        private const string IEnumerableMetadataName = "System.Collections.Generic.IEnumerable`1";
        private const string InstanceAttributePropertyName = "Instance";
        private const string FactoryAttributePropertyName = "Factory";

        private readonly GeneratorContext _context;
        private readonly INamedTypeSymbol _iEnumerableType;
        private readonly INamedTypeSymbol _compositionRootAttributeType;
        private readonly INamedTypeSymbol _transientAttributeType;
        private readonly INamedTypeSymbol _singletonAttribute;

        public ServiceProviderBuilder(GeneratorContext context)
        {
            static INamedTypeSymbol GetTypeByMetadataNameOrThrow(GeneratorContext context, string fullyQualifiedMetadataName) =>
                context.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName)
                    ?? throw new InvalidOperationException($"Type with metadata '{fullyQualifiedMetadataName}' not found");

            _context = context;
            _iEnumerableType = GetTypeByMetadataNameOrThrow(context, IEnumerableMetadataName);
            _compositionRootAttributeType = GetTypeByMetadataNameOrThrow(context, CompositionRootAttributeMetadataName);
            _transientAttributeType = GetTypeByMetadataNameOrThrow(context, TransientAttributeMetadataName);
            _singletonAttribute = GetTypeByMetadataNameOrThrow(context, SingletonAttributeMetadataName);
        }

        public ServiceProvider[] BuildRoots()
        {
            List<ServiceProvider> compositionRoots = new();

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

        private bool TryCreateCompositionRoot(INamedTypeSymbol typeSymbol, [NotNullWhen(true)] out ServiceProvider? compositionRoot)
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

            foreach (var registration in description.ServiceRegistrations)
            {
                GetCallSite(description, callSites, services, registration.ServiceType);
            }

            compositionRoot = new ServiceProvider(typeSymbol, services.ToArray());
            return true;
        }

        private ServiceCallSite? GetCallSite(
            ServiceProviderDescription description,
            Dictionary<ITypeSymbol, ServiceCallSite> callSiteCache,
            List<ServiceCallSite> allCallSites,
            ITypeSymbol serviceType)
        {
            if (callSiteCache.TryGetValue(serviceType, out var cachedCallSite))
            {
                return cachedCallSite;
            }

            if (serviceType is INamedTypeSymbol {IsGenericType: true} genericType)
            {
                if (SymbolEqualityComparer.Default.Equals(genericType.ConstructedFrom, _iEnumerableType))
                {
                    var enumerableService = genericType.TypeArguments[0];
                    var items = new List<ServiceCallSite>();
                    int reverseIndex = 0;
                    for (int i = description.ServiceRegistrations.Count - 1; i >= 0; i--)
                    {
                        var registration = description.ServiceRegistrations[i];

                        if (SymbolEqualityComparer.Default.Equals(registration.ServiceType, enumerableService))
                        {
                            ServiceCallSite itemCallSite = CreateCallSite(description, callSiteCache, allCallSites, registration, reverseIndex);
                            if (reverseIndex == 0)
                            {
                                callSiteCache.Add(enumerableService, itemCallSite);
                            }
                            reverseIndex++;
                            allCallSites.Add(itemCallSite);
                            items.Add(itemCallSite);
                        }
                    }

                    var serviceCallSites = items.ToArray();
                    Array.Reverse(serviceCallSites);
                    var callSite = new ArrayServiceCallSite(
                        genericType,
                        genericType,
                        enumerableService,
                        serviceCallSites, items.All(i => i.Singleton));

                    callSiteCache.Add(serviceType, callSite);
                    allCallSites.Add(callSite);

                    return callSite;
                }
            }

            for (int i = description.ServiceRegistrations.Count - 1; i >= 0; i--)
            {
                var registration = description.ServiceRegistrations[i];

                if (SymbolEqualityComparer.Default.Equals(registration.ServiceType, serviceType))
                {
                    ServiceCallSite callSite = CreateCallSite(description, callSiteCache, allCallSites, registration, reverseIndex: 0);

                    callSiteCache.Add(serviceType, callSite);
                    allCallSites.Add(callSite);

                    return callSite;
                }
            }

            return null;
        }

        private ServiceCallSite CreateCallSite(
            ServiceProviderDescription description,
            Dictionary<ITypeSymbol, ServiceCallSite> callSiteCache,
            List<ServiceCallSite> allCallSites,
            ServiceRegistration registration,
            int reverseIndex)
        {
            ServiceCallSite callSite;

            switch (registration)
            {
                case {InstanceMember: { } instanceMember}:
                    callSite = new MemberCallSite(registration.ServiceType, instanceMember, false, reverseIndex);
                    break;
                case {FactoryMember: { } factoryMember}:
                    callSite = new MemberCallSite(registration.ServiceType, factoryMember, registration.Lifetime == ServiceLifetime.Singleton, reverseIndex);
                    break;
                default:
                    var implementationType = registration.ImplementationType ??
                                             registration.ServiceType;

                    var ctor = SelectConstructor(implementationType)
                               ?? throw new InvalidOperationException($"Public constructor not found for type '{implementationType.Name}'");

                    var parameters = new List<ServiceCallSite>();
                    foreach (var parameterSymbol in ctor.Parameters)
                    {
                        var parameterCallSite = GetCallSite(description, callSiteCache, allCallSites, parameterSymbol.Type)
                                                ?? throw new InvalidOperationException($"Failed to resolve parameter of type '{parameterSymbol.Type}'");

                        parameters.Add(parameterCallSite);
                    }

                    callSite = new ConstructorCallSite(
                        registration.ServiceType,
                        implementationType,
                        parameters.ToArray(),
                        registration.Lifetime == ServiceLifetime.Singleton,
                        reverseIndex);
                    break;
            }

            return callSite;
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

        private ServiceProviderDescription? GetDescription(ITypeSymbol typeSymbol)
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
                else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _transientAttributeType) &&
                         CreateRegistration(typeSymbol, attributeData, ServiceLifetime.Transient, out var registration))
                {
                    registrations.Add(registration);
                }
                else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _singletonAttribute) &&
                         CreateRegistration(typeSymbol, attributeData, ServiceLifetime.Singleton, out registration))
                {
                    registrations.Add(registration);
                }
            }

            if (isCompositionRoot)
            {
                return new ServiceProviderDescription(registrations);
            }
            else
            {
                // TODO: Diagnostic
                return null;
            }
        }

        private bool CreateRegistration(ITypeSymbol typeSymbol, AttributeData attributeData, ServiceLifetime serviceLifetime, [NotNullWhen(true)] out ServiceRegistration? registration)
        {
            registration = null;

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
            if (instanceMemberName != null &&
                !TryFindMember(typeSymbol, attributeData, instanceMemberName, InstanceAttributePropertyName, out instanceMember))
            {
                return false;
            }

            ISymbol? factoryMember = null;
            if (factoryMemberName != null&&
                !TryFindMember(typeSymbol, attributeData, factoryMemberName, FactoryAttributePropertyName, out factoryMember))
            {
                return false;
            }

            var serviceType = ExtractType(attributeData.ConstructorArguments[0]);
            var implementationType = attributeData.ConstructorArguments.Length == 2 ? ExtractType(attributeData.ConstructorArguments[1]) : null;

            registration = new ServiceRegistration(
                serviceLifetime,
                serviceType,
                implementationType,
                instanceMember,
                factoryMember);

            return true;
        }

        private bool TryFindMember(ITypeSymbol typeSymbol, AttributeData attributeData, string memberName, string parameterName, [NotNullWhen(true)] out ISymbol? instanceMember)
        {
            instanceMember = null;

            var members = typeSymbol.GetMembers(memberName);
            if (members.Length == 0)
            {
                _context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.UnexpectedErrorDescriptor,
                    attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                    $"Unable to find a member '{memberName}' referenced in the '{parameterName}' attribute parameter."
                ));
                return false;
            }

            if (members.Length > 1)
            {
                _context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.UnexpectedErrorDescriptor,
                    attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                    $"Found multiple members with the '{memberName}' name, referenced in the '{parameterName}' attribute parameter."
                ));
            }

            instanceMember = members[0];
            return true;
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