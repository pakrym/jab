namespace Jab;

internal record ServiceRegistration(ServiceLifetime Lifetime, INamedTypeSymbol ServiceType, INamedTypeSymbol? ImplementationType, ISymbol? InstanceMember, ISymbol? FactoryMember, Location? Location)
{
    public INamedTypeSymbol ServiceType { get; } = ServiceType;
    public INamedTypeSymbol? ImplementationType { get; } = ImplementationType;
    public ISymbol? InstanceMember { get; } = InstanceMember;
    public ISymbol? FactoryMember { get; } = FactoryMember;
    public ServiceLifetime Lifetime { get; } = Lifetime;
    public Location? Location { get; } = Location;
}

internal record RootService(INamedTypeSymbol Service, Location? Location);