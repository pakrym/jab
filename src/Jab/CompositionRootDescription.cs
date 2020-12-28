using System.Collections.Generic;

namespace Jab
{
    internal record CompositionRootDescription
    {
        public CompositionRootDescription(IReadOnlyList<ServiceRegistration> serviceRegistrations)
        {
            ServiceRegistrations = serviceRegistrations;
        }

        public IReadOnlyList<ServiceRegistration> ServiceRegistrations { get; }
    }
}