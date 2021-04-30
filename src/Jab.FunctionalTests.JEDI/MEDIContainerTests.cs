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
            CanResolveIServiceScopeFactoryContainer c = new();
            var factory = c.GetService<IServiceScopeFactory>();

            var scope = factory.CreateScope();
            var scopeProvider = scope.ServiceProvider.GetService<IServiceScopeFactory>();

            Assert.IsType<CanResolveIServiceScopeFactoryContainer.Scope>(scope);
            Assert.Same(factory, c);
            Assert.Same(scopeProvider, c);
            Assert.Same(scope, scope.ServiceProvider);
        }

        [ServiceProvider]
        internal partial class CanResolveIServiceScopeFactoryContainer
        {
        }

        [Fact]
        public void CanResolveServiceCollectionService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IService, ServiceImplementation>();
            CanResolveServiceCollectionServiceContainer c = new(serviceCollection);
            var service = c.GetService<IService>();
            Assert.NotNull(service);
        }

        [ServiceProvider]
        internal partial class CanResolveServiceCollectionServiceContainer
        {
        }
    }
}