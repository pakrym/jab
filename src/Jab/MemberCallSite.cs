namespace Jab;

internal record MemberCallSite : ServiceCallSite
{
    public ISymbol Member { get; }
    public MemberLocation MemberLocation { get; set; }

    public MemberCallSite(INamedTypeSymbol serviceType, ISymbol member, MemberLocation memberLocation, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable) : base(serviceType, serviceType, lifetime, reverseIndex, isDisposable)
    {
        Member = member;
        MemberLocation = memberLocation;
    }
}