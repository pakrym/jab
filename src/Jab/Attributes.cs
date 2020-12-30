using System;

#nullable enable

namespace Jab
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class ServiceProviderAttribute: Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal class SingletonAttribute: Attribute
    {
        public Type ServiceType { get; }

        public Type? ImplementationType { get; }

        public string? Instance { get; set; }

        public string? Factory { get; set; }

        public SingletonAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public SingletonAttribute(Type serviceType, Type implementationType)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal class TransientAttribute : Attribute
    {
        public Type ServiceType { get; }

        public Type? ImplementationType { get; }

        public string? Factory { get; set; }

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