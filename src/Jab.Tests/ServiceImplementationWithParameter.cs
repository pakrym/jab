namespace Jab.Tests
{
    internal class ServiceImplementationWithParameter : IService
    {
        public IAnotherService AnotherService { get; }

        public ServiceImplementationWithParameter(IAnotherService anotherService)
        {
            AnotherService = anotherService;
        }
        public void M()
        {
        }
    }
}