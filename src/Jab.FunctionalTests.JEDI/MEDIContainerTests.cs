using System;
using Jab.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Jab.FunctionalTests.MEDI
{
    public partial class JEDIContainerTests
    {
        [Fact]
        public void CanResolveIServiceScopeFactory()
        {
            EmptyServiceProvider c = new();
            var factory = c.GetService<IServiceScopeFactory>();

            var scope = factory.CreateScope();
            var scopeProvider = scope.ServiceProvider.GetService<IServiceScopeFactory>();

            Assert.IsType<EmptyServiceProvider.Scope>(scope);
            Assert.Same(factory, c);
            Assert.Same(scopeProvider, c);
            Assert.Same(scope, scope.ServiceProvider);
        }

        [ServiceProvider]
        internal partial class EmptyServiceProvider
        {
        }

        [Fact]
        public void CanResolveServiceCollectionService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IService, ServiceImplementation>();
            EmptyServiceProvider c = new(serviceCollection);
            var service = c.GetService<IService>();
            Assert.NotNull(service);
        }

        [Fact]
        public void CanResolveIServiceProviderIndirectly()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IService<>), typeof(ServiceImplementation<>));

            EmptyServiceProvider c = new(serviceCollection);
            var serviceProvider = c.GetRequiredService<IService<IServiceProvider>>().InnerService;
            Assert.Equal(c, serviceProvider);
        }

        [Fact]
        public void CanResolveIServiceScopeFactoryIndirectly()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IService<>), typeof(ServiceImplementation<>));

            EmptyServiceProvider c = new(serviceCollection);
            var serviceProvider = c.GetRequiredService<IService<IServiceScopeFactory>>().InnerService;
            Assert.Equal(c, serviceProvider);
        }
    }
}