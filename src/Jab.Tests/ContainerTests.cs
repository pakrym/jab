using System;
using System.Collections.Generic;
using Xunit;

namespace Jab.Tests
{
    public partial class ContainerTests
    {
        [Fact]
        public void CanCreateTransientService()
        {
            CanCreateTransientServiceContainer c = new();
            Assert.IsType<ServiceImplementation>(c.GetService());
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanCreateTransientServiceContainer { }

        [Fact]
        public void CanCreateTransientServiceWithConstructorParameters()
        {
            CanCreateTransientServiceWithConstructorParametersContainer c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanCreateTransientServiceWithConstructorParametersContainer { }

        [Fact]
        public void CanCreateSingleton()
        {
            CanCreateSingletonContainer c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var implementationWithParameter2 = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(implementationWithParameter, implementationWithParameter2);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanCreateSingletonContainer { }

        [Fact]
        public void CanUseSingletonInstance()
        {
            CanUseSingletonInstanceContainer c = new();
            c.MyIServiceInstance = new AnotherServiceImplementation();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Same(c.MyIServiceInstance, anotherImplementation);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Singleton(typeof(IAnotherService), Instance = "MyIServiceInstance")]
        internal partial class CanUseSingletonInstanceContainer
        {
            public IAnotherService MyIServiceInstance { get; set; }
        }

        [Fact]
        public void CanUseSingletonFactory()
        {
            CanUseSingletonFactoryContainer c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Equal(1, c.FactoryInvocationCount);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Singleton(typeof(IAnotherService), Factory = nameof(CreateMyIServiceInstance))]
        internal partial class CanUseSingletonFactoryContainer
        {
            public int FactoryInvocationCount;
            public IAnotherService CreateMyIServiceInstance()
            {
                FactoryInvocationCount++;
                return new AnotherServiceImplementation();
            }
        }

        [Fact]
        public void CanUseTransientFactory()
        {
            CanUseTransientFactoryContainer c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.NotSame(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Equal(2, c.FactoryInvocationCount);
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Transient(typeof(IAnotherService), Factory = nameof(CreateMyIServiceInstance))]
        internal partial class CanUseTransientFactoryContainer
        {
            public int FactoryInvocationCount;
            public IAnotherService CreateMyIServiceInstance()
            {
                FactoryInvocationCount++;
                return new AnotherServiceImplementation();
            }
        }

        [Fact]
        public void CanResolveIEnumerableOfTransients()
        {
            CanResolveIEnumerableOfTransientsContainer c = new();
            var enumerable = c.GetService<IEnumerable<IAnotherService>>();
            var array = Assert.IsType<IAnotherService[]>(enumerable);
            Assert.Equal(3, array.Length);
            var service1 = array[0];
            var service2 = array[1];
            var service3 = array[2];
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotNull(service3);
            Assert.NotSame(service1, service2);
            Assert.NotSame(service2, service3);
        }

        [ServiceProvider(RootServices = new [] {typeof(IEnumerable<IAnotherService>)})]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveIEnumerableOfTransientsContainer
        {
        }


        [Fact]
        public void CanResolveIEnumerableOfSingletons()
        {
            CanResolveIEnumerableOfSingletonsContainer c = new();
            var enumerable = c.GetService<IEnumerable<IAnotherService>>();
            var array = Assert.IsType<IAnotherService[]>(enumerable);
            Assert.Equal(3, array.Length);
            var service1 = array[0];
            var service2 = array[1];
            var service3 = array[2];
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotNull(service3);
            Assert.NotSame(service1, service2);
            Assert.NotSame(service2, service3);

            Assert.Same(service3, c.GetService<IAnotherService>());
            Assert.Same(enumerable, c.GetService<IEnumerable<IAnotherService>>());
        }

        [ServiceProvider(RootServices = new [] {typeof(IEnumerable<IAnotherService>)})]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveIEnumerableOfSingletonsContainer
        {
        }

        [Fact]
        public void CanResolveIEnumerableInferredFromParameter()
        {
            CanResolveIEnumerableInferredFromParameterContainer c = new();
            var enumerable = c.GetService<IEnumerable<IAnotherService>>();
            var array = Assert.IsType<IAnotherService[]>(enumerable);
            Assert.Equal(3, array.Length);
            var service1 = array[0];
            var service2 = array[1];
            var service3 = array[2];
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotNull(service3);
            Assert.NotSame(service1, service2);
            Assert.NotSame(service2, service3);
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter<IEnumerable<IAnotherService>>))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveIEnumerableInferredFromParameterContainer
        {
        }

        [Fact]
        public void CanResolveOpenGenericService()
        {
            CanResolveOpenGenericServiceContainer c = new();
            var service = c.GetService<IService<IAnotherService>>();
            Assert.IsType<ServiceImplementation<IAnotherService>>(service);
            Assert.IsType<AnotherServiceImplementation>(service.InnerService);
        }

        [ServiceProvider(RootServices = new [] {typeof(IService<IAnotherService>)})]
        [Transient(typeof(IService<>), typeof(ServiceImplementation<>))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveOpenGenericServiceContainer
        {
        }

        [Fact]
        public void CanResolveEnumerableOfMixedOpenGenericService()
        {
            CanResolveEnumerableOfMixedOpenGenericServiceContainer c = new();
            c.Instance = new ServiceImplementation<IAnotherService>(new AnotherServiceImplementation());

            var services = c.GetService<IEnumerable<IService<IAnotherService>>>();
            var array = Assert.IsType<IService<IAnotherService>[]>(services);
            Assert.IsType<ServiceImplementation<IAnotherService>>(array[0]);
            Assert.Same(c.Instance, array[1]);
        }

        [ServiceProvider(RootServices = new [] {typeof(IEnumerable<IService<IAnotherService>>)})]
        [Transient(typeof(IService<>), typeof(ServiceImplementation<>))]
        [Singleton(typeof(IService<IAnotherService>), Instance = nameof(Instance))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveEnumerableOfMixedOpenGenericServiceContainer
        {
            public IService<IAnotherService> Instance { get; set; }
        }
    }
}