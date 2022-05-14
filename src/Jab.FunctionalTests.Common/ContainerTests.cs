using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JabTests;
using Xunit;

using Jab;

namespace JabTests
{
    public partial class ContainerTests
    {
        [Fact]
        public void CanCreateTransientService()
        {
            CanCreateTransientServiceContainer c = new();
            Assert.IsType<ServiceImplementation>(c.GetService<IService>());
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanCreateTransientServiceContainer { }

        [Fact]
        public void GetServiceForUnregisteredServiceNull()
        {
            CanCreateTransientServiceContainer c = new();
            var provider = (IServiceProvider)c;
            Assert.Null(provider.GetService(typeof(IService2)));
            Assert.Throws<InvalidOperationException>(() => c.CreateScope().GetService<IService2>());
        }

        [Fact]
        public void CanCreateTransientServiceWithConstructorParameters()
        {
            CanCreateTransientServiceWithConstructorParametersContainer c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanCreateTransientServiceWithConstructorParametersContainer { }

        [Fact]
        public void CanCreateSingleton()
        {
            CanCreateSingletonContainer c = new();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var implementationWithParameter2 = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(implementationWithParameter, implementationWithParameter2);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), typeof(ServiceImplementationWithParameter))]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanCreateSingletonContainer { }

        [Fact]
        public void CanUseSingletonInstance()
        {
            CanUseSingletonInstanceContainer c = new();
            c.MyIServiceInstance = new AnotherServiceImplementation();
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Same(c.MyIServiceInstance, anotherImplementation);
        }

        [ServiceProvider]
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
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.Same(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Equal(1, c.FactoryInvocationCount);
        }

        [ServiceProvider]
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
            var implementationWithParameter = Assert.IsType<ServiceImplementationWithParameter>(c.GetService<IService>());
            var anotherImplementation = c.GetService<IAnotherService>();

            Assert.IsType<AnotherServiceImplementation>(implementationWithParameter.AnotherService);
            Assert.NotSame(anotherImplementation, implementationWithParameter.AnotherService);
            Assert.Equal(2, c.FactoryInvocationCount);
        }

        [ServiceProvider]
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

        [Fact]
        public void CanUseScopedFactory()
        {
            CanUseScopedFactoryContainer c = new();
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
        internal partial class CanUseScopedFactoryContainer
        {
            public int FactoryInvocationCount;
            public IAnotherService CreateMyIServiceInstance()
            {
                FactoryInvocationCount++;
                return new AnotherServiceImplementation();
            }
        }

        [Fact]
        public void CanUseGenericFactory()
        {
            CanUseGenericFactoryContainer c = new();
            var service = c.GetService<IService<IService2>>();
            Assert.NotNull(service.InnerService);
            Assert.Equal(1, c.FactoryInvocationCount);
        }

        [ServiceProvider]
        [Transient(typeof(IService<>), Factory = nameof(CreateMyIServiceInstance))]
        [Transient(typeof(IService2), typeof(ServiceImplementation))]
        internal partial class CanUseGenericFactoryContainer
        {
            public int FactoryInvocationCount;
            public IService<T> CreateMyIServiceInstance<T>()
            {
                FactoryInvocationCount++;
                return new ServiceImplementation<T>(this.GetService<T>());
            }
        }

        [Fact]
        public void CanResolveIEnumerableOfTransients()
        {
            CanResolveIEnumerableOfTransientsContainer c = new();
            var enumerable = c.GetService<IEnumerable<IAnotherService>>();
            var array = Assert.IsType<IAnotherService[]>(enumerable);
            Assert.Equal(3, array.Length);
            var service1 = array[0];
            var service2 = array[1];
            var service3 = array[2];
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotNull(service3);
            Assert.NotSame(service1, service2);
            Assert.NotSame(service2, service3);
        }

        [ServiceProvider(RootServices = new[] { typeof(IEnumerable<IAnotherService>) })]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveIEnumerableOfTransientsContainer { }


        [Fact]
        public void CanResolveIEnumerableOfSingletons()
        {
            CanResolveIEnumerableOfSingletonsContainer c = new();
            var enumerable = c.GetService<IEnumerable<IAnotherService>>();
            var array = Assert.IsType<IAnotherService[]>(enumerable);
            Assert.Equal(3, array.Length);
            var service1 = array[0];
            var service2 = array[1];
            var service3 = array[2];
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotNull(service3);
            Assert.NotSame(service1, service2);
            Assert.NotSame(service2, service3);

            Assert.Same(service3, c.GetService<IAnotherService>());
            Assert.Same(enumerable, c.GetService<IEnumerable<IAnotherService>>());
        }

        [ServiceProvider(RootServices = new[] { typeof(IEnumerable<IAnotherService>) })]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveIEnumerableOfSingletonsContainer { }

        [Fact]
        public void CanResolveIEnumerableInferredFromParameter()
        {
            CanResolveIEnumerableInferredFromParameterContainer c = new();
            var enumerable = c.GetService<IEnumerable<IAnotherService>>();
            var array = Assert.IsType<IAnotherService[]>(enumerable);
            Assert.Equal(3, array.Length);
            var service1 = array[0];
            var service2 = array[1];
            var service3 = array[2];
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotNull(service3);
            Assert.NotSame(service1, service2);
            Assert.NotSame(service2, service3);
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter<IEnumerable<IAnotherService>>))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveIEnumerableInferredFromParameterContainer { }

        [Fact]
        public void CanResolveEmptyEnumerable()
        {
            CanResolveEmptyEnumerableContainer c = new();
            var enumerable = c.GetService<IEnumerable<IAnotherService>>();
            var array = Assert.IsType<IAnotherService[]>(enumerable);
            Assert.Empty(array);
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(ServiceImplementationWithParameter<IEnumerable<IAnotherService>>))]
        internal partial class CanResolveEmptyEnumerableContainer { }

        [Fact]
        public void CanResolveOpenGenericService()
        {
            CanResolveOpenGenericServiceContainer c = new();
            var service = c.GetService<IService<IAnotherService>>();
            Assert.IsType<ServiceImplementation<IAnotherService>>(service);
            Assert.IsType<AnotherServiceImplementation>(service.InnerService);
        }

        [ServiceProvider(RootServices = new[] { typeof(IService<IAnotherService>) })]
        [Transient(typeof(IService<>), typeof(ServiceImplementation<>))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveOpenGenericServiceContainer { }

        [Fact]
        public void CanResolveEnumerableOfMixedOpenGenericService()
        {
            CanResolveEnumerableOfMixedOpenGenericServiceContainer c = new();
            c.Instance = new ServiceImplementation<IAnotherService>(new AnotherServiceImplementation());

            var services = c.GetService<IEnumerable<IService<IAnotherService>>>();
            var array = Assert.IsType<IService<IAnotherService>[]>(services);
            Assert.IsType<ServiceImplementation<IAnotherService>>(array[0]);
            Assert.Same(c.Instance, array[1]);
        }

        [ServiceProvider(RootServices = new[] { typeof(IEnumerable<IService<IAnotherService>>) })]
        [Transient(typeof(IService<>), typeof(ServiceImplementation<>))]
        [Singleton(typeof(IService<IAnotherService>), Instance = nameof(Instance))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanResolveEnumerableOfMixedOpenGenericServiceContainer
        {
            public IService<IAnotherService> Instance { get; set; }
        }

        [Fact]
        public void CanInferRootServiceFromGetServiceCall()
        {
            var c = new CanInferRootServiceFromGetServiceCallContainer();
            var ss = c.GetService<IEnumerable<string>>();
            Assert.Empty(ss);
        }

        [ServiceProvider]
        internal partial class CanInferRootServiceFromGetServiceCallContainer { }

        [Fact]
        public void CanUseModules()
        {
            CanUseModulesContainer c = new();
            Assert.IsType<ServiceImplementation>(c.GetService<IService>());
        }

        [ServiceProviderModule]
        [Transient(typeof(IService), typeof(ServiceImplementation))]
        internal interface ICanUseModulesModule { }

        [ServiceProvider]
        [Import(typeof(ICanUseModulesModule))]
        internal partial class CanUseModulesContainer { }

        [Fact]
        public void CanUseModulesFromAnotherAssembly()
        {
            CanUseModulesFromAnotherAssemblyContainer c = new();
            Assert.IsType<ModuleService>(c.GetService<IModuleService>());
        }

        [ServiceProvider]
        [Import(typeof(IModule))]
        internal partial class CanUseModulesFromAnotherAssemblyContainer { }

        [Fact]
        public void CanExtendModules()
        {
            CanExtendModulesContainer c = new();
            var serviceImplementation = Assert.IsType<ServiceImplementation<IAnotherService>>(c.GetService<IService<IAnotherService>>());
            Assert.IsType<AnotherServiceImplementation>(serviceImplementation.InnerService);
        }

        [ServiceProviderModule]
        [Transient(typeof(IService<>), typeof(ServiceImplementation<>))]
        internal interface ICanExtendModulesModule { }

        [ServiceProvider]
        [Import(typeof(ICanExtendModulesModule))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal partial class CanExtendModulesContainer { }

        [Fact]
        public void CanOverrideModules()
        {
            CanOverrideModulesContainer c = new();
            var serviceImplementation = Assert.IsType<ServiceImplementation>(c.GetService<IService>());
        }

        [ServiceProviderModule]
        [Transient(typeof(IService), typeof(ServiceImplementation<IAnotherService>))]
        internal interface ICanOverrideModulesModule { }

        [ServiceProvider]
        [Import(typeof(ICanUseModulesModule))]
        [Transient(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanOverrideModulesContainer { }

        [Fact]
        public void CanChainModules()
        {
            CanChainModulesModule c = new();
            var serviceImplementation = Assert.IsType<ServiceImplementation<IAnotherService>>(c.GetService<IService<IAnotherService>>());
            Assert.IsType<AnotherServiceImplementation>(serviceImplementation.InnerService);
        }

        [ServiceProviderModule]
        [Transient(typeof(IService<>), typeof(ServiceImplementation<>))]
        internal interface ICanChainModulesModule1 { }

        [ServiceProviderModule]
        [Import(typeof(ICanChainModulesModule1))]
        [Transient(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        internal interface ICanChainModulesModule2 { }

        [ServiceProvider]
        [Import(typeof(ICanChainModulesModule2))]
        internal partial class CanChainModulesModule { }

        [Fact]
        public void CanResolveScoped()
        {
            CanResolveScopedContainer c = new();
            var scope1 = c.CreateScope();
            var service1_1 = scope1.GetService<IService>();
            var service1_2 = scope1.GetService<IService>();

            var scope2 = c.CreateScope();
            var service2_1 = scope2.GetService<IService>();
            var service2_2 = scope2.GetService<IService>();

            Assert.NotSame(scope1, scope2);
            Assert.NotSame(service1_1, service2_1);
            Assert.Same(service1_1, service1_2);
            Assert.Same(service2_1, service2_2);
        }

        [ServiceProvider]
        [Scoped(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanResolveScopedContainer { }


        [Fact]
        public void CanResolveSingletonViaScope()
        {
            CanResolveSingletonViaScopeContainer c = new();
            var scope1 = c.CreateScope();
            var service = c.GetService<IService>();
            var service1_1 = scope1.GetService<IService>();
            var service1_2 = scope1.GetService<IService>();

            Assert.Same(service1_1, service1_2);
            Assert.Same(service, service1_1);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanResolveSingletonViaScopeContainer { }

        [Fact]
        public void CanResolveMixedEnumerable()
        {
            CanResolveMixedEnumerableContainer c = new();
            var service = c.GetService<IService>();
            var scope1 = c.CreateScope();
            var services1_1 = scope1.GetService<IEnumerable<IService>>();
            var services1_2 = scope1.GetService<IEnumerable<IService>>();
            var array1_1 = Assert.IsType<IService[]>(services1_1);
            var array1_2 = Assert.IsType<IService[]>(services1_2);

            var scope2 = c.CreateScope();
            var services2 = scope2.GetService<IEnumerable<IService>>();
            var array2 = Assert.IsType<IService[]>(services2);

            Assert.NotSame(array1_1[0], array1_2[0]);
            Assert.NotSame(array1_2[0], array2[0]);

            Assert.Same(array1_1[1], array1_2[1]);
            Assert.NotSame(array1_2[1], array2[1]);

            Assert.Same(service, array1_1[2]);
            Assert.Same(array1_1[2], array1_2[2]);
            Assert.Same(array1_2[2], array2[2]);
        }

        [ServiceProvider(RootServices = new[] { typeof(IEnumerable<IService>) })]
        [Transient(typeof(IService), typeof(ServiceImplementation))]
        [Scoped(typeof(IService), typeof(ServiceImplementation))]
        [Singleton(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanResolveMixedEnumerableContainer { }

        [Fact]
        public void DisposingScopeDisposesServices()
        {
            DisposingScopeDisposesServicesContainer c = new();
            var scope = c.CreateScope();
            var service = Assert.IsType<DisposableServiceImplementation>(scope.GetService<IService>());

            scope.Dispose();

            Assert.Equal(1, service.DisposalCount);
        }

        [ServiceProvider]
        [Scoped(typeof(IService), typeof(DisposableServiceImplementation))]
        internal partial class DisposingScopeDisposesServicesContainer { }

        [Fact]
        public async Task DisposingScopeDisposesAsyncServices()
        {
            DisposingScopeDisposesAsyncServicesContainer c = new();
            var scope = c.CreateScope();
            var service = Assert.IsType<AsyncDisposableServiceImplementation>(scope.GetService<IService>());

            await scope.DisposeAsync();

            Assert.Equal(1, service.AsyncDisposalCount);
            Assert.Equal(0, service.DisposalCount);

            scope = c.CreateScope();
            service = Assert.IsType<AsyncDisposableServiceImplementation>(scope.GetService<IService>());

            scope.Dispose();

            Assert.Equal(0, service.AsyncDisposalCount);
            Assert.Equal(1, service.DisposalCount);
        }

        [ServiceProvider]
        [Scoped(typeof(IService), typeof(AsyncDisposableServiceImplementation))]
        internal partial class DisposingScopeDisposesAsyncServicesContainer { }

        [Fact]
        public void DisposingProviderDisposesRootScopedServices()
        {
            DisposingProviderDisposesRootScopedServicesContainer c = new();
            var service = Assert.IsType<DisposableServiceImplementation>(c.GetService<IService>());

            c.Dispose();

            Assert.Equal(1, service.DisposalCount);
        }

        [ServiceProvider]
        [Scoped(typeof(IService), typeof(DisposableServiceImplementation))]
        internal partial class DisposingProviderDisposesRootScopedServicesContainer { }

        [Fact]
        public void DisposingProviderDisposesRootSingletonServices()
        {
            DisposingProviderDisposesRootSingletonServicesContainer c = new();
            var service = Assert.IsType<DisposableServiceImplementation>(c.GetService<IService>());

            c.Dispose();

            Assert.Equal(1, service.DisposalCount);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), typeof(DisposableServiceImplementation))]
        internal partial class DisposingProviderDisposesRootSingletonServicesContainer { }

        [Fact]
        public async Task DisposingProviderDisposesRootSingAsyncServices()
        {
            DisposingProviderDisposesRootSingAsyncServicesContainer c = new();
            var service = Assert.IsType<AsyncDisposableServiceImplementation>(c.GetService<IService>());

            await c.DisposeAsync();

            Assert.Equal(1, service.AsyncDisposalCount);
            Assert.Equal(0, service.DisposalCount);
        }

        [ServiceProvider]
        [Scoped(typeof(IService), typeof(AsyncDisposableServiceImplementation))]
        internal partial class DisposingProviderDisposesRootSingAsyncServicesContainer { }

        [Fact]
        public async Task DisposingProviderDisposesAllSingletonEnumerableServices()
        {
            DisposingProviderDisposesAllSingletonEnumerableServicesContainer c = new();
            var services = Assert.IsType<IService[]>(c.GetService<IEnumerable<IService>>());

            await c.DisposeAsync();

            foreach (var service in services)
            {
                var disposableService = Assert.IsType<DisposableServiceImplementation>(service);
                Assert.Equal(1, disposableService.DisposalCount);
            }
        }

        [ServiceProvider]
        [Scoped(typeof(IService), typeof(DisposableServiceImplementation))]
        [Scoped(typeof(IService), typeof(DisposableServiceImplementation))]
        [Scoped(typeof(IService), typeof(DisposableServiceImplementation))]
        internal partial class DisposingProviderDisposesAllSingletonEnumerableServicesContainer { }

        [Fact]
        public void DisposingProviderDisposesTransients()
        {
            DisposingProviderDisposesTransientsContainer c = new();
            List<IService> services = new();
            for (int i = 0; i < 5; i++)
            {
                services.Add(c.GetService<IService>());
            }

            c.Dispose();

            foreach (var service in services)
            {
                var disposableService = Assert.IsType<DisposableServiceImplementation>(service);
                Assert.Equal(1, disposableService.DisposalCount);
            }
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(DisposableServiceImplementation))]
        internal partial class DisposingProviderDisposesTransientsContainer { }

        [Fact]
        public void DisposingScopeDisposesTransients()
        {
            DisposingScopeDisposesTransientsContainer c = new();
            var scope = c.CreateScope();

            List<IService> services = new();
            for (int i = 0; i < 5; i++)
            {
                services.Add(scope.GetService<IService>());
            }

            scope.Dispose();

            foreach (var service in services)
            {
                var disposableService = Assert.IsType<DisposableServiceImplementation>(service);
                Assert.Equal(1, disposableService.DisposalCount);
            }
        }

        [ServiceProvider]
        [Transient(typeof(IService), typeof(DisposableServiceImplementation))]
        internal partial class DisposingScopeDisposesTransientsContainer { }

        [Fact]
        public void DisposingProviderDisposesRootSingletonFactoryServices()
        {
            DisposingProviderDisposesRootSingletonServicesContainer c = new();
            var service = Assert.IsType<DisposableServiceImplementation>(c.GetService<IService>());

            c.Dispose();

            Assert.Equal(1, service.DisposalCount);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), Factory = nameof(CreateDisposableServiceImplementation))]
        internal partial class DisposingProviderDisposesRootSingletonFactoryServicesContainer
        {
            internal IService CreateDisposableServiceImplementation()
            {
                return new DisposableServiceImplementation();
            }
        }

        [Fact]
        public void DisposingProviderDoesNotDisposeRootSingletonInstanceServices()
        {
            DisposingProviderDoesNotDisposeRootSingletonInstanceServicesContainer c = new();
            var service = Assert.IsType<DisposableServiceImplementation>(c.GetService<IService>());

            c.Dispose();

            Assert.Equal(0, service.DisposalCount);
            Assert.Equal(0, c.DisposableServiceImplementation.DisposalCount);
        }

        [ServiceProvider]
        [Singleton(typeof(IService), Instance = nameof(DisposableServiceImplementation))]
        internal partial class DisposingProviderDoesNotDisposeRootSingletonInstanceServicesContainer
        {
            internal DisposableServiceImplementation DisposableServiceImplementation { get; } = new DisposableServiceImplementation();
        }

        [Fact]
        public void CanResolveServicesUsingIServiceProvider()
        {
            CanResolveServicesUsingIServiceProviderContainer c = new();
            IServiceProvider serviceProvider = c;
            Assert.IsType<ServiceImplementation>(serviceProvider.GetService(typeof(IService)));
            var services = Assert.IsType<IService[]>(serviceProvider.GetService(typeof(IEnumerable<IService>)));
            var service = Assert.Single(services);
            Assert.NotNull(service);
        }

        [ServiceProvider(RootServices = new[] { typeof(IEnumerable<IService>) })]
        [Transient(typeof(IService), typeof(ServiceImplementation))]
        internal partial class CanResolveServicesUsingIServiceProviderContainer { }

        [Fact]
        public void CanResolveIServiceProvider()
        {
            CanResolveIServiceProviderContainer c = new();
            var provider = c.GetService<IServiceProvider>();

            var scope = c.CreateScope();
            var scopeProvider = scope.GetService<IServiceProvider>();

            Assert.Same(provider, c);
            Assert.Same(scopeProvider, scope);
        }

        [ServiceProvider]
        internal partial class CanResolveIServiceProviderContainer { }

        [Fact]
        public void CanGetSingletonServiceFromFactory()
        {
            GetSingletonServiceFromFactoryServiceContainer c = new();
            Assert.Same(c.GetService<ServiceImplementation>(), c.GetService<IService>());
        }

        [ServiceProvider]
        [Singleton(typeof(ServiceImplementation))]
        [Singleton(typeof(IService), Factory = nameof(IServiceFactory))]
        partial class GetSingletonServiceFromFactoryServiceContainer
        {
            public IService IServiceFactory() => GetService<ServiceImplementation>();
        }

        [Fact]
        public void CanGetScopedServiceFromFactory()
        {
            GetScopedServiceFromFactoryServiceContainer c = new();
            Assert.Same(c.GetService<ServiceImplementation>(), c.GetService<IService>());

            var scope = c.CreateScope();
            Assert.Same(scope.GetService<ServiceImplementation>(), scope.GetService<IService>());
        }

        [ServiceProvider]
        [Scoped(typeof(ServiceImplementation))]
        [Scoped(typeof(IService), Factory = nameof(Scope.IServiceFactory))]
        partial class GetScopedServiceFromFactoryServiceContainer
        {
            public partial class Scope
            {
                public IService IServiceFactory() => GetService<ServiceImplementation>();
            }
        }

        [Fact]
        public void CanGetScopedServiceFromStaticFactory()
        {
            CanGetScopedServiceFromStaticFactoryContainer c = new();
            Assert.NotNull(c.GetService<IService>());

            var scope = c.CreateScope();
            Assert.NotNull(scope.GetService<IService>());
        }

        [ServiceProvider]
        [Scoped(typeof(IService), Factory = nameof(Scope.IServiceFactory))]
        partial class CanGetScopedServiceFromStaticFactoryContainer
        {
            public partial class Scope
            {
                public static IService IServiceFactory() => new ServiceImplementation();
            }
        }

        [Fact]
        public void CanGetScopedServiceFromStaticRootFactory()
        {
            CanGetScopedServiceFromStaticRootFactoryContainer c = new();
            Assert.NotNull(c.GetService<IService>());

            var scope = c.CreateScope();
            Assert.NotNull(scope.GetService<IService>());
        }

        [ServiceProvider]
        [Scoped(typeof(IService), Factory = nameof(IServiceFactory))]
        partial class CanGetScopedServiceFromStaticRootFactoryContainer
        {
            public static IService IServiceFactory() => new ServiceImplementation();
        }

        [Fact]
        public void CanGetMultipleIEnumerableSingleton()
        {
            CanGetMultipleIEnumerableServiceSingletonContainer c = new();
            Assert.NotEmpty(c.GetService<IEnumerable<IService>>());
            Assert.NotEmpty(c.GetService<IEnumerable<IService1>>());
        }

        [ServiceProvider]
        [Singleton(typeof(IService), typeof(ServiceImplementation))]
        [Singleton(typeof(IService1), typeof(ServiceImplementation))]
        partial class CanGetMultipleIEnumerableServiceSingletonContainer
        {
        }

        [Fact]
        public void CanGetMultipleIEnumerableScoped()
        {
            CanGetMultipleIEnumerableScopedContainer c = new();
            Assert.NotEmpty(c.GetService<IEnumerable<IService>>());
            Assert.NotEmpty(c.GetService<IEnumerable<IService1>>());

            var scope = c.CreateScope();
            Assert.NotEmpty(scope.GetService<IEnumerable<IService>>());
            Assert.NotEmpty(scope.GetService<IEnumerable<IService1>>());
        }

        [ServiceProvider]
        [Scoped(typeof(IService), typeof(ServiceImplementation))]
        [Scoped(typeof(IService1), typeof(ServiceImplementation))]
        partial class CanGetMultipleIEnumerableScopedContainer
        {
        }

        [Fact]
        public void CanGetMultipleOpenGenericSingleton()
        {
            CanGetMultipleOpenGenericSingletonContainer c = new();
            Assert.NotNull(c.GetService<IService<IService1>>());
            Assert.NotNull(c.GetService<IService<IAnotherService>>());
        }

        [ServiceProvider]
        [Singleton(typeof(IService<>), typeof(ServiceImplementation<>))]
        [Singleton(typeof(IService1), typeof(ServiceImplementation))]
        [Singleton(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        partial class CanGetMultipleOpenGenericSingletonContainer
        {
        }

        [Fact]
        public void CanGetMultipleOpenGenericScoped()
        {
            CanGetMultipleOpenGenericScopedContainer c = new();
            Assert.NotNull(c.GetService<IService<IService1>>());
            Assert.NotNull(c.GetService<IService<IAnotherService>>());

            var scope = c.CreateScope();
            Assert.NotNull(scope.GetService<IService<IService1>>());
            Assert.NotNull(scope.GetService<IService<IAnotherService>>());
        }

        [ServiceProvider]
        [Scoped(typeof(IService<>), typeof(ServiceImplementation<>))]
        [Scoped(typeof(IService1), typeof(ServiceImplementation))]
        [Scoped(typeof(IAnotherService), typeof(AnotherServiceImplementation))]
        partial class CanGetMultipleOpenGenericScopedContainer
        {
        }

#if JAB_PREVIEW
        [Fact]
        public void CanUseGenericAttributes()
        {
            CanUseGenericAttributesContainer c = new();
            var moduleService = c.GetService<IModuleService>();

            var singleton = c.GetService<IService>();
            var singleton2 = c.GetService<IService>();

            var transient = c.GetService<IService3>();
            var transient2 = c.GetService<IService3>();

            var scope = c.CreateScope();
            var scoped = scope.GetService<IService2>();
            var scoped2 = scope.GetService<IService2>();

            var scope2 = c.CreateScope();
            var scoped3 = scope2.GetService<IService2>();

            Assert.NotSame(transient, transient2);

            Assert.Same(singleton, singleton2);
            Assert.Same(scoped, scoped2);
            Assert.NotSame(scoped2, scoped3);

            Assert.NotNull(moduleService);
        }

        [ServiceProvider]
        [Import<IModule>]
        [Singleton<IService, ServiceImplementation>]
        [Scoped<IService2, ServiceImplementation>]
        [Transient<IService3, ServiceImplementation>]
        internal partial class CanUseGenericAttributesContainer { }
#endif
    }
}

[ServiceProvider]
internal partial class CanGenerateInGlobalNamespace { }