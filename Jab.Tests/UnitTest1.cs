using System;
using Xunit;

namespace Jab.Tests
{

    internal interface IService
    {
        void M();
    }

    internal class ServiceImplementation : IService
    {
        public void M()
        {
        }
    }

    internal class ServiceImplementationWithParameter : IService
    {
        public IAnotherService AnotherService { get; }

        public ServiceImplementationWithParameter(IAnotherService anotherService)
        {
            AnotherService = anotherService;
        }
        public void M()
        {
        }
    }

    internal interface IAnotherService
    {
        public void N()
        {
        }
    }

    internal class AnotherServiceImplementation: IAnotherService
    {
        public void N()
        {
        }
    }

    public partial class UnitTest1
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
    }
}