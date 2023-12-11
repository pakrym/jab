﻿// <auto-generated/>

#if !JAB_ATTRIBUTES_REFERENCED || JAB_ATTRIBUTES_PACKAGE

using System;
using System.Threading.Tasks;

#nullable enable

namespace Jab
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class ServiceProviderAttribute: Attribute
    {
        public Type[]? RootServices { get; set; }
    }

    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class ServiceProviderModuleAttribute: Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class ImportAttribute: Attribute
    {
        public Type ModuleType { get; }

        public ImportAttribute(Type moduleType)
        {
            ModuleType = moduleType;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class SingletonAttribute: Attribute
    {
        public Type ServiceType { get; }

        public string? Name { get; set; }

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
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class TransientAttribute : Attribute
    {
        public Type ServiceType { get; }
        public string? Name { get; set; }

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
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class ScopedAttribute : Attribute
    {
        public Type ServiceType { get; }
        public string? Name { get; set; }

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


    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
        class FromNamedServicesAttribute : Attribute
    {
        public string? Name { get; set; }

        public FromNamedServicesAttribute(string name)
        {
            Name = name;
        }
    }

#if GENERIC_ATTRIBUTES
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class ImportAttribute<TModule> : ImportAttribute
    {
        public ImportAttribute() : base(typeof(TModule))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class TransientAttribute<TService> : TransientAttribute
    {
        public TransientAttribute() : base(typeof(TService))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class TransientAttribute<TService, TImpl> : TransientAttribute where TImpl: TService
    {
        public TransientAttribute() : base(typeof(TService), typeof(TImpl))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class ScopedAttribute<TService> : ScopedAttribute
    {
        public ScopedAttribute() : base(typeof(TService))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class ScopedAttribute<TService, TImpl> : ScopedAttribute where TImpl: TService
    {
        public ScopedAttribute() : base(typeof(TService), typeof(TImpl))
        {
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class SingletonAttribute<TService> : SingletonAttribute
    {
        public SingletonAttribute() : base(typeof(TService))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    class SingletonAttribute<TService, TImpl> : SingletonAttribute where TImpl: TService
    {
        public SingletonAttribute() : base(typeof(TService), typeof(TImpl))
        {
        }
    }

#endif

#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    interface IServiceProvider<T>
    {
        T GetService();
    }

#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    interface INamedServiceProvider<T>
    {
        T GetService(string name);
    }

#if JAB_ATTRIBUTES_PACKAGE
    public
#else
    internal
#endif
    static class JabHelpers
    {
        public static InvalidOperationException CreateServiceNotFoundException<T>(string? name = null) =>
            CreateServiceNotFoundException(typeof(T), name);
        public static InvalidOperationException CreateServiceNotFoundException(Type type, string? name = null) =>
            new InvalidOperationException(
                name != null ?
                    $"Service with type {type} and name {name} not registered" :
                    $"Service with type {type} not registered");
    }
}

#endif