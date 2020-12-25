using Microsoft.CodeAnalysis;

namespace Jab
{
    internal abstract record ServiceCallSite
    {
        protected ServiceCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }

        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol ImplementationType { get; }
    }
}