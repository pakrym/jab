using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jab
{
    internal class SyntaxCollector : ISyntaxReceiver
    {
        public List<InvocationExpressionSyntax> InvocationExpressions { get; } = new();
        public List<TypeDeclarationSyntax> CandidateTypes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is AttributeSyntax
            {
                Name: SimpleNameSyntax
                {
                    Identifier: {} identifier
                },
                Parent: AttributeListSyntax
                {
                    Parent: TypeDeclarationSyntax candidateType
                }
            })
            {
                switch (identifier.Text)
                {
                    case AttributeNames.TransientAttributeShortName:
                    case AttributeNames.SingletonAttributeShortName:
                    case AttributeNames.ScopedAttributeShortName:
                    case AttributeNames.CompositionRootAttributeShortName:
                    case AttributeNames.ServiceProviderModuleAttributeShortName:
                    case AttributeNames.ImportAttributeShortName:
                    case AttributeNames.TransientAttributeTypeName:
                    case AttributeNames.SingletonAttributeTypeName:
                    case AttributeNames.ScopedAttributeTypeName:
                    case AttributeNames.CompositionRootAttributeTypeName:
                    case AttributeNames.ServiceProviderModuleAttributeTypeName:
                    case AttributeNames.ImportAttributeTypeName:
                        CandidateTypes.Add(candidateType);
                        break;
                }
            }
            else if (syntaxNode is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name: GenericNameSyntax { Identifier: { Text: "GetService" } } } } invocationExpressionSyntax)
            {
                InvocationExpressions.Add(invocationExpressionSyntax);
            }
        }
    }
}