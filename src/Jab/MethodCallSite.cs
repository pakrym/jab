namespace Jab;

internal record MethodCallSite : ServiceCallSite
{
    public ISymbol Member { get; }
    public bool IsScopeMember { get; set; }

    public ServiceCallSite[] Parameters { get; }
    public KeyValuePair<IParameterSymbol, ServiceCallSite>[] OptionalParameters { get; }

    public MethodCallSite(INamedTypeSymbol serviceType, ISymbol member, bool isScopeMember, ServiceCallSite[] parameters, KeyValuePair<IParameterSymbol, ServiceCallSite>[] optionalParameters, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable) : base(serviceType, serviceType, lifetime, reverseIndex, isDisposable)
    {
        Member = member;
        IsScopeMember = isScopeMember;

        Parameters = parameters;
        OptionalParameters = optionalParameters;
    }
}