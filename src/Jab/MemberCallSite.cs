using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record MemberCallSite : ServiceCallSite
    {
        public ISymbol Member { get; }

        public MemberCallSite(INamedTypeSymbol serviceType, ISymbol member, bool singleton) : base(serviceType, serviceType, singleton)
        {
            Member = member;
        }
    }
}