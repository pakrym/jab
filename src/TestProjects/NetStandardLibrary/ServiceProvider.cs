using Jab;

namespace NetStandardLibrary;

[ServiceProviderModule]
interface IModule
{

}

interface IService
{

}

[ServiceProvider]
[Import(typeof(IModule))]
[Singleton(typeof(Program))]
partial class ServiceProvider
{
}