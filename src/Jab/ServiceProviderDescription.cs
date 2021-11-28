namespace Jab;

internal record ServiceProviderDescription(IReadOnlyList<ServiceRegistration> ServiceRegistrations, ITypeSymbol[] RootServices, Location? Location)
{
    public Location? Location { get; } = Location;
    public ITypeSymbol[] RootServices { get; } = RootServices;
    public IReadOnlyList<ServiceRegistration> ServiceRegistrations { get; } = ServiceRegistrations;
}