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

    internal class ServiceImplementationWithParameter<T1, T2> : IService
    {
        public T1 Parameter1 { get; }
        public T2 Parameter2 { get; }

        public ServiceImplementationWithParameter(T1 parameter1)
        {
            Parameter1 = parameter1;
        }

        public ServiceImplementationWithParameter(T1 parameter1, T2 parameter2)
        {
            Parameter1 = parameter1;
            Parameter2 = parameter2;
        }
    }

    internal class ServiceImplementationWithParameter<T1, T2, T3> : IService
    {
        public T1 Parameter1 { get; }
        public T2 Parameter2 { get; }
        public T3 Parameter3 { get; }

        public int SelectedCtor { get; }

        public ServiceImplementationWithParameter(T1 parameter1)
        {
            Parameter1 = parameter1;
            SelectedCtor = 1;
        }

        public ServiceImplementationWithParameter(T1 parameter1, T2 parameter2 = default)
        {
            Parameter1 = parameter1;
            Parameter2 = parameter2;
            SelectedCtor = 2;
        }

        public ServiceImplementationWithParameter(T1 parameter1, T2 parameter2 = default, T3 parameter3 = default)
        {
            Parameter1 = parameter1;
            Parameter2 = parameter2;
            Parameter3 = parameter3;
            SelectedCtor = 3;
        }
    }
}