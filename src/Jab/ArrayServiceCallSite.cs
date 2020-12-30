using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record ArrayServiceCallSite: ServiceCallSite
    {
        public ArrayServiceCallSite(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, ITypeSymbol itemType, ServiceCallSite[] items, bool singleton)
            : base(serviceType, implementationType, singleton, 0)
        {
            ItemType = itemType;
            Items = items;
        }

        public ITypeSymbol ItemType { get; }
        public ServiceCallSite[] Items { get; }
    }
}