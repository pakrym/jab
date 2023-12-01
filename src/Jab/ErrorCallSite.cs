namespace Jab;

internal record ErrorCallSite : ServiceCallSite
{
    public ErrorCallSite(ServiceIdentity identity, params Diagnostic[] diagnostic) : base(identity, identity.Type, ServiceLifetime.Transient, IsDisposable: false)
    {
        Diagnostic = diagnostic;
    }

    public Diagnostic[] Diagnostic { get; }
}