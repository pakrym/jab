namespace Jab;

internal record ArrayServiceCallSite: ServiceCallSite
{
    public ArrayServiceCallSite(ServiceIdentity identity, INamedTypeSymbol implementationType, ITypeSymbol itemType, ServiceCallSite[] items, ServiceLifetime lifetime)
        : base(identity, implementationType, lifetime, false)
    {
        ItemType = itemType;
        Items = items;
    }

    public ITypeSymbol ItemType { get; }
    public ServiceCallSite[] Items { get; }
}