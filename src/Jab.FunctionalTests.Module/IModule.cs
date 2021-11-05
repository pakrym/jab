using Jab;

namespace JabTests
{
    [ServiceProviderModule]
    [Singleton(typeof(IModuleService), typeof(ModuleService))]
    public interface IModule
    {
    }
}