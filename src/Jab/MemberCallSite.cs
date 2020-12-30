using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record MemberCallSite : ServiceCallSite
    {
        public ISymbol Member { get; }

        public MemberCallSite(INamedTypeSymbol serviceType, ISymbol member, bool singleton, int reverseIndex) : base(serviceType, serviceType, singleton, reverseIndex)
        {
            Member = member;
        }
    }
}