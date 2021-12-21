namespace Jab;

internal record MemberCallSite : ServiceCallSite
{
    public ISymbol Member { get; }
    public bool IsScopeMember { get; set; }

    public MemberCallSite(INamedTypeSymbol serviceType, ISymbol member, bool isScopeMember, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable) : base(serviceType, serviceType, lifetime, reverseIndex, isDisposable)
    {
        Member = member;
        IsScopeMember = isScopeMember;
    }
}