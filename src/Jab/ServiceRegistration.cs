using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ServiceRegistration
    {
        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol? ImplementationType { get; }
        public ISymbol? InstanceMember { get; }
        public ISymbol? FactoryMember { get; }
        public ServiceLifetime Lifetime { get; }

        public ServiceRegistration(ServiceLifetime lifetime, INamedTypeSymbol serviceType, INamedTypeSymbol? implementationType, ISymbol? instanceMember, ISymbol? factoryMember)
        {
            Lifetime = lifetime;
            ServiceType = serviceType;
            ImplementationType = implementationType;
            InstanceMember = instanceMember;
            FactoryMember = factoryMember;
        }
    }
}