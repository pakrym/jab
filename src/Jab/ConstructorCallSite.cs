using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ConstructorCallSite : ServiceCallSite
    {
        public ConstructorCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ServiceCallSite[] parameters, bool singleton, int reverseIndex) : base(serviceType, implementationType, singleton, reverseIndex)
        {
            Parameters = parameters;
        }

        public ServiceCallSite[] Parameters { get; }
    }
}