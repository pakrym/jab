﻿namespace Jab;

internal record ServiceRegistration(
    ServiceLifetime Lifetime,
    INamedTypeSymbol ServiceType,
    string? Name,
    INamedTypeSymbol? ImplementationType,
    ISymbol? InstanceMember,
    ISymbol? FactoryMember,
    Location? Location,
    MemberLocation MemberLocation,
    bool AutoDispose);

internal record RootService(INamedTypeSymbol Service, Location? Location);