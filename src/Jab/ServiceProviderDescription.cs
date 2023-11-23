namespace Jab;

internal record ServiceProviderDescription
{
    public ServiceProviderDescription(IReadOnlyList<ServiceRegistration> serviceRegistrations, RootService[] rootServices, Location? location)
    {
        Location = location;
        RootServices = rootServices;
        ServiceRegistrations = serviceRegistrations;
        ServiceRegistrationsLookup = new(SymbolEqualityComparer.Default);

        foreach (var registration in serviceRegistrations)
        {
            var registrations = ServiceRegistrationsLookup[registration.ServiceType] ??= new();
            registrations.Add(registration);
        }
    }

    public Dictionary<ITypeSymbol, List<ServiceRegistration>> ServiceRegistrationsLookup { get; }

    public Location? Location { get; }
    public RootService[] RootServices { get; }
    public IReadOnlyList<ServiceRegistration> ServiceRegistrations { get; }
}