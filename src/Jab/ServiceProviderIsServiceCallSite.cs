namespace Jab;

internal record ServiceProviderIsServiceCallSite: ServiceCallSite
{
    public ServiceProviderIsServiceCallSite(ITypeSymbol serviceType) : base(new ServiceIdentity(serviceType, null, null), serviceType, ServiceLifetime.Transient, false)
    {
    }
}