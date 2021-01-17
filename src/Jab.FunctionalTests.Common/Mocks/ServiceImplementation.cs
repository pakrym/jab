namespace Jab.Tests
{
    internal class ServiceImplementation : IService, IService1, IService2, IService3
    {
    }

    internal class ServiceImplementation<T> : IService<T>
    {
        public ServiceImplementation(T innerService)
        {
            InnerService = innerService;
        }

        public T InnerService { get; }
    }
}