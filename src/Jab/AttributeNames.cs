namespace Jab
{
    internal class AttributeNames
    {
        public const string TransientAttributeShortName = "Transient";
        public const string SingletonAttributeShortName = "Singleton";
        public const string ScopedAttributeShortName = "Scoped";
        public const string CompositionRootAttributeShortName = "ServiceProvider";
        public const string ServiceProviderModuleAttributeShortName = "ServiceProviderModule";
        public const string ImportAttributeShortName = "Import";

        public const string TransientAttributeTypeName = TransientAttributeShortName + "Attribute";
        public const string SingletonAttributeTypeName = SingletonAttributeShortName + "Attribute";
        public const string ScopedAttributeTypeName = ScopedAttributeShortName + "Attribute";
        public const string CompositionRootAttributeTypeName = CompositionRootAttributeShortName + "Attribute";
        public const string ServiceProviderModuleAttributeTypeName = ServiceProviderModuleAttributeShortName + "Attribute";
        public const string ImportAttributeTypeName = ImportAttributeShortName + "Attribute";

        public const string TransientAttributeMetadataName = "Jab." + TransientAttributeTypeName;
        public const string SingletonAttributeMetadataName = "Jab." + SingletonAttributeTypeName;
        public const string ScopedAttributeMetadataName = "Jab." + ScopedAttributeTypeName;
        public const string CompositionRootAttributeMetadataName = "Jab." + CompositionRootAttributeTypeName;
        public const string ServiceProviderModuleAttributeMetadataName = "Jab." + ServiceProviderModuleAttributeTypeName;
        public const string ImportAttributeMetadataName = "Jab." + ImportAttributeTypeName;

        public const string InstanceAttributePropertyName = "Instance";
        public const string FactoryAttributePropertyName = "Factory";
        public const string RootServicesAttributePropertyName = "RootServices";

    }
}