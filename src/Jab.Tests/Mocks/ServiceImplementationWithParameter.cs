namespace Jab.Tests
{
    internal class ServiceImplementationWithParameter : ServiceImplementationWithParameter<IAnotherService>
    {
        public ServiceImplementationWithParameter(IAnotherService anotherService) : base(anotherService)
        {
        }
    }

    internal class ServiceImplementationWithParameter<T> : IService
    {

        public T AnotherService { get; }

        public ServiceImplementationWithParameter(T anotherService)
        {
            AnotherService = anotherService;
        }
        public void M()
        {
        }
    }
}