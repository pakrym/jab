namespace Jab;

internal class KnownTypes
{
    public const string JabAttributesAssemblyName = "Jab.Attributes";
    public const string TransientAttributeShortName = "Transient";
    public const string SingletonAttributeShortName = "Singleton";
    public const string ScopedAttributeShortName = "Scoped";
    public const string CompositionRootAttributeShortName = "ServiceProvider";
    public const string ServiceProviderModuleAttributeShortName = "ServiceProviderModule";
    public const string ImportAttributeShortName = "Import";

    public const string TransientAttributeTypeName = $"{TransientAttributeShortName}Attribute";
    public const string SingletonAttributeTypeName = $"{SingletonAttributeShortName}Attribute";
    public const string ScopedAttributeTypeName = $"{ScopedAttributeShortName}Attribute";
    public const string CompositionRootAttributeTypeName = $"{CompositionRootAttributeShortName}Attribute";
    public const string ServiceProviderModuleAttributeTypeName = $"{ServiceProviderModuleAttributeShortName}Attribute";

    public const string ImportAttributeTypeName = $"{ImportAttributeShortName}Attribute";

    public const string TransientAttributeMetadataName = $"Jab.{TransientAttributeTypeName}";
    public const string GenericTransientAttributeMetadataName = $"Jab.{TransientAttributeTypeName}`1";
    public const string Generic2TransientAttributeMetadataName = $"Jab.{TransientAttributeTypeName}`2";

    public const string SingletonAttributeMetadataName = $"Jab.{SingletonAttributeTypeName}";
    public const string GenericSingletonAttributeMetadataName = $"Jab.{SingletonAttributeTypeName}`1";
    public const string Generic2SingletonAttributeMetadataName = $"Jab.{SingletonAttributeTypeName}`2";


    public const string ScopedAttributeMetadataName = $"Jab.{ScopedAttributeTypeName}";
    public const string GenericScopedAttributeMetadataName = $"Jab.{ScopedAttributeTypeName}`1";
    public const string Generic2ScopedAttributeMetadataName = $"Jab.{ScopedAttributeTypeName}`2";

    public const string CompositionRootAttributeMetadataName = $"Jab.{CompositionRootAttributeTypeName}";
    public const string ServiceProviderModuleAttributeMetadataName = $"Jab.{ServiceProviderModuleAttributeTypeName}";

    public const string ImportAttributeMetadataName = $"Jab.{ImportAttributeTypeName}";
    public const string GenericImportAttributeMetadataName = $"Jab.{ImportAttributeTypeName}`1";

    public const string InstanceAttributePropertyName = "Instance";
    public const string FactoryAttributePropertyName = "Factory";
    public const string RootServicesAttributePropertyName = "RootServices";

    private const string IAsyncDisposableMetadataName = "System.IAsyncDisposable";
    private const string IEnumerableMetadataName = "System.Collections.Generic.IEnumerable`1";
    private const string IServiceProviderMetadataName = "System.IServiceProvider";
    private const string IServiceScopeMetadataName = "Microsoft.Extensions.DependencyInjection.IServiceScope";

    private const string IServiceScopeFactoryMetadataName =
        "Microsoft.Extensions.DependencyInjection.IServiceScopeFactory";

    private const string IServiceProviderIsServiceMetadataName =
        "Microsoft.Extensions.DependencyInjection.IServiceProviderIsService";

    public INamedTypeSymbol IEnumerableType { get; }
    public INamedTypeSymbol IServiceProviderType { get; }
    public INamedTypeSymbol CompositionRootAttributeType { get; }
    public INamedTypeSymbol TransientAttributeType { get; }
    public INamedTypeSymbol? GenericTransientAttributeType { get; }
    public INamedTypeSymbol? Generic2TransientAttributeType { get; }

    public INamedTypeSymbol SingletonAttribute { get; }
    public INamedTypeSymbol? GenericSingletonAttribute { get; }
    public INamedTypeSymbol? Generic2SingletonAttribute { get; }

    public INamedTypeSymbol ImportAttribute { get; }
    public INamedTypeSymbol? GenericImportAttribute { get; }

    public INamedTypeSymbol ModuleAttribute { get; }
    public INamedTypeSymbol ScopedAttribute { get; }
    public INamedTypeSymbol? GenericScopedAttribute { get; }
    public INamedTypeSymbol? Generic2ScopedAttribute { get; }
    public INamedTypeSymbol? IAsyncDisposableType { get; }
    public INamedTypeSymbol? IServiceScopeType { get; }
    public INamedTypeSymbol? IServiceScopeFactoryType { get; }
    public INamedTypeSymbol? IServiceProviderIsServiceType { get; }

    public KnownTypes(Compilation compilation, IAssemblySymbol assemblySymbol)
    {
        static INamedTypeSymbol GetTypeByMetadataNameOrThrow(IAssemblySymbol assemblySymbol,
            string fullyQualifiedMetadataName) =>
            assemblySymbol.GetTypeByMetadataName(fullyQualifiedMetadataName)
            ?? throw new MissingTypeException(fullyQualifiedMetadataName);

        static INamedTypeSymbol GetTypeFromCompilationByMetadataNameOrThrow(Compilation compilation,
            string fullyQualifiedMetadataName) =>
            compilation.GetTypeByMetadataName(fullyQualifiedMetadataName)
            ?? throw new MissingTypeException(fullyQualifiedMetadataName);

        IEnumerableType = GetTypeFromCompilationByMetadataNameOrThrow(compilation, IEnumerableMetadataName);
        IServiceProviderType = GetTypeFromCompilationByMetadataNameOrThrow(compilation, IServiceProviderMetadataName);
        IServiceScopeType = compilation.GetTypeByMetadataName(IServiceScopeMetadataName);
        IAsyncDisposableType = compilation.GetTypeByMetadataName(IAsyncDisposableMetadataName);
        IServiceScopeFactoryType = compilation.GetTypeByMetadataName(IServiceScopeFactoryMetadataName);
        IServiceProviderIsServiceType = compilation.GetTypeByMetadataName(IServiceProviderIsServiceMetadataName);

        CompositionRootAttributeType =
            GetTypeByMetadataNameOrThrow(assemblySymbol, CompositionRootAttributeMetadataName);

        TransientAttributeType = GetTypeByMetadataNameOrThrow(assemblySymbol, TransientAttributeMetadataName);
        GenericTransientAttributeType = assemblySymbol.GetTypeByMetadataName(GenericTransientAttributeMetadataName);
        Generic2TransientAttributeType = assemblySymbol.GetTypeByMetadataName(Generic2TransientAttributeMetadataName);

        SingletonAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, SingletonAttributeMetadataName);
        GenericSingletonAttribute = assemblySymbol.GetTypeByMetadataName(GenericSingletonAttributeMetadataName);
        Generic2SingletonAttribute = assemblySymbol.GetTypeByMetadataName(Generic2SingletonAttributeMetadataName);

        ScopedAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, ScopedAttributeMetadataName);
        GenericScopedAttribute = assemblySymbol.GetTypeByMetadataName(GenericScopedAttributeMetadataName);
        Generic2ScopedAttribute = assemblySymbol.GetTypeByMetadataName(Generic2ScopedAttributeMetadataName);

        ImportAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, ImportAttributeMetadataName);
        GenericImportAttribute = assemblySymbol.GetTypeByMetadataName(GenericImportAttributeMetadataName);

        ModuleAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, ServiceProviderModuleAttributeMetadataName);
    }
}

public class MissingTypeException : Exception
{
    public MissingTypeException(string fullyQualifiedMetadataName)
     : base($"Type with metadata '{fullyQualifiedMetadataName}' not found")
    {
        FullyQualifiedMetadataName = fullyQualifiedMetadataName;
    }

    public string FullyQualifiedMetadataName { get; }
}

internal class ServiceProviderBuilder
{
    private readonly GeneratorContext _context;
    private readonly KnownTypes _knownTypes;

    public ServiceProviderBuilder(GeneratorContext context)
    {
        _context = context;

        var assemblySymbol =
            context.Compilation.SourceModule.ReferencedAssemblySymbols.FirstOrDefault(
                s => s.Name == KnownTypes.JabAttributesAssemblyName)
            ?? context.Compilation.Assembly;
        _knownTypes = new KnownTypes(context.Compilation, assemblySymbol);
    }

    public ServiceProvider[] BuildRoots()
    {
        List<GetServiceCallCandidate> getServiceCallCandidates = new();

        foreach (var candidateGetServiceCallGroup in _context.CandidateGetServiceCalls.GroupBy(c => c.SyntaxTree))
        {
            var semanticModel = _context.Compilation.GetSemanticModel(candidateGetServiceCallGroup.Key);
            foreach (var candidateGetServiceCall in candidateGetServiceCallGroup)
            {
                if (candidateGetServiceCall.Expression is MemberAccessExpressionSyntax
                    {
                        Name: GenericNameSyntax
                        {
                            IsUnboundGenericName: false,
                            TypeArgumentList: { Arguments: { Count: 1 } arguments }
                        }
                    } memberAccessExpression)
                {
                    var containerTypeInfo = semanticModel.GetTypeInfo(memberAccessExpression.Expression);
                    var serviceInfo = semanticModel.GetSymbolInfo(arguments[0]);
                    if (containerTypeInfo.Type != null &&
                        serviceInfo.Symbol is ITypeSymbol serviceType &&
                        serviceType.TypeKind != TypeKind.TypeParameter
                       )
                    {
                        getServiceCallCandidates.Add(new GetServiceCallCandidate(containerTypeInfo.Type, serviceType,
                            candidateGetServiceCall.GetLocation()));
                    }
                }
            }
        }

        List<ServiceProvider> compositionRoots = new();
#pragma warning disable RS1024 // Compare symbols correctly
        HashSet<ITypeSymbol> processedTypes = new(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
        foreach (var candidateTypeDeclaration in _context.CandidateTypes)
        {
            var semanticModel = _context.Compilation.GetSemanticModel(candidateTypeDeclaration.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(candidateTypeDeclaration);

            if (symbol is ITypeSymbol typeSymbol &&
                processedTypes.Add(typeSymbol) &&
                TryCreateCompositionRoot(typeSymbol, getServiceCallCandidates, out var compositionRoot))
            {
                compositionRoots.Add(compositionRoot);
            }
        }

        return compositionRoots.ToArray();
    }

    private bool TryCreateCompositionRoot(ITypeSymbol typeSymbol,
        List<GetServiceCallCandidate> getServiceCallCandidates,
        [NotNullWhen(true)] out ServiceProvider? compositionRoot)
    {
        compositionRoot = null;

        var description = GetDescription(typeSymbol);
        if (description == null)
        {
            return false;
        }

        EmitTypeDiagnostics(typeSymbol);

        Dictionary<CallSiteCacheKey, ServiceCallSite> callSites = new();
        foreach (var registration in description.ServiceRegistrations)
        {
            if (registration.ServiceType.IsUnboundGenericType)
            {
                continue;
            }

            GetCallSite(registration.ServiceType,
                new ServiceResolutionContext(description, callSites, registration.ServiceType, registration.Location));
        }

        List<RootService> rootServices = new(description.RootServices);
        rootServices.Add(new(_knownTypes.IServiceProviderType, null));
        if (_knownTypes.IServiceScopeFactoryType != null)
        {
            rootServices.Add(new(_knownTypes.IServiceScopeFactoryType, null));
        }
        if (_knownTypes.IServiceProviderIsServiceType != null)
        {
            rootServices.Add(new(_knownTypes.IServiceProviderIsServiceType, null));
        }

        foreach (var rootService in rootServices)
        {
            var serviceType = rootService.Service;
            var callSite = GetCallSite(serviceType,
                new ServiceResolutionContext(description, callSites, serviceType, description.Location));
            if (callSite == null)
            {
                _context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.NoServiceTypeRegistered,
                    rootService.Location,
                    serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                ));
            }
        }

        foreach (var getServiceCallCandidate in getServiceCallCandidates)
        {
            if (SymbolEqualityComparer.Default.Equals(getServiceCallCandidate.ProviderType, typeSymbol))
            {
                var serviceType = getServiceCallCandidate.ServiceType;
                var callSite = GetCallSite(serviceType,
                    new ServiceResolutionContext(description, callSites, serviceType,
                        getServiceCallCandidate.Location));
                if (callSite == null)
                {
                    _context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.NoServiceTypeRegistered,
                        getServiceCallCandidate.Location,
                        serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                    ));
                }
            }
        }

        compositionRoot = new ServiceProvider(typeSymbol, callSites.Values.ToArray(), _knownTypes);
        return true;
    }

    private void EmitTypeDiagnostics(ITypeSymbol typeSymbol)
    {
        foreach (var declaringSyntaxReference in typeSymbol.DeclaringSyntaxReferences)
        {
            if (declaringSyntaxReference.GetSyntax() is ClassDeclarationSyntax typeDeclarationSyntax &&
                !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                _context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.ServiceProviderTypeHasToBePartial,
                    typeDeclarationSyntax.Identifier.GetLocation(),
                    typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                ));
            }
        }
    }

    private ServiceCallSite? GetCallSite(
        ITypeSymbol serviceType,
        ServiceResolutionContext context)
    {
        if (context.CallSiteCache.TryGetValue(new CallSiteCacheKey(serviceType), out var cachedCallSite))
        {
            return cachedCallSite;
        }

        if (!context.TryAdd(serviceType))
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.CyclicDependencyDetected,
                context.RequestLocation,
                context.RequestService.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                context.ToString(serviceType));

            _context.ReportDiagnostic(
                diagnostic);

            return new ErrorCallSite(serviceType, diagnostic);
        }

        try
        {
            return TryCreateSpecial(serviceType, context) ??
                   TryCreateExact(serviceType, context) ??
                   TryCreateEnumerable(serviceType, context) ??
                   TryCreateGeneric(serviceType, context);
        }
        catch
        {
            context.Remove(serviceType);
            throw;
        }
    }

    private ServiceCallSite? TryCreateSpecial(ITypeSymbol serviceType, ServiceResolutionContext context)
    {
        if (SymbolEqualityComparer.Default.Equals(serviceType, _knownTypes.IServiceProviderType))
        {
            var callSite = new ServiceProviderCallSite(serviceType);
            context.CallSiteCache[new CallSiteCacheKey(serviceType)] = callSite;
            return callSite;
        }

        if (SymbolEqualityComparer.Default.Equals(serviceType, _knownTypes.IServiceScopeFactoryType))
        {
            var callSite = new ScopeFactoryCallSite(serviceType);
            context.CallSiteCache[new CallSiteCacheKey(serviceType)] = callSite;
            return callSite;
        }

        if (SymbolEqualityComparer.Default.Equals(serviceType, _knownTypes.IServiceProviderIsServiceType))
        {
            var callSite = new ServiceProviderIsServiceCallSite(serviceType);
            context.CallSiteCache[new CallSiteCacheKey(serviceType)] = callSite;
            return callSite;
        }

        return null;
    }

    private ServiceCallSite? TryCreateGeneric(
        ITypeSymbol serviceType,
        ServiceResolutionContext context)
    {
        if (serviceType is INamedTypeSymbol { IsGenericType: true })
        {
            for (int i = context.ProviderDescription.ServiceRegistrations.Count - 1; i >= 0; i--)
            {
                var registration = context.ProviderDescription.ServiceRegistrations[i];

                var callSite = TryCreateGeneric(serviceType, registration, 0, context);
                if (callSite != null)
                {
                    return callSite;
                }
            }
        }

        return null;
    }

    private ServiceCallSite? TryCreateGeneric(
        ITypeSymbol serviceType,
        ServiceRegistration registration,
        int reverseIndex,
        ServiceResolutionContext context)
    {
        if (serviceType is INamedTypeSymbol { IsGenericType: true } genericType &&
            registration.ServiceType.IsUnboundGenericType &&
            SymbolEqualityComparer.Default.Equals(registration.ServiceType.ConstructedFrom,
                genericType.ConstructedFrom))
        {
            // TODO: This can use better error reporting
            if (registration.FactoryMember is IMethodSymbol factoryMethod)
            {
                var constructedFactoryMethod = factoryMethod.ConstructedFrom.Construct(genericType.TypeArguments,
                    genericType.TypeArgumentNullableAnnotations);
                var callSite = CreateFactoryCallSite(
                    genericType,
                    null,
                    registration.Lifetime,
                    registration.Location,
                    memberLocation: registration.MemberLocation,
                    factoryMember: constructedFactoryMethod,
                    reverseIndex: reverseIndex,
                    context: context);

                context.CallSiteCache[new CallSiteCacheKey(reverseIndex, serviceType)] = callSite;

                return callSite;
            }
            else if (registration.ImplementationType != null)
            {
                var implementationType =
                    registration.ImplementationType.ConstructedFrom.Construct(genericType.TypeArguments,
                        genericType.TypeArgumentNullableAnnotations);
                return CreateConstructorCallSite(registration, genericType, implementationType, reverseIndex, context);
            }
            else
            {
                throw new InvalidOperationException($"Can't construct generic callsite for {serviceType}");
            }
        }

        return null;
    }

    private ServiceCallSite? TryCreateEnumerable(ITypeSymbol serviceType, ServiceResolutionContext context)
    {
        static ServiceLifetime GetCommonLifetime(IEnumerable<ServiceCallSite> callSites)
        {
            var commonLifetime = ServiceLifetime.Singleton;

            foreach (var serviceCallSite in callSites)
            {
                if (serviceCallSite.Lifetime < commonLifetime)
                {
                    commonLifetime = serviceCallSite.Lifetime;
                }
            }

            return commonLifetime;
        }

        if (serviceType is INamedTypeSymbol { IsGenericType: true } genericType &&
            SymbolEqualityComparer.Default.Equals(genericType.ConstructedFrom, _knownTypes.IEnumerableType))
        {
            var enumerableService = genericType.TypeArguments[0];
            var items = new List<ServiceCallSite>();
            int reverseIndex = 0;
            for (int i = context.ProviderDescription.ServiceRegistrations.Count - 1; i >= 0; i--)
            {
                var registration = context.ProviderDescription.ServiceRegistrations[i];

                var itemCallSite = TryCreateGeneric(enumerableService, registration, reverseIndex, context) ??
                                   TryCreateExact(registration, enumerableService, reverseIndex, context);
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
                serviceCallSites,
                // Pick a most common lifetime
                GetCommonLifetime(items));

            context.CallSiteCache[new CallSiteCacheKey(reverseIndex, serviceType)] = callSite;

            return callSite;
        }

        return null;
    }

    private ServiceCallSite? TryCreateExact(ITypeSymbol serviceType, ServiceResolutionContext context)
    {
        if (context.ProviderDescription.ServiceRegistrationsLookup.TryGetValue(serviceType, out var registration))
        {
            return CreateCallSite(registration, reverseIndex: 0, context: context);
        }

        return null;
    }

    private ServiceCallSite? TryCreateExact(ServiceRegistration registration, ITypeSymbol serviceType, int reverseIndex,
        ServiceResolutionContext context)
    {
        if (SymbolEqualityComparer.Default.Equals(registration.ServiceType, serviceType))
        {
            return CreateCallSite(registration, reverseIndex: reverseIndex, context: context);
        }

        return null;
    }

    private ServiceCallSite CreateCallSite(
        ServiceRegistration registration,
        int reverseIndex,
        ServiceResolutionContext context)
    {
        var cacheKey = new CallSiteCacheKey(reverseIndex, registration.ServiceType);

        if (context.CallSiteCache.TryGetValue(cacheKey, out ServiceCallSite callSite))
        {
            return callSite;
        }

        if (registration.InstanceMember is { } instanceMember)
        {
            callSite = CreateMemberCallSite(
                registration,
                instanceMember,
                registration.MemberLocation,
                reverseIndex);
        }
        else if (registration.FactoryMember is { } factoryMember)
        {
            callSite = CreateFactoryCallSite(
                registration.ServiceType,
                registration.ImplementationType,
                registration.Lifetime,
                registration.Location,
                registration.MemberLocation,
                factoryMember,
                reverseIndex,
                context);
        }
        else
        {
            var implementationType = registration.ImplementationType ??
                                     registration.ServiceType;

            callSite = CreateConstructorCallSite(registration, registration.ServiceType, implementationType,
                reverseIndex, context);
        }

        context.CallSiteCache[cacheKey] = callSite;

        return callSite;
    }

    private ServiceCallSite CreateMemberCallSite(
        ServiceRegistration registration,
        ISymbol instanceMember,
        MemberLocation memberLocation,
        int reverseIndex)
    {
        return new MemberCallSite(registration.ServiceType,
            instanceMember,
            memberLocation: memberLocation,
            registration.Lifetime,
            reverseIndex,
            false);
    }

    private ServiceCallSite CreateFactoryCallSite(INamedTypeSymbol serviceType,
        INamedTypeSymbol? implementationType,
        ServiceLifetime lifetime,
        Location? registrationLocation,
        MemberLocation memberLocation,
        ISymbol factoryMember,
        int reverseIndex,
        ServiceResolutionContext context)
    {
        ImmutableArray<IParameterSymbol> GetDelegateParameters(ITypeSymbol type)
        {
            foreach (var member in type.GetMembers("Invoke"))
            {
                if (member is IMethodSymbol method)
                {
                    return method.Parameters;
                }
            }

            throw new InvalidOperationException($"Unable to determine parameters for {type.ToDisplayString()}");
        }

        var cacheKey = new CallSiteCacheKey(reverseIndex, serviceType);

        if (context.CallSiteCache.TryGetValue(cacheKey, out ServiceCallSite callSite))
        {
            return callSite;
        }

        implementationType ??= serviceType;

        ImmutableArray<IParameterSymbol> factoryParameters;
        switch (factoryMember)
        {
            case IMethodSymbol method:
                factoryParameters = method.Parameters;
                break;
            case IFieldSymbol { Type: { TypeKind: TypeKind.Delegate } type }:
                factoryParameters = GetDelegateParameters(type);
                break;
            case IPropertySymbol { Type: { TypeKind: TypeKind.Delegate } type }:
                factoryParameters = GetDelegateParameters(type);
                break;
            default:
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.FactoryMemberMustBeAMethodOrHaveDelegateType,
                    ExtractMemberTypeLocation(factoryMember),
                    factoryMember.Name,
                    serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
                _context.ReportDiagnostic(diagnostic);
                return new ErrorCallSite(serviceType, diagnostic);
        }

        var (parameters, namedParameters, diagnostics) =
            GetParameters(factoryParameters, registrationLocation, implementationType, context);

        if (diagnostics.Count > 0)
        {
            return new ErrorCallSite(serviceType, diagnostics.ToArray());
        }

        return new FactoryCallSite(serviceType,
            factoryMember,
            memberLocation: memberLocation,
            parameters.ToArray(),
            namedParameters.ToArray(),
            lifetime,
            reverseIndex,
            false);
    }

    private ServiceCallSite CreateConstructorCallSite(
        ServiceRegistration registration,
        INamedTypeSymbol serviceType,
        INamedTypeSymbol implementationType,
        int reverseIndex,
        ServiceResolutionContext context)
    {
        var cacheKey = new CallSiteCacheKey(reverseIndex, serviceType);

        if (context.CallSiteCache.TryGetValue(cacheKey, out ServiceCallSite callSite))
        {
            return callSite;
        }

        context.TryAdd(implementationType);
        try
        {
            var ctor = SelectConstructor(implementationType, context.ProviderDescription);
            if (ctor == null)
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ImplementationTypeRequiresPublicConstructor,
                    registration.Location,
                    implementationType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

                _context.ReportDiagnostic(
                    diagnostic);

                return new ErrorCallSite(serviceType, diagnostic);
            }

            var (parameters, namedParameters, diagnostics) =
                GetParameters(ctor.Parameters, registration.Location, implementationType, context);

            if (diagnostics.Count > 0)
            {
                return new ErrorCallSite(serviceType, diagnostics.ToArray());
            }

            callSite = new ConstructorCallSite(
                serviceType,
                implementationType,
                parameters.ToArray(),
                namedParameters.ToArray(),
                registration.Lifetime,
                reverseIndex,
                // TODO: this can be optimized to avoid check for all the types
                isDisposable: null
            );

            context.CallSiteCache[cacheKey] = callSite;

            return callSite;
        }
        catch
        {
            context.Remove(implementationType);
            throw;
        }
    }

    private (
        List<ServiceCallSite> Parameters,
        List<KeyValuePair<IParameterSymbol, ServiceCallSite>> NamedParameters,
        List<Diagnostic> Diagnostics) GetParameters(
            ImmutableArray<IParameterSymbol> parameters,
            Location? registrationLocation,
            INamedTypeSymbol implementationType,
            ServiceResolutionContext context)
    {
        var callSites = new List<ServiceCallSite>();
        var namedParameters = new List<KeyValuePair<IParameterSymbol, ServiceCallSite>>();
        var diagnostics = new List<Diagnostic>();
        foreach (var parameterSymbol in parameters)
        {
            var parameterCallSite = GetCallSite(parameterSymbol.Type, context);
            if (parameterSymbol.IsOptional)
            {
                if (parameterCallSite != null)
                {
                    namedParameters.Add(
                        new KeyValuePair<IParameterSymbol, ServiceCallSite>(parameterSymbol, parameterCallSite));
                }
            }
            else
            {
                bool isNullable = parameterSymbol.Type.NullableAnnotation == NullableAnnotation.Annotated;
                if (parameterCallSite == null)
                {
                    var diagnostic = Diagnostic.Create(
                        isNullable ? DiagnosticDescriptors.NullableServiceNotRegistered : DiagnosticDescriptors.ServiceRequiredToConstructNotRegistered,
                        registrationLocation,
                        parameterSymbol.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                        implementationType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

                    diagnostics.Add(diagnostic);
                    _context.ReportDiagnostic(diagnostic);
                }
                else
                {
                    if (isNullable)
                    {
                        var diagnostic = Diagnostic.Create(
                                DiagnosticDescriptors.NullableServiceRegistered,
                                registrationLocation,
                                parameterSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                                implementationType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

                        _context.ReportDiagnostic(diagnostic);
                    }

                    callSites.Add(parameterCallSite);
                }
            }
        }

        return (callSites, namedParameters, diagnostics);
    }

    private bool CanSatisfy(ITypeSymbol serviceType, ServiceProviderDescription description)
    {
        INamedTypeSymbol? genericType = null;

        if (serviceType is INamedTypeSymbol { IsGenericType: true } genericServiceType)
        {
            genericType = genericServiceType;
        }

        if (genericType != null &&
            SymbolEqualityComparer.Default.Equals(genericType.ConstructedFrom, _knownTypes.IEnumerableType))
        {
            // We can always satisfy IEnumerables
            return true;
        }

        foreach (var registration in description.ServiceRegistrations)
        {
            if (SymbolEqualityComparer.Default.Equals(registration.ServiceType.ConstructedFrom, serviceType))
            {
                return true;
            }

            if (genericType != null &&
                registration.ServiceType.IsUnboundGenericType &&
                SymbolEqualityComparer.Default.Equals(registration.ServiceType.ConstructedFrom,
                    genericType.ConstructedFrom))
            {
                return true;
            }
        }

        return false;
    }

    private IMethodSymbol? SelectConstructor(INamedTypeSymbol implementationType,
        ServiceProviderDescription description)
    {
        IMethodSymbol? selectedCtor = null;
        IMethodSymbol? candidate = null;
        foreach (var constructor in implementationType.Constructors)
        {
            if (constructor.DeclaredAccessibility == Accessibility.Public)
            {
                // Pick a shortest candidate just in case we don't find
                // any applicable ctor and need to produce diagnostics
                if (candidate == null ||
                    candidate.Parameters.Length > constructor.Parameters.Length)
                {
                    candidate = constructor;
                }

                if (constructor.Parameters.Length > (selectedCtor?.Parameters.Length ?? -1))
                {
                    bool allSatisfied = true;
                    foreach (var constructorParameter in constructor.Parameters)
                    {
                        if (!CanSatisfy(constructorParameter.Type, description) &&
                            !constructorParameter.IsOptional)
                        {
                            allSatisfied = false;
                            break;
                        }
                    }

                    if (allSatisfied)
                    {
                        selectedCtor = constructor;
                    }
                }
            }
        }

        // Return a candidate so we can produce diagnostics for required services in a simple case
        return selectedCtor ?? candidate;
    }

    private ServiceProviderDescription? GetDescription(ITypeSymbol serviceProviderType)
    {
        bool isCompositionRoot = false;
        bool isModule = false;
        Location? location = null;
        List<ServiceRegistration> registrations = new();
        List<RootService> rootServices = new();
        foreach (var attributeData in serviceProviderType.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass,
                    _knownTypes.CompositionRootAttributeType))
            {
                location = attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation();
                isCompositionRoot = true;
                foreach (var namedArgument in attributeData.NamedArguments)
                {
                    if (namedArgument.Key == KnownTypes.RootServicesAttributePropertyName)
                    {
                        foreach (var typedConstant in namedArgument.Value.Values)
                        {
                            rootServices.Add(new(ExtractType(typedConstant),
                                attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation()));
                        }
                    }
                }
            }
            else if (TryGetModuleImport(attributeData, _knownTypes, out var innerModuleType))
            {
                ProcessModule(serviceProviderType, registrations, innerModuleType, attributeData);
            }
            else if (TryCreateRegistration(serviceProviderType, null, attributeData, _knownTypes, out var registration))
            {
                registrations.Add(registration);
            }
            else if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, _knownTypes.ModuleAttribute))
            {
                isModule = true;
            }
        }

        if (isCompositionRoot)
        {
            return new ServiceProviderDescription(registrations, rootServices.ToArray(), location);
        }

        if (registrations.Count > 0 && !isModule)
        {
            _context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingServiceProviderAttribute,
                ExtractSymbolNameLocation(serviceProviderType),
                serviceProviderType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
            ));
        }

        return null;
    }

    private void ProcessModule(ITypeSymbol serviceProviderType, List<ServiceRegistration> registrations,
        INamedTypeSymbol moduleType, AttributeData importAttributeData)
    {
        // TODO: idempotency
        bool isModule = false;
        // If module it in another assembly use KnownTypes native to that assembly
        KnownTypes knownTypes;
        if (SymbolEqualityComparer.Default.Equals(moduleType.ContainingAssembly, _context.Compilation.Assembly))
        {
            knownTypes = _knownTypes;
        }
        else
        {
            var assemblySymbol = moduleType.ContainingModule.ReferencedAssemblySymbols.FirstOrDefault(
                s => s.Name == KnownTypes.JabAttributesAssemblyName)
                ?? moduleType.ContainingAssembly;
            knownTypes = new KnownTypes(_context.Compilation, assemblySymbol);
        }

        foreach (var attributeData in moduleType.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, knownTypes.ModuleAttribute))
            {
                isModule = true;
            }
            else if (TryGetModuleImport(attributeData, knownTypes, out var innerModuleType))
            {
                ProcessModule(serviceProviderType, registrations, innerModuleType, importAttributeData);
            }
            else if (TryCreateRegistration(serviceProviderType, moduleType, attributeData, knownTypes, out var registration))
            {
                registrations.Add(registration);
            }
        }

        if (!isModule)
        {
            _context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.ImportedTypeNotMarkedWithModuleAttribute,
                importAttributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                moduleType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                knownTypes.ModuleAttribute.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
            ));
        }
    }

    private bool TryGetModuleImport(AttributeData attributeData, KnownTypes knownTypes,
        [NotNullWhen(true)] out INamedTypeSymbol? moduleType)
    {
        if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, knownTypes.ImportAttribute) ||
            SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass?.ConstructedFrom,
                knownTypes.GenericImportAttribute))
        {
            if (attributeData.AttributeClass is { IsGenericType: true })
            {
                moduleType = (INamedTypeSymbol)attributeData.AttributeClass.TypeArguments[0];
            }
            else
            {
                moduleType = ExtractType(attributeData.ConstructorArguments[0]);
            }

            return true;
        }

        moduleType = null;
        return false;
    }

    private bool TryCreateRegistration(ITypeSymbol serviceProviderType,
        ITypeSymbol? moduleType,
        AttributeData attributeData,
        KnownTypes knownTypes, [NotNullWhen(true)] out ServiceRegistration? registration)
    {
        registration = null;

        if (attributeData.AttributeClass == null)
        {
            return false;
        }

        if ((SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, knownTypes.TransientAttributeType) ||
             SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass.ConstructedFrom,
                 knownTypes.GenericTransientAttributeType) ||
             SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass.ConstructedFrom,
                 knownTypes.Generic2TransientAttributeType)) &&
            TryCreateRegistration(serviceProviderType, moduleType, attributeData, ServiceLifetime.Transient, out registration))
        {
            return true;
        }

        if ((SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, knownTypes.SingletonAttribute) ||
             SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass.ConstructedFrom,
                 knownTypes.GenericSingletonAttribute) ||
             SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass.ConstructedFrom,
                 knownTypes.Generic2SingletonAttribute)) &&
            TryCreateRegistration(serviceProviderType, moduleType, attributeData, ServiceLifetime.Singleton, out registration))
        {
            return true;
        }

        if ((SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, knownTypes.ScopedAttribute) ||
             SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass.ConstructedFrom,
                 knownTypes.GenericScopedAttribute) ||
             SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass.ConstructedFrom,
                 knownTypes.Generic2ScopedAttribute)) &&
            TryCreateRegistration(serviceProviderType, moduleType, attributeData, ServiceLifetime.Scoped, out registration))
        {
            return true;
        }

        return false;
    }

    private bool TryCreateRegistration(
        ITypeSymbol serviceProviderType,
        ITypeSymbol? moduleType,
        AttributeData attributeData,
        ServiceLifetime serviceLifetime,
        [NotNullWhen(true)] out ServiceRegistration? registration)
    {
        registration = null;

        string? instanceMemberName = null;
        string? factoryMemberName = null;
        foreach (var namedArgument in attributeData.NamedArguments)
        {
            if (namedArgument.Key == KnownTypes.InstanceAttributePropertyName)
            {
                instanceMemberName = (string?)namedArgument.Value.Value;
            }
            else if (namedArgument.Key == KnownTypes.FactoryAttributePropertyName)
            {
                factoryMemberName = (string?)namedArgument.Value.Value;
            }
        }

        INamedTypeSymbol serviceType;
        INamedTypeSymbol? implementationType;

        if (attributeData.AttributeClass is { IsGenericType: true } attributeClass)
        {
            serviceType = (INamedTypeSymbol)attributeClass.TypeArguments[0];
            implementationType = attributeClass.TypeArguments.Length == 2
                ? (INamedTypeSymbol)attributeClass.TypeArguments[1]
                : null;
        }
        else
        {
            serviceType = ExtractType(attributeData.ConstructorArguments[0]);
            implementationType = attributeData.ConstructorArguments.Length == 2
                ? ExtractType(attributeData.ConstructorArguments[1])
                : null;
        }

        if (implementationType != null &&
            (instanceMemberName != null || factoryMemberName != null))
        {
            _context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.ImplementationTypeAndFactoryNotAllowed,
                attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
        }

        ISymbol? instanceMember = null;
        MemberLocation memberLocation = default;
        if (instanceMemberName != null &&
            !TryFindMember(serviceProviderType,
                moduleType,
                attributeData,
                instanceMemberName,
                KnownTypes.InstanceAttributePropertyName,
                includeScope: false,
                out instanceMember,
                out memberLocation))
        {
            return false;
        }

        ISymbol? factoryMember = null;
        if (factoryMemberName != null &&
            !TryFindMember(serviceProviderType,
                moduleType,
                attributeData,
                factoryMemberName,
                KnownTypes.FactoryAttributePropertyName,
                includeScope: serviceLifetime == ServiceLifetime.Scoped,
                out factoryMember,
                out memberLocation))
        {
            return false;
        }

        registration = new ServiceRegistration(
            serviceLifetime,
            serviceType,
            implementationType,
            instanceMember,
            factoryMember,
            attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
            memberLocation);

        return true;
    }

    private bool TryFindMember(ITypeSymbol typeSymbol,
        ITypeSymbol? moduleType,
        AttributeData attributeData,
        string memberName,
        string parameterName,
        bool includeScope,
        [NotNullWhen(true)] out ISymbol? member,
        out MemberLocation memberLocation)
    {
        member = null;
        memberLocation = MemberLocation.Root;

        List<ISymbol> members = new();

        // Prefer module members
        if (moduleType != null)
        {
            members.AddRange(moduleType.GetMembers(memberName));
        }
        
        if (members.Count == 0)
        {
            members.AddRange(typeSymbol.GetMembers(memberName));
            if (includeScope)
            {
                foreach (var scopeType in typeSymbol.GetTypeMembers("Scope"))
                {
                    members.AddRange(scopeType.GetMembers(memberName));
                }
            }
        }

        if (members.Count == 0)
        {
            _context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.MemberReferencedByInstanceOrFactoryAttributeNotFound,
                attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                memberName,
                parameterName
            ));
            return false;
        }

        if (members.Count > 1)
        {
            _context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.MemberReferencedByInstanceOrFactoryAttributeAmbiguous,
                attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                memberName,
                parameterName
            ));
            return false;
        }

        member = members[0];
        if (SymbolEqualityComparer.Default.Equals(member.ContainingType, typeSymbol))
        {
            memberLocation = MemberLocation.Scope;
        }
        else if (SymbolEqualityComparer.Default.Equals(member.ContainingType, moduleType))
        {
            memberLocation = MemberLocation.Module;
        }
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

        return (INamedTypeSymbol)typedConstant.Value;
    }

    private Location? ExtractMemberTypeLocation(ISymbol symbol)
    {
        foreach (var declaringSyntaxReference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = declaringSyntaxReference.GetSyntax();
            return syntax switch
            {
                PropertyDeclarationSyntax declarationSyntax => declarationSyntax.Type.GetLocation(),
                FieldDeclarationSyntax fieldDeclarationSyntax => fieldDeclarationSyntax.Declaration.Type.GetLocation(),
                VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Type: {} type }} => type.GetLocation(),
                _ => syntax.GetLocation()
            };
        }

        return null;
    }

    private Location? ExtractSymbolNameLocation(ISymbol symbol)
    {
        foreach (var declaringSyntaxReference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = declaringSyntaxReference.GetSyntax();
            if (syntax is TypeDeclarationSyntax declarationSyntax)
            {
                return declarationSyntax.Identifier.GetLocation();
            }

            return syntax.GetLocation();
        }

        return null;
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

    private class ServiceResolutionContext
    {
        private readonly HashSet<ServiceChainItem> _chain = new();
        private int _index;

        public Dictionary<CallSiteCacheKey, ServiceCallSite> CallSiteCache { get; }
        public ITypeSymbol RequestService { get; }
        public ServiceProviderDescription ProviderDescription { get; }
        public Location? RequestLocation { get; }

        public ServiceResolutionContext(
            ServiceProviderDescription providerDescription,
            Dictionary<CallSiteCacheKey, ServiceCallSite> serviceCallSites,
            ITypeSymbol requestService,
            Location? requestLocation)
        {
            CallSiteCache = serviceCallSites;
            RequestService = requestService;
            ProviderDescription = providerDescription;
            RequestLocation = requestLocation;
        }

        public bool TryAdd(ITypeSymbol typeSymbol)
        {
            var serviceChainItem = new ServiceChainItem(typeSymbol, _index);
            if (_chain.Contains(serviceChainItem))
            {
                return false;
            }

            _index++;
            _chain.Add(serviceChainItem);
            return true;
        }

        public void Remove(ITypeSymbol typeSymbol)
        {
            _chain.Remove(new ServiceChainItem(typeSymbol, 0));
            _index--;
        }

        public string ToString(ITypeSymbol typeSymbol)
        {
            StringBuilder builder = new();
            foreach (var serviceChainItem in _chain.OrderBy(c => c.Index))
            {
                builder.Append(
                    serviceChainItem.TypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
                builder.Append(" -> ");
            }

            builder.Append(typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
            return builder.ToString();
        }

        private struct ServiceChainItem : IEquatable<ServiceChainItem>
        {
            public ITypeSymbol TypeSymbol { get; }
            public int Index { get; }

            public ServiceChainItem(ITypeSymbol typeSymbol, int index)
            {
                TypeSymbol = typeSymbol;
                Index = index;
            }

            public bool Equals(ServiceChainItem other)
            {
                return SymbolEqualityComparer.Default.Equals(TypeSymbol, other.TypeSymbol);
            }

            public override bool Equals(object? obj)
            {
                return obj is ServiceChainItem other && Equals(other);
            }

            public override int GetHashCode()
            {
                return SymbolEqualityComparer.Default.GetHashCode(TypeSymbol);
            }
        }
    }
}