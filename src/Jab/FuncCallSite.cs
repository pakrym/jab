namespace Jab;

internal record FuncCallSite : ServiceCallSite
{
    public FuncCallSite(ServiceIdentity identity, ServiceCallSite inner)
        : base(identity, identity.Type, GetFuncLifetime(inner.Lifetime), false)
    {
        Inner = inner;
    }
    
    public ServiceCallSite Inner { get; }

    private static ServiceLifetime GetFuncLifetime(ServiceLifetime innerLifetime) => innerLifetime switch
    {
        ServiceLifetime.Scoped => ServiceLifetime.Scoped,
        _ => ServiceLifetime.Singleton
    };
}