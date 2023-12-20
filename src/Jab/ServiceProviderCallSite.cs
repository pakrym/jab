namespace Jab;

internal record ServiceProviderCallSite: ServiceCallSite
{
    public ServiceProviderCallSite(ITypeSymbol serviceType) : base(new ServiceIdentity(serviceType, null, null), serviceType, ServiceLifetime.Transient, false)
    {
    }
}