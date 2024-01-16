namespace Jab;

internal record FactoryCallSite : ServiceCallSite
{
    public ISymbol Member { get; }
    public MemberLocation MemberLocation { get; set; }

    public ServiceCallSite[] Parameters { get; }
    public KeyValuePair<IParameterSymbol, ServiceCallSite>[] OptionalParameters { get; }

    public FactoryCallSite(ServiceIdentity identity, ISymbol member, MemberLocation memberLocation, ServiceCallSite[] parameters, KeyValuePair<IParameterSymbol, ServiceCallSite>[] optionalParameters, ServiceLifetime lifetime, bool? isDisposable) : base(identity, identity.Type, lifetime, isDisposable)
    {
        Member = member;
        MemberLocation = memberLocation;

        Parameters = parameters;
        OptionalParameters = optionalParameters;
    }
}