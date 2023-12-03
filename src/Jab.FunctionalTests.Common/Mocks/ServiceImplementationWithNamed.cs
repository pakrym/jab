using Jab;

namespace JabTests;

public class ServiceImplementationWithNamed<T>: IService<T>
{
    public T InnerService { get; }
    public ServiceImplementationWithNamed([FromNamedServices("Named")] T innerService)
    {
        InnerService = innerService;
    }
}