namespace Jab;

internal record ServiceProvider(ITypeSymbol Type, ServiceCallSite[] RootCallSites, KnownTypes KnownTypes)
{
    public ITypeSymbol Type { get; } = Type;
    public ServiceCallSite[] RootCallSites { get; } = RootCallSites;
    public KnownTypes KnownTypes { get; } = KnownTypes;
}