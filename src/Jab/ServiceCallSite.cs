using Microsoft.CodeAnalysis;

namespace Jab
{
    internal abstract record ServiceCallSite
    {
        protected ServiceCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, bool singleton, int reverseIndex)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Singleton = singleton;
            ReverseIndex = reverseIndex;
        }

        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol ImplementationType { get; }
        public bool Singleton { get; }
        public int ReverseIndex { get; }
        public bool IsMainImplementation => ReverseIndex == 0;
    }
}