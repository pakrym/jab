using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Jab;

internal record ServiceProviderDescription
{
    public ServiceProviderDescription(IReadOnlyList<ServiceRegistration> serviceRegistrations, ITypeSymbol[] rootServices, Location? location)
    {
        ServiceRegistrations = serviceRegistrations;
        RootServices = rootServices;
        Location = location;
    }

    public Location? Location { get; }
    public ITypeSymbol[] RootServices { get; }
    public IReadOnlyList<ServiceRegistration> ServiceRegistrations { get; }
}