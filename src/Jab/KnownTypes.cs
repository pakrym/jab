namespace Jab;

internal class KnownTypes
{
    public const string TransientAttributeShortName = "Transient";
    public const string SingletonAttributeShortName = "Singleton";
    public const string ScopedAttributeShortName = "Scoped";
    public const string CompositionRootAttributeShortName = "ServiceProvider";
    public const string ServiceProviderModuleAttributeShortName = "ServiceProviderModule";
    public const string ImportAttributeShortName = "Import";
    public const string ServiceNameAttributeShortName = "ServiceName";

    public const string TransientAttributeTypeName = $"{TransientAttributeShortName}Attribute";
    public const string SingletonAttributeTypeName = $"{SingletonAttributeShortName}Attribute";
    public const string ScopedAttributeTypeName = $"{ScopedAttributeShortName}Attribute";
    public const string CompositionRootAttributeTypeName = $"{CompositionRootAttributeShortName}Attribute";
    public const string ServiceProviderModuleAttributeTypeName = $"{ServiceProviderModuleAttributeShortName}Attribute";

    public const string ImportAttributeTypeName = $"{ImportAttributeShortName}Attribute";
    public const string ServiceNameAttributeName = $"{ServiceNameAttributeShortName}Attribute";

    public const string TransientAttributeMetadataName = $"Jab.{TransientAttributeTypeName}";
    public const string GenericTransientAttributeMetadataName = $"Jab.{TransientAttributeTypeName}`1";
    public const string Generic2TransientAttributeMetadataName = $"Jab.{TransientAttributeTypeName}`2";

    public const string SingletonAttributeMetadataName = $"Jab.{SingletonAttributeTypeName}";
    public const string GenericSingletonAttributeMetadataName = $"Jab.{SingletonAttributeTypeName}`1";
    public const string Generic2SingletonAttributeMetadataName = $"Jab.{SingletonAttributeTypeName}`2";


    public const string ScopedAttributeMetadataName = $"Jab.{ScopedAttributeTypeName}";
    public const string GenericScopedAttributeMetadataName = $"Jab.{ScopedAttributeTypeName}`1";
    public const string Generic2ScopedAttributeMetadataName = $"Jab.{ScopedAttributeTypeName}`2";

    public const string CompositionRootAttributeMetadataName = $"Jab.{CompositionRootAttributeTypeName}";
    public const string ServiceProviderModuleAttributeMetadataName = $"Jab.{ServiceProviderModuleAttributeTypeName}";

    public const string ImportAttributeMetadataName = $"Jab.{ImportAttributeTypeName}";
    public const string GenericImportAttributeMetadataName = $"Jab.{ImportAttributeTypeName}`1";

    public const string NameAttributePropertyName = "Name";
    public const string InstanceAttributePropertyName = "Instance";
    public const string FactoryAttributePropertyName = "Factory";
    public const string RootServicesAttributePropertyName = "RootServices";

    private const string IAsyncDisposableMetadataName = "System.IAsyncDisposable";
    private const string IEnumerableMetadataName = "System.Collections.Generic.IEnumerable`1";
    private const string IServiceProviderMetadataName = "System.IServiceProvider";
    private const string IServiceScopeMetadataName = "Microsoft.Extensions.DependencyInjection.IServiceScope";
    private const string ServiceNameAttributeMetadataName = $"Jab.{ServiceNameAttributeName}`1";

    private const string IServiceScopeFactoryMetadataName =
        "Microsoft.Extensions.DependencyInjection.IServiceScopeFactory";

    public INamedTypeSymbol IEnumerableType { get; }
    public INamedTypeSymbol IServiceProviderType { get; }
    public INamedTypeSymbol CompositionRootAttributeType { get; }
    public INamedTypeSymbol TransientAttributeType { get; }
    public INamedTypeSymbol? GenericTransientAttributeType { get; }
    public INamedTypeSymbol? Generic2TransientAttributeType { get; }

    public INamedTypeSymbol SingletonAttribute { get; }
    public INamedTypeSymbol? GenericSingletonAttribute { get; }
    public INamedTypeSymbol? Generic2SingletonAttribute { get; }

    public INamedTypeSymbol ImportAttribute { get; }
    public INamedTypeSymbol? GenericImportAttribute { get; }

    public INamedTypeSymbol ModuleAttribute { get; }
    public INamedTypeSymbol ScopedAttribute { get; }
    public INamedTypeSymbol? GenericScopedAttribute { get; }
    public INamedTypeSymbol? Generic2ScopedAttribute { get; }
    public INamedTypeSymbol? IAsyncDisposableType { get; }
    public INamedTypeSymbol? IServiceScopeType { get; }
    public INamedTypeSymbol? IServiceScopeFactoryType { get; }
    public INamedTypeSymbol? ServiceNameAttribute { get; }

    public KnownTypes(Compilation compilation, IAssemblySymbol assemblySymbol)
    {
        static INamedTypeSymbol GetTypeByMetadataNameOrThrow(IAssemblySymbol assemblySymbol,
            string fullyQualifiedMetadataName) =>
            assemblySymbol.GetTypeByMetadataName(fullyQualifiedMetadataName)
            ?? throw new InvalidOperationException($"Type with metadata '{fullyQualifiedMetadataName}' not found");

        static INamedTypeSymbol GetTypeFromCompilationByMetadataNameOrThrow(Compilation compilation,
            string fullyQualifiedMetadataName) =>
            compilation.GetTypeByMetadataName(fullyQualifiedMetadataName)
            ?? throw new InvalidOperationException($"Type with metadata '{fullyQualifiedMetadataName}' not found");

        IEnumerableType = GetTypeFromCompilationByMetadataNameOrThrow(compilation, IEnumerableMetadataName);
        IServiceProviderType = GetTypeFromCompilationByMetadataNameOrThrow(compilation, IServiceProviderMetadataName);
        IServiceScopeType = compilation.GetTypeByMetadataName(IServiceScopeMetadataName);
        IAsyncDisposableType = compilation.GetTypeByMetadataName(IAsyncDisposableMetadataName);
        IServiceScopeFactoryType = compilation.GetTypeByMetadataName(IServiceScopeFactoryMetadataName);

        CompositionRootAttributeType =
            GetTypeByMetadataNameOrThrow(assemblySymbol, CompositionRootAttributeMetadataName);

        TransientAttributeType = GetTypeByMetadataNameOrThrow(assemblySymbol, TransientAttributeMetadataName);
        GenericTransientAttributeType = assemblySymbol.GetTypeByMetadataName(GenericTransientAttributeMetadataName);
        Generic2TransientAttributeType = assemblySymbol.GetTypeByMetadataName(Generic2TransientAttributeMetadataName);

        SingletonAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, SingletonAttributeMetadataName);
        GenericSingletonAttribute = assemblySymbol.GetTypeByMetadataName(GenericSingletonAttributeMetadataName);
        Generic2SingletonAttribute = assemblySymbol.GetTypeByMetadataName(Generic2SingletonAttributeMetadataName);

        ScopedAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, ScopedAttributeMetadataName);
        GenericScopedAttribute = assemblySymbol.GetTypeByMetadataName(GenericScopedAttributeMetadataName);
        Generic2ScopedAttribute = assemblySymbol.GetTypeByMetadataName(Generic2ScopedAttributeMetadataName);

        ImportAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, ImportAttributeMetadataName);
        GenericImportAttribute = assemblySymbol.GetTypeByMetadataName(GenericImportAttributeMetadataName);

        ModuleAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, ServiceProviderModuleAttributeMetadataName);
        ServiceNameAttribute = GetTypeByMetadataNameOrThrow(assemblySymbol, ServiceNameAttributeMetadataName);
    }
}