using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ErrorCallSite : ServiceCallSite
    {
        public ErrorCallSite(INamedTypeSymbol serviceType, Diagnostic diagnostic) : base(serviceType, serviceType, ServiceLifetime.Transient, reverseIndex: 0, isDisposable: false)
        {
            Diagnostic = diagnostic;
        }

        public Diagnostic Diagnostic { get; }
    }
}