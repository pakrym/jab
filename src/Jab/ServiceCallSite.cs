using Microsoft.CodeAnalysis;

namespace Jab;

internal abstract record ServiceCallSite
{
    protected ServiceCallSite(ITypeSymbol serviceType, ITypeSymbol implementationType, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
        ReverseIndex = reverseIndex;
        IsDisposable = isDisposable;
    }

    public ITypeSymbol ServiceType { get; }
    public ITypeSymbol ImplementationType { get; }
    public ServiceLifetime Lifetime { get; }
    public int ReverseIndex { get; }
    public bool? IsDisposable { get; }
    public bool IsMainImplementation => ReverseIndex == 0;
}