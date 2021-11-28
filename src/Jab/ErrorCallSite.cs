namespace Jab;

internal record ErrorCallSite : ServiceCallSite
{
    public ErrorCallSite(ITypeSymbol serviceType, params Diagnostic[] diagnostic) : base(serviceType, serviceType, ServiceLifetime.Transient, ReverseIndex: 0, IsDisposable: false)
    {
        Diagnostic = diagnostic;
    }

    public Diagnostic[] Diagnostic { get; }
}