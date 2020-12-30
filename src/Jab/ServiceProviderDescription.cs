using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ServiceProviderDescription
    {
        public ServiceProviderDescription(IReadOnlyList<ServiceRegistration> serviceRegistrations, ITypeSymbol[] rootServices)
        {
            ServiceRegistrations = serviceRegistrations;
            RootServices = rootServices;
        }

        public ITypeSymbol[] RootServices { get; }
        public IReadOnlyList<ServiceRegistration> ServiceRegistrations { get; }
    }
}