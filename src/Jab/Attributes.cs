using System;

#nullable enable

namespace Jab
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class ServiceProviderAttribute: Attribute
    {
        public Type[]? RootServices { get; set; }
    }

    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    internal class ServiceProviderModuleAttribute: Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class ImportAttribute: Attribute
    {
        public Type ModuleType { get; }

        public ImportAttribute(Type moduleType)
        {
            ModuleType = moduleType;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class ScopedAttribute : Attribute
    {
        public Type ServiceType { get; }

        public Type? ImplementationType { get; }

        public string? Factory { get; set; }

        public ScopedAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public ScopedAttribute(Type serviceType, Type implementationType)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }
    }

#if JAB_PREVIEW
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class ImportAttribute<TModule> : ImportAttribute
    {
        public ImportAttribute() : base(typeof(TModule))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class TransientAttribute<TService> : TransientAttribute
    {
        public TransientAttribute() : base(typeof(TService))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class TransientAttribute<TService, TImpl> : TransientAttribute
    {
        public TransientAttribute() : base(typeof(TService), typeof(TImpl))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class ScopedAttribute<TService> : ScopedAttribute
    {
        public ScopedAttribute() : base(typeof(TService))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class ScopedAttribute<TService, TImpl> : ScopedAttribute
    {
        public ScopedAttribute() : base(typeof(TService), typeof(TImpl))
        {
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class SingletonAttribute<TService> : SingletonAttribute
    {
        public SingletonAttribute() : base(typeof(TService))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    internal class SingletonAttribute<TService, TImpl> : SingletonAttribute
    {
        public SingletonAttribute() : base(typeof(TService), typeof(TImpl))
        {
        }
    }

#endif

    internal interface IServiceProvider<T>
    {
        T GetService();
    }
}