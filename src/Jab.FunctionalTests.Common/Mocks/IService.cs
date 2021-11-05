using Jab;

namespace JabTests
{
    internal interface IService
    {
    }
    internal interface IService<T>
    {
        T InnerService { get; }
    }
}