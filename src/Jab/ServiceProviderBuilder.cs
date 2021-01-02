using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        private const string RootServicesAttributePropertyName = "RootServices";

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

            foreach (var declaringSyntaxReference in typeSymbol.DeclaringSyntaxReferences)
            {
                if (declaringSyntaxReference.GetSyntax() is ClassDeclarationSyntax typeDeclarationSyntax &&
                    !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    _context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.UnexpectedErrorDescriptor,
                        typeDeclarationSyntax.Identifier.GetLocation(),
                        "The type marked with the ServiceProvider attribute has to be marked partial."
                    ));
                }

            }

            Dictionary<CallSiteCacheKey, ServiceCallSite> callSites = new();

            foreach (var registration in description.ServiceRegistrations)
            {
                if (registration.ServiceType.IsUnboundGenericType)
                {
                    continue;
                }
                GetCallSite(description, callSites, registration.ServiceType);
            }

            foreach (var rootService in description.RootServices)
            {
                GetCallSite(description, callSites, rootService);
            }

            foreach (var candidateGetServiceCall in _context.CandidateGetServiceCalls)
            {
                var semanticModel = _context.Compilation.GetSemanticModel(candidateGetServiceCall.SyntaxTree);
                if (candidateGetServiceCall.Expression is MemberAccessExpressionSyntax
                {
                    Name: GenericNameSyntax
                    {
                        IsUnboundGenericName: false,
                        TypeArgumentList: { Arguments: { Count: 1 } arguments }
                    }
                } memberAccessExpression)
                {
                    var containerTypeInfo = ModelExtensions.GetTypeInfo(semanticModel, memberAccessExpression.Expression);
                    var serviceInfo = ModelExtensions.GetSymbolInfo(semanticModel, arguments[0]);

                    if (SymbolEqualityComparer.Default.Equals(containerTypeInfo.Type, typeSymbol) &&
                        serviceInfo.Symbol is INamedTypeSymbol serviceSymbol)
                    {
                        GetCallSite(description, callSites, serviceSymbol);
                    }

                }
            }

            compositionRoot = new ServiceProvider(typeSymbol, callSites.Values.ToArray());
            return true;
        }

        private ServiceCallSite? GetCallSite(
            ServiceProviderDescription description,
            Dictionary<CallSiteCacheKey, ServiceCallSite> callSiteCache,
            ITypeSymbol serviceType)
        {
            if (callSiteCache.TryGetValue(new CallSiteCacheKey(serviceType), out var cachedCallSite))
            {
                return cachedCallSite;
            }

            return TryCreateExact(description, callSiteCache, serviceType) ??
                   TryCreateEnumerable(description, callSiteCache, serviceType) ??
                   TryCreateGeneric(description, callSiteCache, serviceType);
        }

        private ServiceCallSite? TryCreateGeneric(ServiceProviderDescription description,
            Dictionary<CallSiteCacheKey, ServiceCallSite> callSiteCache,
            ITypeSymbol serviceType)
        {
            if (serviceType is INamedTypeSymbol { IsGenericType: true })
            {
                for (int i = description.ServiceRegistrations.Count - 1; i >= 0; i--)
                {
                    var registration = description.ServiceRegistrations[i];

                    var callSite = TryCreateGeneric(description, callSiteCache, serviceType, registration, 0);
                    if (callSite != null)
                    {
                        return callSite;
                    }
                }
            }

            return null;
        }


        private ServiceCallSite? TryCreateGeneric(ServiceProviderDescription description,
            Dictionary<CallSiteCacheKey, ServiceCallSite> callSiteCache,
            ITypeSymbol serviceType,
            ServiceRegistration registration,
            int reverseIndex)
        {
            if (serviceType is INamedTypeSymbol {IsGenericType: true} genericType &&
                registration.ServiceType.IsUnboundGenericType &&
                SymbolEqualityComparer.Default.Equals(registration.ServiceType.ConstructedFrom, genericType.ConstructedFrom))
            {
                var implementationType = registration.ImplementationType!.ConstructedFrom.Construct(genericType.TypeArguments, genericType.TypeArgumentNullableAnnotations);
                return CreateConstructorCallSite(description, callSiteCache, registration, genericType, implementationType, reverseIndex);
            }

            return null;
        }

        private ServiceCallSite? TryCreateEnumerable(ServiceProviderDescription description, Dictionary<CallSiteCacheKey, ServiceCallSite> callSiteCache, ITypeSymbol serviceType)
        {
            if (serviceType is INamedTypeSymbol {IsGenericType: true} genericType &&
                SymbolEqualityComparer.Default.Equals(genericType.ConstructedFrom, _iEnumerableType))
            {
                var enumerableService = genericType.TypeArguments[0];
                var items = new List<ServiceCallSite>();
                int reverseIndex = 0;
                for (int i = description.ServiceRegistrations.Count - 1; i >= 0; i--)
                {
                    var registration = description.ServiceRegistrations[i];

                    var itemCallSite = TryCreateGeneric(description, callSiteCache, enumerableService, registration, reverseIndex) ??
                                       TryCreateExact(description, callSiteCache, enumerableService, registration, reverseIndex);
                    if (itemCallSite != null)
                    {
                        reverseIndex++;
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

                callSiteCache[new CallSiteCacheKey(reverseIndex, serviceType)] = callSite;

                return callSite;
            }

            return null;
        }

        private ServiceCallSite? TryCreateExact(ServiceProviderDescription description, Dictionary<CallSiteCacheKey, ServiceCallSite> callSiteCache, ITypeSymbol serviceType)
        {
            for (int i = description.ServiceRegistrations.Count - 1; i >= 0; i--)
            {
                var registration = description.ServiceRegistrations[i];

                if (TryCreateExact(description, callSiteCache, serviceType, registration, 0) is { } callSite)
                {
                    return callSite;
                }
            }

            return null;
        }

        private ServiceCallSite? TryCreateExact(ServiceProviderDescription description, Dictionary<CallSiteCacheKey, ServiceCallSite> callSiteCache, ITypeSymbol serviceType, ServiceRegistration registration, int reverseIndex)
        {
            if (SymbolEqualityComparer.Default.Equals(registration.ServiceType, serviceType))
            {
                return CreateCallSite(description, callSiteCache, registration, reverseIndex: reverseIndex);
            }

            return null;
        }

        private ServiceCallSite CreateCallSite(
            ServiceProviderDescription description,
            Dictionary<CallSiteCacheKey, ServiceCallSite> callSiteCache,
            ServiceRegistration registration,
            int reverseIndex)
        {
            var cacheKey = new CallSiteCacheKey(reverseIndex, registration.ServiceType);

            if (callSiteCache.TryGetValue(cacheKey, out ServiceCallSite callSite))
            {
                return callSite;
            }

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

                    callSite = CreateConstructorCallSite(description, callSiteCache, registration, registration.ServiceType, implementationType, reverseIndex);
                    break;
            }

            callSiteCache[cacheKey] = callSite;

            return callSite;
        }

        private ServiceCallSite CreateConstructorCallSite(ServiceProviderDescription description,
            Dictionary<CallSiteCacheKey, ServiceCallSite> callSiteCache,
            ServiceRegistration registration,
            INamedTypeSymbol serviceType,
            INamedTypeSymbol implementationType,
            int reverseIndex)
        {

            var cacheKey = new CallSiteCacheKey(reverseIndex, registration.ServiceType);

            if (callSiteCache.TryGetValue(cacheKey, out ServiceCallSite callSite))
            {
                return callSite;
            }

            var ctor = SelectConstructor(implementationType)
                       ?? throw new InvalidOperationException(
                           $"Public constructor not found for type '{implementationType.Name}'");

            var parameters = new List<ServiceCallSite>();
            foreach (var parameterSymbol in ctor.Parameters)
            {
                var parameterCallSite = GetCallSite(description, callSiteCache, parameterSymbol.Type)
                                        ?? throw new InvalidOperationException(
                                            $"Failed to resolve parameter of type '{parameterSymbol.Type}'");

                parameters.Add(parameterCallSite);
            }

            callSite = new ConstructorCallSite(
                serviceType,
                implementationType,
                parameters.ToArray(),
                registration.Lifetime == ServiceLifetime.Singleton,
                reverseIndex);

            callSiteCache[cacheKey] = callSite;

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
            List<ITypeSymbol> rootServices = new();
            foreach (var attributeData in attributes)
            {
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _compositionRootAttributeType))
                {
                    isCompositionRoot = true;
                    foreach (var namedArgument in attributeData.NamedArguments)
                    {
                        if (namedArgument.Key == RootServicesAttributePropertyName)
                        {
                            foreach (var typedConstant in namedArgument.Value.Values)
                            {
                                rootServices.Add(ExtractType(typedConstant));
                            }
                        }
                    }

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
                return new ServiceProviderDescription(registrations, rootServices.ToArray());
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

        private readonly struct CallSiteCacheKey : IEquatable<CallSiteCacheKey>
        {
            public CallSiteCacheKey(ITypeSymbol type) : this(0, type)
            {
            }

            public CallSiteCacheKey(int reverseIndex, ITypeSymbol type)
            {
                ReverseIndex = reverseIndex;
                Type = type;
            }

            public int ReverseIndex { get; }
            public ITypeSymbol Type { get; }

            public bool Equals(CallSiteCacheKey other)
            {
                return ReverseIndex == other.ReverseIndex && SymbolEqualityComparer.Default.Equals(Type, other.Type);
            }

            public override bool Equals(object? obj)
            {
                return obj is CallSiteCacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (ReverseIndex * 397) ^ SymbolEqualityComparer.Default.GetHashCode(Type);
                }
            }
        }
    }
}