namespace Jab;

internal class ServiceProviderBuilder
{
    private readonly GeneratorContext _context;
    private readonly KnownTypes _knownTypes;
    private readonly ServiceProviderCallSite _serviceProviderCallsite;
    private readonly ScopeFactoryCallSite? _scopeFactoryCallSite;
    private readonly ServiceProviderIsServiceCallSite? _serviceProviderIsServiceCallSite;

    public ServiceProviderBuilder(GeneratorContext context)
    {
        _context = context;
        _knownTypes = new KnownTypes(context.Compilation, context.Compilation.SourceModule, context.Compilation.Assembly);
        _serviceProviderCallsite = new ServiceProviderCallSite(_knownTypes.IServiceProviderType);
        if (_knownTypes.IServiceScopeFactoryType != null)
        {
            _scopeFactoryCallSite = new ScopeFactoryCallSite(_knownTypes.IServiceScopeFactoryType);
        }
        if (_knownTypes.IServiceProviderIsServiceType != null)
        {
            _serviceProviderIsServiceCallSite = new ServiceProviderIsServiceCallSite(_knownTypes.IServiceProviderIsServiceType);
        }
    }

    public ServiceProvider[] BuildRoots()
    {
        List<GetServiceCallCandidate> getServiceCallCandidates = new();

        foreach (var candidateGetServiceCallGroup in _context.CandidateGetServiceCalls.GroupBy(c => c.SyntaxTree))
        {
            var semanticModel = _context.Compilation.GetSemanticModel(candidateGetServiceCallGroup.Key);
            foreach (var candidateGetServiceCall in candidateGetServiceCallGroup)
            {
                if (candidateGetServiceCall is
                    {
                        Expression: MemberAccessExpressionSyntax
                        {
                            Name: GenericNameSyntax
                            {
                                IsUnboundGenericName: false,
                                TypeArgumentList: { Arguments: { Count: 1 } arguments }
                            }
                        } memberAccessExpression
                    }
                  )
                {
                    var containerTypeInfo = semanticModel.GetTypeInfo(memberAccessExpression.Expression);
                    var serviceInfo = semanticModel.GetSymbolInfo(arguments[0]);
                    if (containerTypeInfo.Type != null &&
                        serviceInfo.Symbol is ITypeSymbol serviceType &&
                        serviceType.TypeKind != TypeKind.TypeParameter
                       )
                    {
                        string? serviceName = null;
                        var invocationArguments = candidateGetServiceCall.ArgumentList.Arguments;
                        if (invocationArguments.Count == 1)
                        {
                            if (invocationArguments[0].Expression is LiteralExpressionSyntax { } literal &&
                                literal.Token.IsKind(SyntaxKind.StringLiteralToken))
                            {
                                serviceName = literal.Token.ValueText;
                            }
                            else
                            {
                                // Service name is dynamic, can't do anything
                                continue;
                            }
                        }
                        getServiceCallCandidates.Add(new GetServiceCallCandidate(
                            containerTypeInfo.Type,
                            serviceType,
                            serviceName,
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

        CallSiteCache callSites = new();
        foreach (var registration in description.ServiceRegistrations)
        {
            if (registration.ServiceType.IsUnboundGenericType)
            {
                continue;
            }

            GetCallSite(
                registration.ServiceType,
                registration.Name,
                new ServiceResolutionContext(description, callSites, registration.Location));
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
            var callSite = GetCallSite(
                serviceType,
                null,
                new ServiceResolutionContext(description, callSites, description.Location));
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
                var callSite = GetCallSite(
                    serviceType,
                    getServiceCallCandidate.ServiceName,
                    new ServiceResolutionContext(description, callSites, getServiceCallCandidate.Location));
                if (callSite == null)
                {
                    if (getServiceCallCandidate.ServiceName == null)
                    {
                        _context.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.NoServiceTypeRegistered,
                            getServiceCallCandidate.Location,
                            serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                        ));
                    }
                    else
                    {
                        _context.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.NoServiceTypeAndNameRegistered,
                            getServiceCallCandidate.Location,
                            serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                            getServiceCallCandidate.ServiceName
                        ));
                    }
                }
            }
        }

        compositionRoot = new ServiceProvider(typeSymbol, callSites.GetRootCallSites(), _knownTypes);
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
        string? name,
        ServiceResolutionContext context)
    {
        if (!context.TryAdd(serviceType))
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.CyclicDependencyDetected,
                context.RequestLocation,
                serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                context.ToString(serviceType));

            _context.ReportDiagnostic(
                diagnostic);

            return new ErrorCallSite(new ServiceIdentity(serviceType, name, null), diagnostic);
        }

        try
        {
            return TryCreateSpecial(serviceType, name, context) ??
                   TryCreateExact(serviceType, name, null, context) ??
                   TryCreateEnumerable(serviceType, name, context) ??
                   TryCreateFunc(serviceType, name, context) ??
                   TryCreateGeneric(serviceType, name, context);
        }
        finally
        {
            context.Remove(serviceType);
        }
    }

    private ServiceCallSite? TryCreateSpecial(ITypeSymbol serviceType, string? name, ServiceResolutionContext context)
    {
        ErrorCallSite? CheckNotNamed(ServiceIdentity identity)
        {
            if (name == null) return null;

            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.BuiltInServicesAreNotNamed,
                context.RequestLocation,
                serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

            _context.ReportDiagnostic(
                diagnostic);

            return new ErrorCallSite(identity);
        }

        ServiceCallSite BuiltInCallSite(ServiceCallSite callSite)
        {
            if (CheckNotNamed(callSite.Identity) is { } error)
            {
                return error;
            }
            if (!context.CallSiteCache.TryGet(callSite.Identity, out _))
            {
                context.CallSiteCache.Add(callSite);
            }
            return callSite;
        }

        if (SymbolEqualityComparer.Default.Equals(serviceType, _knownTypes.IServiceProviderType))
        {
            return BuiltInCallSite(_serviceProviderCallsite);
        }

        if (SymbolEqualityComparer.Default.Equals(serviceType, _knownTypes.IServiceScopeFactoryType))
        {
            return BuiltInCallSite(_scopeFactoryCallSite!);
        }

        if (SymbolEqualityComparer.Default.Equals(serviceType, _knownTypes.IServiceProviderIsServiceType))
        {
            return BuiltInCallSite(_serviceProviderIsServiceCallSite!);
        }

        return null;
    }

    private ServiceCallSite? TryCreateGeneric(
        ITypeSymbol serviceType,
        string? name,
        ServiceResolutionContext context)
    {
        if (serviceType is INamedTypeSymbol { IsGenericType: true })
        {
            for (int i = context.ProviderDescription.ServiceRegistrations.Count - 1; i >= 0; i--)
            {
                var registration = context.ProviderDescription.ServiceRegistrations[i];

                var callSite = TryMatchGeneric(serviceType, name, null, registration, context);
                if (callSite != null)
                {
                    return callSite;
                }
            }
        }

        return null;
    }

    private ServiceCallSite? TryMatchGeneric(
        ITypeSymbol serviceType,
        string? name,
        int? reverseIndex,
        ServiceRegistration registration,
        ServiceResolutionContext context)
    {
        if (registration.Name == name &&
            serviceType is INamedTypeSymbol { IsGenericType: true } genericType &&
            registration.ServiceType.IsUnboundGenericType &&
            SymbolEqualityComparer.Default.Equals(registration.ServiceType.ConstructedFrom,
                genericType.ConstructedFrom))
        {
            var identity = new ServiceIdentity(serviceType, name, reverseIndex);
            if (context.CallSiteCache.TryGet(identity, out var callSite))
            {
                return callSite;
            }

            // TODO: This can use better error reporting
            if (registration.FactoryMember is IMethodSymbol factoryMethod)
            {
                var constructedFactoryMethod = factoryMethod.ConstructedFrom.Construct(genericType.TypeArguments,
                    genericType.TypeArgumentNullableAnnotations);
                callSite = CreateFactoryCallSite(
                    identity,
                    genericType,
                    registration.Lifetime,
                    registration.Location,
                    memberLocation: registration.MemberLocation,
                    factoryMember: constructedFactoryMethod,
                    context: context);
            }
            else if (registration.ImplementationType != null)
            {
                var implementationType =
                    registration.ImplementationType.ConstructedFrom.Construct(genericType.TypeArguments,
                        genericType.TypeArgumentNullableAnnotations);

                callSite = CreateConstructorCallSite(identity, registration, implementationType, context);
            }
            else
            {
                throw new InvalidOperationException($"Can't construct generic callsite for {serviceType}");
            }

            context.CallSiteCache.Add(callSite);

            return callSite;
        }

        return null;
    }

    private ServiceCallSite? TryCreateEnumerable(ITypeSymbol serviceType, string? name, ServiceResolutionContext context)
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
            var identity = new ServiceIdentity(genericType, null, null);

            if (name != null)
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ImplicitIEnumerableNotNamed,
                    context.RequestLocation,
                    serviceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

                _context.ReportDiagnostic(
                    diagnostic);
                return new ErrorCallSite(identity, diagnostic);
            }

            if (context.CallSiteCache.TryGet(identity, out var callSite))
            {
                return callSite;
            }

            var enumerableService = genericType.TypeArguments[0];
            var items = new List<ServiceCallSite>();
            int reverseIndex = 0;
            for (int i = context.ProviderDescription.ServiceRegistrations.Count - 1; i >= 0; i--)
            {
                var registration = context.ProviderDescription.ServiceRegistrations[i];

                var itemCallSite = TryMatchGeneric(enumerableService, null, reverseIndex, registration, context) ??
                                   TryMatchExact(enumerableService, null, reverseIndex, registration, context);
                if (itemCallSite != null)
                {
                    reverseIndex++;
                    items.Add(itemCallSite);
                }
            }

            var serviceCallSites = items.ToArray();
            Array.Reverse(serviceCallSites);
            Debug.Assert(name == null);
            callSite = new ArrayServiceCallSite(
                identity,
                genericType,
                enumerableService,
                serviceCallSites,
                // Pick a most common lifetime
                GetCommonLifetime(items));

            context.CallSiteCache.Add(callSite);

            return callSite;
        }

        return null;
    }

    
    private ServiceCallSite? TryCreateFunc(ITypeSymbol serviceType, string? name, ServiceResolutionContext context)
    {
        if (serviceType is INamedTypeSymbol { IsGenericType: true } genericType &&
            SymbolEqualityComparer.Default.Equals(genericType.ConstructedFrom, _knownTypes.FuncType))
        {
            var identity = new ServiceIdentity(genericType, name, null);

            if (context.CallSiteCache.TryGet(identity, out var callSite))
            {
                return callSite;
            }

            var innerType = genericType.TypeArguments[0];
            var inner = GetCallSite(innerType, name, context);

            if (inner == null)
            {
                return null;
            }
            
            callSite = new FuncCallSite(identity, inner);

            context.CallSiteCache.Add(callSite);

            return callSite;
        }

        return null;
    }

    private ServiceCallSite? TryCreateExact(
        ITypeSymbol serviceType,
        string? name,
        int? reverseIndex,
        ServiceResolutionContext context)
    {
        if (!context.ProviderDescription.ServiceRegistrationsLookup.TryGetValue(serviceType, out var registrations))
        {
            return null;
        }

        for (var index = registrations.Count - 1; index >= 0; index--)
        {
            var callSite = TryMatchExact(serviceType, name, reverseIndex, registrations[index], context: context);
            if (callSite != null)
            {
                return callSite;
            }
        }

        return null;
    }

    private ServiceCallSite? TryMatchExact(
        ITypeSymbol serviceType,
        string? name,
        int? reverseIndex,
        ServiceRegistration registration,
        ServiceResolutionContext context)
    {
        if (
            registration.Name == name &&
            SymbolEqualityComparer.Default.Equals(registration.ServiceType, serviceType))
        {
            var identity = new ServiceIdentity(registration.ServiceType, registration.Name, reverseIndex);
            if (context.CallSiteCache.TryGet(identity, out ServiceCallSite callSite))
            {
                return callSite;
            }

            if (registration.InstanceMember is { } instanceMember)
            {
                callSite = new MemberCallSite(identity,
                    instanceMember,
                    memberLocation: registration.MemberLocation,
                    registration.Lifetime,
                    false);
            }
            else if (registration.FactoryMember is { } factoryMember)
            {
                callSite = CreateFactoryCallSite(
                    identity,
                    registration.ImplementationType,
                    registration.Lifetime,
                    registration.Location,
                    registration.MemberLocation,
                    factoryMember,
                    context);
            }
            else
            {
                var implementationType = registration.ImplementationType ??
                                         registration.ServiceType;

                callSite = CreateConstructorCallSite(identity, registration, implementationType, context);
            }

            context.CallSiteCache.Add(callSite);
            return callSite;
        }

        return null;
    }

    private ServiceCallSite CreateFactoryCallSite(
        ServiceIdentity identity,
        ITypeSymbol? implementationType,
        ServiceLifetime lifetime,
        Location? registrationLocation,
        MemberLocation memberLocation,
        ISymbol factoryMember,
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

        if (context.CallSiteCache.TryGet(identity, out ServiceCallSite callSite))
        {
            return callSite;
        }

        implementationType ??= identity.Type;

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
                    identity.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
                _context.ReportDiagnostic(diagnostic);
                return new ErrorCallSite(identity, diagnostic);
        }

        var (parameters, namedParameters, diagnostics) =
            GetParameters(factoryParameters, registrationLocation, implementationType, context);

        if (diagnostics.Count > 0)
        {
            return new ErrorCallSite(identity, diagnostics.ToArray());
        }

        var factoryCallSite = new FactoryCallSite(identity,
            factoryMember,
            memberLocation: memberLocation,
            parameters.ToArray(),
            namedParameters.ToArray(),
            lifetime,
            true);

        return factoryCallSite;
    }

    private ServiceCallSite CreateConstructorCallSite(
        ServiceIdentity identity,
        ServiceRegistration registration,
        INamedTypeSymbol implementationType,
        ServiceResolutionContext context)
    {
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

                return new ErrorCallSite(identity, diagnostic);
            }

            var (parameters, namedParameters, diagnostics) =
                GetParameters(ctor.Parameters, registration.Location, implementationType, context);

            if (diagnostics.Count > 0)
            {
                return new ErrorCallSite(identity, diagnostics.ToArray());
            }

            return new ConstructorCallSite(
                identity,
                implementationType,
                parameters.ToArray(),
                namedParameters.ToArray(),
                registration.Lifetime,
                // TODO: this can be optimized to avoid check for all the types
                isDisposable: null
            );
        }
        finally
        {
            context.Remove(implementationType);
        }
    }

    private (
        List<ServiceCallSite> Parameters,
        List<KeyValuePair<IParameterSymbol, ServiceCallSite>> NamedParameters,
        List<Diagnostic> Diagnostics) GetParameters(
            ImmutableArray<IParameterSymbol> parameters,
            Location? registrationLocation,
            ITypeSymbol implementationType,
            ServiceResolutionContext context)
    {
        var callSites = new List<ServiceCallSite>();
        var namedParameters = new List<KeyValuePair<IParameterSymbol, ServiceCallSite>>();
        var diagnostics = new List<Diagnostic>();
        foreach (var parameterSymbol in parameters)
        {
            string? registrationName = null;
            foreach (var attributeData in parameterSymbol.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass,
                        _knownTypes.FromNamedServicesAttribute))
                {
                    registrationName = (string?)attributeData.ConstructorArguments[0].Value;
                    ValidateServiceName(registrationName, attributeData);
                }

                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass,
                        _knownTypes.FromKeyedServicesAttribute))
                {
                    var key = attributeData.ConstructorArguments[0].Value;
                    if (key is not string)
                    {
                        var diagnostic = Diagnostic.Create(
                            DiagnosticDescriptors.OnlyStringKeysAreSupported,
                            attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                            key);
                        _context.ReportDiagnostic(diagnostic);
                    }
                    registrationName = Convert.ToString(key);
                    ValidateServiceName(registrationName, attributeData);
                }
            }

            var parameterCallSite = GetCallSite(
                parameterSymbol.Type,
                registrationName,
                context.WithRequestLocation(ExtractMemberTypeLocation(parameterSymbol)));
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
                    Diagnostic diagnostic;
                    if (registrationName == null)
                    {
                        diagnostic = Diagnostic.Create(
                            isNullable ? DiagnosticDescriptors.NullableServiceNotRegistered : DiagnosticDescriptors.ServiceRequiredToConstructNotRegistered,
                            registrationLocation,
                            parameterSymbol.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                            implementationType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
                    }
                    else
                    {
                        diagnostic = Diagnostic.Create(DiagnosticDescriptors.NamedServiceRequiredToConstructNotRegistered,
                            registrationLocation,
                            parameterSymbol.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                            registrationName,
                            implementationType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
                    }

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
        var knownTypes =
            SymbolEqualityComparer.Default.Equals(moduleType.ContainingAssembly, _context.Compilation.Assembly)
                ? _knownTypes
                : new KnownTypes(_context.Compilation, moduleType.ContainingModule, moduleType.ContainingAssembly);

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

        string? registrationName = null;
        string? instanceMemberName = null;
        string? factoryMemberName = null;
        foreach (var namedArgument in attributeData.NamedArguments)
        {
            switch (namedArgument.Key)
            {
                case KnownTypes.NameAttributePropertyName:
                    registrationName = (string?)namedArgument.Value.Value;
                    ValidateServiceName(registrationName, attributeData);
                    break;
                case KnownTypes.InstanceAttributePropertyName:
                    instanceMemberName = (string?)namedArgument.Value.Value;
                    break;
                case KnownTypes.FactoryAttributePropertyName:
                    factoryMemberName = (string?)namedArgument.Value.Value;
                    break;
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
            registrationName,
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

    private void ValidateServiceName(string? name, AttributeData attributeData)
    {
        if (name == null)
        {
            return;
        }

        bool badName = name == "" || !char.IsLetter(name[0]);

        foreach (var c in name)
        {
            if (!char.IsLetterOrDigit(c))
            {
                badName = true;
            }
        }

        if (badName)
        {
            _context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.ServiceNameMustBeAlphanumeric,
                attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                name));
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

        return (INamedTypeSymbol)typedConstant.Value;
    }

    private Location? ExtractMemberTypeLocation(ISymbol symbol)
    {
        foreach (var declaringSyntaxReference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = declaringSyntaxReference.GetSyntax();
            return syntax switch
            {
                ParameterSyntax { Type: {} type } => type.GetLocation(),
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

    private class CallSiteCache
    {
        private Dictionary<ServiceIdentity, ServiceCallSite> _callSites = new();
        public bool TryGet(ServiceIdentity identity, out ServiceCallSite callSite)
        {
            return _callSites.TryGetValue(identity, out callSite);
        }

        public void Add(ServiceCallSite callSite)
        {
            _callSites.Add(callSite.Identity, callSite);
        }

        public ServiceCallSite[] GetRootCallSites()
        {
            return _callSites.Values.ToArray();
        }
    }
    private class ServiceResolutionContext
    {
        private HashSet<ServiceChainItem> _chain = new();

        public CallSiteCache CallSiteCache { get; }
        public ServiceProviderDescription ProviderDescription { get; }
        public Location? RequestLocation { get; }

        public ServiceResolutionContext(
            ServiceProviderDescription providerDescription,
            CallSiteCache serviceCallSites,
            Location? requestLocation)
        {
            CallSiteCache = serviceCallSites;
            ProviderDescription = providerDescription;
            RequestLocation = requestLocation;
        }

        public ServiceResolutionContext WithRequestLocation(Location? requestLocation)
        {
            return new ServiceResolutionContext(ProviderDescription, CallSiteCache, requestLocation)
            {
                _chain = this._chain
            };
        }

        public bool TryAdd(ITypeSymbol typeSymbol)
        {
            var serviceChainItem = new ServiceChainItem(typeSymbol, _chain.Count);
            if (_chain.Contains(serviceChainItem))
            {
                return false;
            }

            _chain.Add(serviceChainItem);
            return true;
        }

        public void Remove(ITypeSymbol typeSymbol)
        {
            _chain.Remove(new ServiceChainItem(typeSymbol, 0));
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