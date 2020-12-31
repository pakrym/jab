namespace Jab.Tests
{
    internal interface IService
    {
    }
    internal interface IService<T>
    {
        T InnerService { get; }
    }
}