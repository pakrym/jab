using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record CompositionRootKnownAttribute: KnownAttribute
    {
        public CompositionRootKnownAttribute(AttributeData attributeData) : base(attributeData)
        {
        }

        public const string MetadataName = "Jab.CompositionRootAttribute";
    }
}