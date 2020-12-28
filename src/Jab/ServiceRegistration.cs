using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ServiceRegistration
    {
        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol? ImplementationType { get; }
        public ISymbol? InstanceMember { get; }
        public ServiceLifetime Lifetime { get; }

        public ServiceRegistration(ServiceLifetime lifetime, INamedTypeSymbol serviceType, INamedTypeSymbol? implementationType, ISymbol? instanceMember)
        {
            Lifetime = lifetime;
            ServiceType = serviceType;
            ImplementationType = implementationType;
            InstanceMember = instanceMember;
        }
    }
}