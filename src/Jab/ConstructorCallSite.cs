using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ConstructorCallSite : ServiceCallSite
    {
        public ConstructorCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ServiceCallSite[] parameters, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable)
            : base(serviceType, implementationType, lifetime, reverseIndex, isDisposable)
        {
            Parameters = parameters;
        }

        public ServiceCallSite[] Parameters { get; }
    }
}