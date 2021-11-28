using Microsoft.CodeAnalysis;

namespace Jab;

internal record ServiceProviderCallSite: ServiceCallSite
{
    public ServiceProviderCallSite(ITypeSymbol serviceType) : base(serviceType, serviceType, ServiceLifetime.Transient, 0, false)
    {
    }
}