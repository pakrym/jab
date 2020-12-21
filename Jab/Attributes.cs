using System;

namespace Jab
{
    internal class CompositionRootAttribute: Attribute
    {
    }

    internal class SingletonAttribute: Attribute
    {
    }

    internal class TransientAttribute : Attribute
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; }

        public TransientAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public TransientAttribute(Type serviceType, Type implementationType)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }
    }
}