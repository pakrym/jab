using Microsoft.CodeAnalysis;

namespace Jab
{
    internal record KnownAttribute
    {
        protected KnownAttribute(AttributeData attributeData)
        {
            AttributeData = attributeData;
        }

        public AttributeData AttributeData { get; }
    }
}