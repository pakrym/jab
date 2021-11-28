using Microsoft.CodeAnalysis;

namespace Jab;

internal struct GetServiceCallCandidate
{
    public ITypeSymbol ProviderType { get; }
    public ITypeSymbol ServiceType { get; }
    public Location? Location { get; }

    public GetServiceCallCandidate(ITypeSymbol providerType, ITypeSymbol serviceType, Location? location)
    {
        ProviderType = providerType;
        ServiceType = serviceType;
        Location = location;
    }
}