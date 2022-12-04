using System;
using System.Collections.Generic;
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

        [ServiceProvider]
        internal partial class CanResolveIServiceScopeFactoryContainer
        {
        }

        [Fact]
        public void CanCreateScopeUsingExtensionMethod()
        {
            CanCreateScopeUsingExtensionMethodContainer c = new();
            var scope = ((IServiceProvider)c).CreateScope();
            Assert.IsType<CanCreateScopeUsingExtensionMethodContainer.Scope>(scope);
        }

        [ServiceProvider]
        internal partial class CanCreateScopeUsingExtensionMethodContainer
        {
        }

#if NET6_0_OR_GREATER
        [Fact]
        public void CanUseIsService()
        {
            CanUseIsServiceContainer c = new();
            IServiceProviderIsService iss = c;
            
            Assert.True(iss.IsService(typeof(IServiceProvider)));
            Assert.True(iss.IsService(typeof(IServiceProviderIsService)));
            Assert.True(iss.IsService(typeof(IServiceScopeFactory)));
            Assert.True(iss.IsService(typeof(IService)));
            Assert.False(iss.IsService(typeof(IAnotherService)));
        }

        [ServiceProvider(RootServices = new[] {typeof(IEnumerable<IService>)})]
        [Transient(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanUseIsServiceContainer
        {
        }

        [Fact]
        public void CanResolveIsService()
        {
            CanUseIsServiceContainer c = new();
            
            Assert.True(c.GetService<IServiceProviderIsService>().IsService(typeof(IServiceProvider)));
            Assert.Same(c, c.CreateScope().GetService<IServiceProviderIsService>());
            Assert.True(c.CreateScope().GetService<IServiceProviderIsService>().IsService(typeof(IServiceProvider)));
        }

        [ServiceProvider()]
        internal partial class CanResolveIsServiceContainer
        {
        }
#endif
    }
}
