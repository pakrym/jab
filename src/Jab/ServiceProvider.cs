using System.Linq;
using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ServiceProvider
    {
        public ServiceProvider(ITypeSymbol type, ServiceCallSite[] callSites, KnownTypes knownTypes)
        {
            RootCallSites = callSites;
            KnownTypes = knownTypes;
            Type = type;
        }


        public ITypeSymbol Type { get; }

        public ServiceCallSite[] RootCallSites { get; }
        public KnownTypes KnownTypes { get; }
    }
}