namespace Jab;

internal record ConstructorCallSite : ServiceCallSite
{
    public ConstructorCallSite(ITypeSymbol serviceType, INamedTypeSymbol implementationType, ServiceCallSite[] parameters, KeyValuePair<IParameterSymbol, ServiceCallSite>[] optionalParameters, ServiceLifetime lifetime, int? reverseIndex, bool? isDisposable)
        : base(serviceType, implementationType, lifetime, reverseIndex, isDisposable)
    {
        Parameters = parameters;
        OptionalParameters = optionalParameters;
    }

    public ServiceCallSite[] Parameters { get; }
    public KeyValuePair<IParameterSymbol, ServiceCallSite>[] OptionalParameters { get; }
}