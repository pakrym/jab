namespace Jab;

internal abstract record ServiceCallSite(ITypeSymbol ServiceType, ITypeSymbol ImplementationType, ServiceLifetime Lifetime, int ReverseIndex, bool? IsDisposable)
{
    public ITypeSymbol ServiceType { get; } = ServiceType;
    public ITypeSymbol ImplementationType { get; } = ImplementationType;
    public ServiceLifetime Lifetime { get; } = Lifetime;
    public int ReverseIndex { get; } = ReverseIndex;
    public bool? IsDisposable { get; } = IsDisposable;
    public bool IsMainImplementation => ReverseIndex == 0;
}