namespace Jab;

internal record ConstructorCallSite : ServiceCallSite
{
    public ConstructorCallSite(ServiceIdentity identity, INamedTypeSymbol implementationType, ServiceCallSite[] parameters, KeyValuePair<IParameterSymbol, ServiceCallSite>[] optionalParameters, ServiceLifetime lifetime, bool? isDisposable)
        : base(identity, implementationType, lifetime, isDisposable)
    {
        Parameters = parameters;
        OptionalParameters = optionalParameters;
    }

    public ServiceCallSite[] Parameters { get; }
    public KeyValuePair<IParameterSymbol, ServiceCallSite>[] OptionalParameters { get; }
}