using System;
using Jab;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JabTests
{
    public partial class MEDIContainerTests
    {
        [Fact]
        public void CanResolveIServiceScopeFactory()
        {
            CanResolveIServiceScopeFactoryContainer c = new();
            var factory = c.GetService<IServiceScopeFactory>();

            var scope = factory.CreateScope();
            var scopeProvider = scope.ServiceProvider.GetService<IServiceScopeFactory>();

            Assert.IsType<CanResolveIServiceScopeFactoryContainer.Scope>(scope);
            Assert.Same(factory, c);
            Assert.Same(scopeProvider, c);
            Assert.Same(scope, scope.ServiceProvider);
        }

        [Fact]
        public void CanCreateScopeUsingExtensionMethod()
        {
            CanResolveIServiceScopeFactoryContainer c = new();
            var scope = ((IServiceProvider)c).CreateScope();
            Assert.IsType<CanResolveIServiceScopeFactoryContainer.Scope>(scope);
        }

        [ServiceProvider]
        internal partial class CanResolveIServiceScopeFactoryContainer
        {
        }

        [Fact]
        public void SupportsKeyedServices()
        {
            SupportsKeyedServicesContainer c = new();

            Assert.IsAssignableFrom<IKeyedServiceProvider>(c);

            Assert.NotNull(c.GetKeyedService<ServiceImplementation>("Key"));
            Assert.NotNull(c.GetRequiredKeyedService<ServiceImplementation>("Key"));

            Assert.Null(c.GetKeyedService<ServiceImplementation>("Bla"));
            Assert.Null(c.GetKeyedService<IService>("Bla"));
            Assert.Throws<InvalidOperationException>(() => c.GetRequiredKeyedService<ServiceImplementation>("Bla"));
            Assert.Throws<InvalidOperationException>(() => c.GetRequiredKeyedService<IService>("Bla"));

            var serviceWithKeyedParameter = c.GetService<ServiceWithKeyedParameter<ServiceImplementation>>();
            Assert.NotNull(serviceWithKeyedParameter);
            Assert.NotNull(serviceWithKeyedParameter.InnerService);
        }

        [ServiceProvider]
        [Singleton(typeof(ServiceImplementation), Name="Key")]
        [Singleton(typeof(ServiceWithKeyedParameter<ServiceImplementation>))]
        internal partial class SupportsKeyedServicesContainer
        {
        }

        internal class ServiceWithKeyedParameter<T>
        {
            public T InnerService { get; }

            public ServiceWithKeyedParameter([FromKeyedServices(typeof(string))] T innerService)
            {
                InnerService = innerService;
            }
        }
    }
}