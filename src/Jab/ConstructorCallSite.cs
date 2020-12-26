using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ConstructorCallSite : ServiceCallSite
    {
        public ConstructorCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ServiceCallSite[] parameters, bool singleton) : base(serviceType, implementationType, singleton)
        {
            Parameters = parameters;
        }

        public ServiceCallSite[] Parameters { get; }
    }
}