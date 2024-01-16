namespace Jab;

internal struct GetServiceCallCandidate
{
    public ITypeSymbol ProviderType { get; }
    public ITypeSymbol ServiceType { get; }
    public string? ServiceName { get; }
    public Location? Location { get; }

    public GetServiceCallCandidate(ITypeSymbol providerType, ITypeSymbol serviceType, string? serviceName, Location? location)
    {
        ProviderType = providerType;
        ServiceType = serviceType;
        ServiceName = serviceName;
        Location = location;
    }
}