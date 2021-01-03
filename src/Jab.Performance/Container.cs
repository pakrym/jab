namespace Jab.Performance
{
    [ServiceProvider]
    [Transient(typeof(IService), typeof(Service))]
    internal partial class Container
    {
    }
}