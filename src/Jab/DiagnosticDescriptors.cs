using Microsoft.CodeAnalysis;

namespace Jab
{
    #pragma warning disable RS2000
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor UnexpectedErrorDescriptor = new DiagnosticDescriptor("JAB0001",
            "Unexpected error during generation",
            "Unexpected error occurred during code generation: {0}", "Usage", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor ServiceRequiredToConstructNotRegistered = new DiagnosticDescriptor("JAB0002",
            "The service registration not found",
            "The service '{0}' required to construct '{1}' is not registered", "Usage", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor MemberReferencedByInstanceOrFactoryAttributeNotFound = new DiagnosticDescriptor("JAB0003",
            "A member referenced by instance or factory attribute not found",
            "Unable to find a member '{0}', referenced by '{1}' attribute parameter", "Usage", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor MemberReferencedByInstanceOrFactoryAttributeAmbiguous = new DiagnosticDescriptor("JAB0004",
            "Found multiple members referenced by instance or factory attribute",
            "Found multiple members with the '{0}' name, referenced in the '{1}' attribute parameter", "Usage", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor ServiceProviderTypeHasToBePartial = new DiagnosticDescriptor("JAB0005",
            "The type marked with the ServiceProvider attribute has to be marked partial",
            "The type '{0}' marked with the ServiceProvider attribute has to be marked partial", "Usage", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor ImportedTypeNotMarkedWithModuleAttribute = new DiagnosticDescriptor("JAB0006",
            "The imported type has to be marked with the ServiceProviderModule attribute",
            "The imported type '{0}' has to be marked with the '{1}'", "Usage", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor ImplementationTypeRequiresPublicConstructor = new DiagnosticDescriptor("JAB0007",
            "The implementation type is required to have at least one public constructor",
            "The implementation type '{0}' is required to have at least one public constructor", "Usage", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor CyclicDependencyDetected = new DiagnosticDescriptor("JAB0008",
            "A cyclic dependency detected for a service",
            "A cyclic dependency detected for a service '{0}', dependency chain: '{1}'", "Usage", DiagnosticSeverity.Error, true);


    }
}