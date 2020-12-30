using System.Linq;
using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ServiceProvider
    {
        public ServiceProvider(ITypeSymbol type, ServiceCallSite[] callSites)
        {
            RootCallSites = callSites;
            Type = type;
        }

        public ITypeSymbol Type { get; }

        public ServiceCallSite[] RootCallSites { get; }
    }
}