using System.Collections.Generic;

namespace Jab
{
    internal record ServiceProviderDescription
    {
        public ServiceProviderDescription(IReadOnlyList<ServiceRegistration> serviceRegistrations)
        {
            ServiceRegistrations = serviceRegistrations;
        }

        public IReadOnlyList<ServiceRegistration> ServiceRegistrations { get; }
    }
}