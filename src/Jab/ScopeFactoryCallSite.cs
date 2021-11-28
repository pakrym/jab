namespace Jab;

internal record ScopeFactoryCallSite: ServiceCallSite
{
    public ScopeFactoryCallSite(ITypeSymbol serviceType) : base(serviceType, serviceType, ServiceLifetime.Transient, 0, false)
    {
    }
}