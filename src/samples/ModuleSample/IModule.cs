using System;
using Jab;

namespace ModuleSample
{
    [ServiceProviderModule]
    [Singleton(typeof(ServiceDefinedInAModule))]
    public interface IModule
    {
    }
}
