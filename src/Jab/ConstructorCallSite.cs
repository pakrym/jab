using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ConstructorCallSite : ServiceCallSite
    {
        public ConstructorCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ServiceCallSite[] parameters, ServiceLifetime lifetime, int reverseIndex)
            : base(serviceType, implementationType, lifetime, reverseIndex)
        {
            Parameters = parameters;
        }

        public ServiceCallSite[] Parameters { get; }
    }
}