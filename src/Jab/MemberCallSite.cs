using Microsoft.CodeAnalysis;

namespace Jab;

internal record MemberCallSite : ServiceCallSite
{
    public ISymbol Member { get; }

    public MemberCallSite(INamedTypeSymbol serviceType, ISymbol member, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable) : base(serviceType, serviceType, lifetime, reverseIndex, isDisposable)
    {
        Member = member;
    }
}