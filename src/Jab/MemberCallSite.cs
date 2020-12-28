using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record MemberCallSite : ServiceCallSite
    {
        public ISymbol Member { get; }

        public MemberCallSite(INamedTypeSymbol serviceType, ISymbol member) : base(serviceType, serviceType, true)
        {
            Member = member;
        }
    }
}