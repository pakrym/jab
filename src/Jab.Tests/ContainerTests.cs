using System;
using Xunit;

namespace Jab.Tests
{
    public partial class ContainerTests
    {
        [Fact]
        public void CanCreateTransientService()
        {
            CanCreateTransientServiceContainer c = new();
            Assert.IsType<ServiceImplementation>(c.GetIService());
        }

        [CompositionRoot]
        [Transient(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanCreateTransientServiceContainer { }

        [Fact]
        public void CanCreateTransientServiceWithConstructorParameters()
        {
            CanCreateTransientServiceWithConstructorParametersContainer c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetIService());
            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
        }

        [CompositionRoot]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanCreateTransientServiceWithConstructorParametersContainer { }

        [Fact]
        public void CanCreateSingleton()
        {
            CanCreateSingletonContainer c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetIService());
            var implementationWithParameter2 = Assert.IsType<ServiceImplementationWithParameter>(c.GetIService());
            var anotherImplementation = c.GetIAnotherService();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(implementationWithParameter, implementationWithParameter2);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
        }

        [CompositionRoot]
        [Singleton(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanCreateSingletonContainer { }

        [Fact]
        public void CanUseSingletonInstance()
        {
            CanUseSingletonInstanceContainer c = new();
            c.MyIServiceInstance = new AnotherServiceImplementation();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetIService());
            var anotherImplementation = c.GetIAnotherService();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Same(c.MyIServiceInstance, anotherImplementation);
        }

        [CompositionRoot]
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
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetIService());
            var anotherImplementation = c.GetIAnotherService();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Equal(1, c.FactoryInvocationCount);
        }

        [CompositionRoot]
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
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetIService());
            var anotherImplementation = c.GetIAnotherService();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.NotSame(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Equal(2, c.FactoryInvocationCount);
        }

        [CompositionRoot]
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
    }
}