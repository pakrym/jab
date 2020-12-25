using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ServiceRegistration
    {
        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol? ImplementationType { get; }
        public ServiceRegistration(INamedTypeSymbol serviceType, INamedTypeSymbol? implementationType)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }
    }
}