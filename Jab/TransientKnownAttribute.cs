using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record TransientKnownAttribute : KnownAttribute
    {
        public const string MetadataName = "Jab.TransientAttribute";

        public INamedTypeSymbol ServiceType { get; }
        public INamedTypeSymbol? ImplementationType { get; }

        public TransientKnownAttribute(AttributeData attributeData, INamedTypeSymbol serviceType, INamedTypeSymbol? implementationType) : base(attributeData)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }
    }
}