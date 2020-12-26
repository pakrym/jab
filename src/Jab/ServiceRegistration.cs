using Microsoft.CodeAnalysis;

namespace Jab
{
    internal enum ServiceLifetime
    {
        Transient,
        Singleton
    }
    internal record ServiceRegistration
    {
        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol? ImplementationType { get; }
        public ServiceLifetime Lifetime { get; }
        public ServiceRegistration(ServiceLifetime lifetime, INamedTypeSymbol serviceType, INamedTypeSymbol? implementationType)
        {
            Lifetime = lifetime;
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }
    }
}