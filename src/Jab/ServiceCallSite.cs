namespace Jab;

internal abstract record ServiceCallSite(ServiceIdentity Identity, ITypeSymbol ImplementationType, ServiceLifetime Lifetime, bool? IsDisposable)
{
    public ServiceIdentity Identity { get; } = Identity;
    public ITypeSymbol ImplementationType { get; } = ImplementationType;
    public ServiceLifetime Lifetime { get; } = Lifetime;
    public bool? IsDisposable { get; } = IsDisposable;
}