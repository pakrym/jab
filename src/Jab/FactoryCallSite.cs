namespace Jab;

internal record FactoryCallSite : ServiceCallSite
{
    public ISymbol Member { get; }
    public MemberLocation MemberLocation { get; set; }

    public ServiceCallSite[] Parameters { get; }
    public KeyValuePair<IParameterSymbol, ServiceCallSite>[] OptionalParameters { get; }

    public FactoryCallSite(INamedTypeSymbol serviceType, ISymbol member, MemberLocation memberLocation, ServiceCallSite[] parameters, KeyValuePair<IParameterSymbol, ServiceCallSite>[] optionalParameters, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable) : base(serviceType, serviceType, lifetime, reverseIndex, isDisposable)
    {
        Member = member;
        MemberLocation = memberLocation;

        Parameters = parameters;
        OptionalParameters = optionalParameters;
    }
}