using Xunit;

namespace Jab.Tests
{
    public partial class ConstructorSelectionTests
    {
        [Fact]
        public void SelectsLongestConstructorWhenAvailable()
        {
            SelectsLongestConstructorWhenAvailableContainer c = new();
            var service = Assert.IsType<ServiceImplementationWithParameter<IService1, IService2>>(c.GetService<IService>());
            Assert.NotNull(service.Parameter1);
            Assert.NotNull(service.Parameter2);
        }

        [ServiceProvider]
        [Transient(typeof(IService1), typeof(ServiceImplementation))]
        [Transient(typeof(IService2), typeof(ServiceImplementation))]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter<IService1, IService2>))]
        internal partial class SelectsLongestConstructorWhenAvailableContainer { }

        [Fact]
        public void SelectsShorterConstructorWhenSatisfiable()
        {
            SelectsShorterConstructorWhenSatisfiableContainer c = new();
            var service = Assert.IsType<ServiceImplementationWithParameter<IService1, IService2>>(c.GetService<IService>());
            Assert.NotNull(service.Parameter1);
            Assert.Null(service.Parameter2);
        }

        [ServiceProvider]
        [Transient(typeof(IService1), typeof(ServiceImplementation))]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter<IService1, IService2>))]
        internal partial class SelectsShorterConstructorWhenSatisfiableContainer { }

        [Fact]
        public void IgnoresOptionalParametersWhenNotAvailable()
        {
            IgnoresOptionalParametersWhenNotAvailableContainer c = new();
            var service = Assert.IsType<ServiceImplementationWithParameter<IService1, IService2, IService3>>(c.GetService<IService>());
            Assert.Equal(1, service.SelectedCtor);
            Assert.NotNull(service.Parameter1);
            Assert.Null(service.Parameter2);
            Assert.Null(service.Parameter3);
        }

        [ServiceProvider]
        [Transient(typeof(IService1), typeof(ServiceImplementation))]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter<IService1, IService2, IService3>))]
        internal partial class IgnoresOptionalParametersWhenNotAvailableContainer { }

        [Fact]
        public void PassesOptionalParametersWhenAvailable()
        {
            PassesOptionalParametersWhenAvailableContainer c = new();
            var service = Assert.IsType<ServiceImplementationWithParameter<IService1, IService2, IService3>>(c.GetService<IService>());
            Assert.Equal(3, service.SelectedCtor);
            Assert.NotNull(service.Parameter1);
            Assert.Null(service.Parameter2);
            Assert.NotNull(service.Parameter3);
        }

        [ServiceProvider]
        [Transient(typeof(IService1), typeof(ServiceImplementation))]
        [Transient(typeof(IService3), typeof(ServiceImplementation))]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter<IService1, IService2, IService3>))]
        internal partial class PassesOptionalParametersWhenAvailableContainer { }
    }
}