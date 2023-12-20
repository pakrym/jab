namespace Jab;

internal record ScopeFactoryCallSite: ServiceCallSite
{
    public ScopeFactoryCallSite(ITypeSymbol serviceType) : base(new ServiceIdentity(serviceType, null, null), serviceType, ServiceLifetime.Transient, false)
    {
    }
}