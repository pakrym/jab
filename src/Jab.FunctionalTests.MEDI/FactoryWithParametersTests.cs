using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JabTests;
using Xunit;

using Jab;

namespace JabTests
{
    public partial class FactoryWithParametersTests
    {
        [Fact]
        public void CanUseSingletonFactory()
        {
            CanUseSingletonFactoryContainer2 c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Equal(1, c.FactoryInvocationCount);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Singleton(typeof(IAnotherService), Factory = nameof(CreateMyIServiceInstance))]
        [Singleton(typeof(IService2), typeof(ServiceImplementation))]
        internal partial class CanUseSingletonFactoryContainer2
        {
            public int FactoryInvocationCount;
            public IAnotherService CreateMyIServiceInstance(IService2 service)
            {
                if(service == null)
                {
                    throw new ArgumentNullException(nameof(service));
                }
                FactoryInvocationCount++;
                return new AnotherServiceImplementation();
            }
        }
        [Fact]
        public void CanUseTransientFactory()
        {
            CanUseTransientFactoryContainer2 c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.NotSame(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Equal(2, c.FactoryInvocationCount);
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Transient(typeof(IAnotherService), Factory = nameof(CreateMyIServiceInstance))]
        [Transient(typeof(IService2), typeof(ServiceImplementation))]
        internal partial class CanUseTransientFactoryContainer2
        {
            public int FactoryInvocationCount;
            public IAnotherService CreateMyIServiceInstance(IService2 service)
            {
                if (service == null)
                {
                    throw new ArgumentNullException(nameof(service));
                }
                FactoryInvocationCount++;
                return new AnotherServiceImplementation();
            }
        }
        [Fact]
        public void CanUseScopedFactory()
        {
            CanUseScopedFactoryContainer2 c = new();
            var scope1 = c.CreateScope();

            var implementationWithParameter1_1 = Assert.IsType<ServiceImplementationWithParameter>(scope1.GetService<IService>());
            var implementationWithParameter1_2 = Assert.IsType<ServiceImplementationWithParameter>(scope1.GetService<IService>());
            var anotherImplementation1 = scope1.GetService<IAnotherService>();


            var scope2 = c.CreateScope();
            var implementationWithParameter2_1 = Assert.IsType<ServiceImplementationWithParameter>(scope2.GetService<IService>());
            var implementationWithParameter2_2 = Assert.IsType<ServiceImplementationWithParameter>(scope2.GetService<IService>());
            var anotherImplementation2 = scope2.GetService<IAnotherService>();

            Assert.Same(implementationWithParameter1_1, implementationWithParameter1_2);
            Assert.Same(anotherImplementation1, implementationWithParameter1_1.AnotherService);
            Assert.Same(anotherImplementation1, implementationWithParameter1_2.AnotherService);

            Assert.Same(implementationWithParameter2_1, implementationWithParameter2_2);
            Assert.NotSame(implementationWithParameter1_1, implementationWithParameter2_1);

            Assert.Same(anotherImplementation2, implementationWithParameter2_1.AnotherService);
            Assert.Same(anotherImplementation2, implementationWithParameter2_2.AnotherService);

            Assert.Equal(2, c.FactoryInvocationCount);
        }

        [ServiceProvider]
        [Scoped(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Scoped(typeof(IAnotherService), Factory = nameof(CreateMyIServiceInstance))]
        [Scoped(typeof(IService2), typeof(ServiceImplementation))]
        internal partial class CanUseScopedFactoryContainer2
        {
            public int FactoryInvocationCount;
            public IAnotherService CreateMyIServiceInstance(IService2 service)
            {
                if (service == null)
                {
                    throw new ArgumentNullException(nameof(service));
                }
                FactoryInvocationCount++;
                return new AnotherServiceImplementation();
            }
        }
    }
}
