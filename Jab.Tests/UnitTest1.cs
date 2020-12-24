using System;
using Xunit;

namespace Jab.Tests
{
    [CompositionRoot]
    [Transient(typeof(IService), typeof(ServiceImplementation))]
    internal partial class Container
    {

    }

    internal interface IService
    {
        void M();
    }

    internal class ServiceImplementation : IService
    {
        public void M()
        {
        }
    }

    public class UnitTest1
    {
        [Fact]
        public void CanCreateTransientService()
        {
            Container c = new Container();
            Assert.IsType<ServiceImplementation>(c.GetContainer());
        }
    }
}