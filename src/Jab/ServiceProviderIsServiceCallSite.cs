namespace Jab;

internal record ServiceProviderIsServiceCallSite: ServiceCallSite
{
    public ServiceProviderIsServiceCallSite(ITypeSymbol serviceType) : base(serviceType, serviceType, ServiceLifetime.Transient, 0, false)
    {
    }
}