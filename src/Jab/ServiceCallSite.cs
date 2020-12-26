using Microsoft.CodeAnalysis;

namespace Jab
{
    internal abstract record ServiceCallSite
    {
        protected ServiceCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, bool singleton)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Singleton = singleton;
        }

        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol ImplementationType { get; }
        public bool Singleton { get; }
    }
}