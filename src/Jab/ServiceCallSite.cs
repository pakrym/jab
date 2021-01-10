using Microsoft.CodeAnalysis;

namespace Jab
{
    internal abstract record ServiceCallSite
    {
        protected ServiceCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            ReverseIndex = reverseIndex;
            IsDisposable = isDisposable;
        }

        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol ImplementationType { get; }
        public ServiceLifetime Lifetime { get; }
        public int ReverseIndex { get; }
        public bool? IsDisposable { get; }
        public bool IsMainImplementation => ReverseIndex == 0;
    }
}