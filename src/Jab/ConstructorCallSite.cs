using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Jab;

internal record ConstructorCallSite : ServiceCallSite
{
    public ConstructorCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ServiceCallSite[] parameters, KeyValuePair<IParameterSymbol, ServiceCallSite>[] optionalParameter, ServiceLifetime lifetime, int reverseIndex, bool? isDisposable)
        : base(serviceType, implementationType, lifetime, reverseIndex, isDisposable)
    {
        Parameters = parameters;
        OptionalParameter = optionalParameter;
    }

    public ServiceCallSite[] Parameters { get; }
    public KeyValuePair<IParameterSymbol, ServiceCallSite>[] OptionalParameter { get; }
}