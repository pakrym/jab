namespace Jab;

internal class ServiceProviderBuilder
{
    private readonly GeneratorContext _context;
    private readonly KnownTypes _knownTypes;

    public ServiceProviderBuilder(GeneratorContext context)
    {
        _context = context;
        _knownTypes = new KnownTypes(context.Compilation, context.Compilation.Assembly);
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

        CallSiteCache callSites = new();
        foreach (var registration in description.ServiceRegistrations)
        {
            if (registration.ServiceType.IsUnboundGenericType)
            {
                continue;
            }

            GetCallSite(
                new ServiceRequest(registration.ServiceType, registration.Name),
                new ServiceResolutionContext(description, callSites, registration.ServiceType, registration.Location));
        }

        foreach (var rootService in description.RootServices)
        {
            var serviceType = rootService.Service;
            var callSite = GetCallSite(
                new ServiceRequest(serviceType),
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
                var callSite = GetCallSite(
                    new ServiceRequest(serviceType),
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
        ServiceRequest request,
        ServiceResolutionContext context)
    {
        if (context.CallSiteCache.TryGet(request, out var cachedCallSite))
        {
            return cachedCallSite;
        }

        if (!context.TryAdd(request.Type))
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.CyclicDependencyDetected,
                context.RequestLocation,
                context.RequestService.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                request.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                context.ToString(request.Type));

            _context.ReportDiagnostic(
                diagnostic);

            return new ErrorCallSite(request.Type, diagnostic);
        }

        try
        {
            return TryCreateSpecial(request, context) ??
                   TryCreateExact(request, context) ??
                   TryCreateEnumerable(request, context) ??
                   TryCreateGeneric(request, context);
        }
        catch
        {
            context.Remove(request.Type);
            throw;
        }
    }

    private ServiceCallSite? TryCreateSpecial(ServiceRequest request, ServiceResolutionContext context)
    {
        bool CheckNotNamed()
        {
            if (request.Name == null) return true;

            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.BuiltInServicesAreNotNamed,
                context.RequestLocation,
                request.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

            _context.ReportDiagnostic(
                diagnostic);
            return false;

        }

        Debug.Assert(request.ReverseIndex == null);

        if (SymbolEqualityComparer.Default.Equals(request.Type, _knownTypes.IServiceProviderType)
            && CheckNotNamed())
        {
            var callSite = new ServiceProviderCallSite(request.Type);
            context.CallSiteCache.Add(request, callSite);
            return callSite;
        }

        if (SymbolEqualityComparer.Default.Equals(request.Type, _knownTypes.IServiceScopeFactoryType)
            && CheckNotNamed())
        {
            var callSite = new ScopeFactoryCallSite(request.Type);
            context.CallSiteCache.Add(request, callSite);
            return callSite;
        }

        return null;
    }

    private ServiceCallSite? TryCreateGeneric(
        ServiceRequest request,
        ServiceResolutionContext context)
    {
        if (request.Type is INamedTypeSymbol { IsGenericType: true })
        {
            for (int i = context.ProviderDescription.ServiceRegistrations.Count - 1; i >= 0; i--)
            {
                var registration = context.ProviderDescription.ServiceRegistrations[i];

                var callSite = TryCreateGeneric(request, registration, context);
                if (callSite != null)
                {
                    return callSite;
                }
            }
        }

        return null;
    }

    private ServiceCallSite? TryCreateGeneric(
        ServiceRequest request,
        ServiceRegistration registration,
        ServiceResolutionContext context)
    {
        if (registration.Name == request.Name &&
            request.Type is INamedTypeSymbol { IsGenericType: true } genericType &&
            registration.ServiceType.IsUnboundGenericType &&
            SymbolEqualityComparer.Default.Equals(registration.ServiceType.ConstructedFrom,
                genericType.ConstructedFrom))
        {
            // TODO: This can use better error reporting
            if (registration.FactoryMember is IMethodSymbol factoryMethod)
            {
                var constructedFactoryMethod = factoryMethod.ConstructedFrom.Construct(genericType.TypeArguments,
                    genericType.TypeArgumentNullableAnnotations);
               return CreateFactoryCallSite(
                    request,
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

                return CreateConstructorCallSite(request, registration, implementationType, context);
            }
            else
            {
                throw new InvalidOperationException($"Can't construct generic callsite for {request.Type}");
            }
        }

        return null;
    }

    private ServiceCallSite? TryCreateEnumerable(ServiceRequest request, ServiceResolutionContext context)
    {
        bool CheckNotNamed()
        {
            if (request.Name == null) return true;

            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ImplicitIEnumerableNotNamed,
                context.RequestLocation,
                request.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

            _context.ReportDiagnostic(
                diagnostic);
            return false;

        }

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

        if (request.Type is INamedTypeSymbol { IsGenericType: true } genericType &&
            SymbolEqualityComparer.Default.Equals(genericType.ConstructedFrom, _knownTypes.IEnumerableType) &&
            CheckNotNamed())
        {
            var enumerableService = genericType.TypeArguments[0];
            var items = new List<ServiceCallSite>();
            int reverseIndex = 0;
            var itemRequest = new ServiceRequest(enumerableService, reverseIndex);
            for (int i = context.ProviderDescription.ServiceRegistrations.Count - 1; i >= 0; i--)
            {
                var registration = context.ProviderDescription.ServiceRegistrations[i];

                var itemCallSite = TryCreateGeneric(itemRequest, registration, context) ??
                                   TryCreateExact(itemRequest, registration, context);
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
            context.CallSiteCache.Add(request, callSite);

            return callSite;
        }

        return null;
    }

    private ServiceCallSite? TryCreateExact(ServiceRequest request, ServiceResolutionContext context)
    {
        if (!context.ProviderDescription.ServiceRegistrationsLookup.TryGetValue(request.Type, out var registrations))
        {
            return null;
        }
        foreach (var registration in registrations)
        {
            var callSite = TryCreateExact(request, registration, context: context);
            if (callSite != null)
            {
                return callSite;
            }
        }

        return null;
    }

    private ServiceCallSite? TryCreateExact(
        ServiceRequest request,
        ServiceRegistration registration,
        ServiceResolutionContext context)
    {
        if (
            registration.Name == request.Name &&
            SymbolEqualityComparer.Default.Equals(registration.ServiceType, request.Type))
        {
            return CreateCallSite(request, registration, context: context);
        }

        return null;
    }

    private ServiceCallSite CreateCallSite(
        ServiceRequest request,
        ServiceRegistration registration,
        ServiceResolutionContext context)
    {
        Debug.Assert(request.Name == registration.Name);
        if (context.CallSiteCache.TryGet(request, out ServiceCallSite callSite))
        {
            return callSite;
        }

        if (registration.InstanceMember is { } instanceMember)
        {
            callSite = CreateMemberCallSite(
                request,
                registration,
                instanceMember,
                registration.MemberLocation);
        }
        else if (registration.FactoryMember is { } factoryMember)
        {
            callSite = CreateFactoryCallSite(
                request,
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

            callSite = CreateConstructorCallSite(request, registration, implementationType, context);
        }

        return callSite;
    }

    private ServiceCallSite CreateMemberCallSite(
        ServiceRequest request,
        ServiceRegistration registration,
        ISymbol instanceMember,
        MemberLocation memberLocation)
    {
        return new MemberCallSite(registration.ServiceType,
            instanceMember,
            memberLocation: memberLocation,
            registration.Lifetime,
            request.ReverseIndex,
            false);
    }

    private ServiceCallSite CreateFactoryCallSite(
        ServiceRequest request,
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

        if (context.CallSiteCache.TryGet(request, out ServiceCallSite callSite))
        {
            return callSite;
        }

        implementationType ??= request.Type;

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
                    request.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
                _context.ReportDiagnostic(diagnostic);
                return new ErrorCallSite(request.Type, diagnostic);
        }

        var (parameters, namedParameters, diagnostics) =
            GetParameters(factoryParameters, registrationLocation, implementationType, context);

        if (diagnostics.Count > 0)
        {
            return new ErrorCallSite(request.Type, diagnostics.ToArray());
        }

        var factoryCallSite = new FactoryCallSite(request.Type,
            factoryMember,
            memberLocation: memberLocation,
            parameters.ToArray(),
            namedParameters.ToArray(),
            lifetime,
            request.ReverseIndex,
            false);

        context.CallSiteCache.Add(request, factoryCallSite);
        return factoryCallSite;
    }

    private ServiceCallSite CreateConstructorCallSite(
        ServiceRequest request,
        ServiceRegistration registration,
        INamedTypeSymbol implementationType,
        ServiceResolutionContext context)
    {
        if (context.CallSiteCache.TryGet(request, out ServiceCallSite callSite))
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

                return new ErrorCallSite(request.Type, diagnostic);
            }

            var (parameters, namedParameters, diagnostics) =
                GetParameters(ctor.Parameters, registration.Location, implementationType, context);

            if (diagnostics.Count > 0)
            {
                return new ErrorCallSite(request.Type, diagnostics.ToArray());
            }

            callSite = new ConstructorCallSite(
                request.Type,
                implementationType,
                parameters.ToArray(),
                namedParameters.ToArray(),
                registration.Lifetime,
                request.ReverseIndex,
                // TODO: this can be optimized to avoid check for all the types
                isDisposable: null
            );

            context.CallSiteCache.Add(request, callSite);

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
                        _knownTypes.ServiceNameAttribute))
                {
                    registrationName = (string?)attributeData.ConstructorArguments[0].Value;
                }
            }

            var request = new ServiceRequest(parameterSymbol.Type, registrationName);
            var parameterCallSite = GetCallSite(request, context);
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
                if (parameterCallSite == null)
                {
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ServiceRequiredToConstructNotRegistered,
                        registrationLocation,
                        parameterSymbol.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                        implementationType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

                    diagnostics.Add(diagnostic);
                    _context.ReportDiagnostic(diagnostic);
                }
                else
                {
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
                : new KnownTypes(_context.Compilation, moduleType.ContainingAssembly);

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

    private readonly record struct ServiceRequest
    {
        public ITypeSymbol Type { get; }
        public string? Name { get; }
        public int? ReverseIndex { get; }

        private ServiceRequest(ITypeSymbol type, string? name, int? reverseIndex)
        {
            Type = type;
            Name = name;
            ReverseIndex = reverseIndex == 0 ? null : reverseIndex;
        }
        public ServiceRequest(ITypeSymbol type) : this(type, null, null) {}
        public ServiceRequest(ITypeSymbol type, string? name) : this(type, name, null) {}
        public ServiceRequest(ITypeSymbol type, int? reverseIndex) : this(type, null, reverseIndex) {}
    }

    private class CallSiteCache
    {
        public bool TryGet(ServiceRequest request, out ServiceCallSite callSite)
        {
            callSite = default!;
            return false;
        }

        public void Add(ServiceRequest request, ServiceCallSite callSite)
        {
        }

        public ServiceCallSite[] GetRootCallSites()
        {
            return null!;
        }
    }
    private class ServiceResolutionContext
    {
        private readonly HashSet<ServiceChainItem> _chain = new();
        private int _index;

        public CallSiteCache CallSiteCache { get; }
        public ITypeSymbol RequestService { get; }
        public ServiceProviderDescription ProviderDescription { get; }
        public Location? RequestLocation { get; }

        public ServiceResolutionContext(
            ServiceProviderDescription providerDescription,
            CallSiteCache serviceCallSites,
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