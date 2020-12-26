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
    }
}