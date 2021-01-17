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
                    case KnownTypes.TransientAttributeShortName:
                    case KnownTypes.SingletonAttributeShortName:
                    case KnownTypes.ScopedAttributeShortName:
                    case KnownTypes.CompositionRootAttributeShortName:
                    case KnownTypes.ServiceProviderModuleAttributeShortName:
                    case KnownTypes.ImportAttributeShortName:
                    case KnownTypes.TransientAttributeTypeName:
                    case KnownTypes.SingletonAttributeTypeName:
                    case KnownTypes.ScopedAttributeTypeName:
                    case KnownTypes.CompositionRootAttributeTypeName:
                    case KnownTypes.ServiceProviderModuleAttributeTypeName:
                    case KnownTypes.ImportAttributeTypeName:
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