using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ThisCallSite: ServiceCallSite
    {
        public ThisCallSite(ITypeSymbol serviceType) : base(serviceType, serviceType, ServiceLifetime.Transient, 0, false)
        {
        }
    }
}