namespace Jab;

internal record MemberCallSite : ServiceCallSite
{
    public ISymbol Member { get; }
    public MemberLocation MemberLocation { get; set; }

    public MemberCallSite(ServiceIdentity identity, ISymbol member, MemberLocation memberLocation, ServiceLifetime lifetime, bool? isDisposable) : base(identity, identity.Type, lifetime, isDisposable)
    {
        Member = member;
        MemberLocation = memberLocation;
    }
}