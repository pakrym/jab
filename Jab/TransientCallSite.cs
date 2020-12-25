using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record TransientCallSite : ServiceCallSite
    {
        public TransientCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ServiceCallSite[] parameters) : base(serviceType, implementationType)
        {
            Parameters = parameters;
        }

        public ServiceCallSite[] Parameters { get; set; }
    }
}