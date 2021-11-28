namespace Jab;

internal record ArrayServiceCallSite: ServiceCallSite
{
    public ArrayServiceCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ITypeSymbol itemType, ServiceCallSite[] items, ServiceLifetime lifetime)
        : base(serviceType, implementationType, lifetime, 0, false)
    {
        ItemType = itemType;
        Items = items;
    }

    public ITypeSymbol ItemType { get; }
    public ServiceCallSite[] Items { get; }
}