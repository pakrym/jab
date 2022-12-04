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
            CanCreateScopeContainer c = new();
            var scope = ((IServiceProvider)c).CreateScope();
            Assert.IsType<CanCreateScopeContainer.Scope>(scope);
        }

        [ServiceProvider]
        internal partial class CanResolveIServiceScopeFactoryContainer
        {
        }

        [ServiceProvider]
        internal partial class CanCreateScopeContainer
        {
        }
    }
}
