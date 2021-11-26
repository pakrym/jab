using System;
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
            if (IsKnownAttribute(syntaxNode))
            {
                CandidateTypes.Add(GetCandidateType(syntaxNode));
            }

            else if (syntaxNode is InvocationExpressionSyntax invocationExpressionSyntax &&
                     IsGetServiceExpression(syntaxNode))
            {
                InvocationExpressions.Add(invocationExpressionSyntax);
            }
        }

        public static bool IsGetServiceExpression(SyntaxNode syntaxNode)
        {
            return syntaxNode is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name: GenericNameSyntax { Identifier: { Text: "GetService" } } } };
        }

        public static TypeDeclarationSyntax GetCandidateType(SyntaxNode syntaxNode)
        {
            if (syntaxNode is AttributeSyntax
                {
                    Parent: AttributeListSyntax
                    {
                        Parent: TypeDeclarationSyntax type
                    }
                })
            {
                return type;
            }

            throw new InvalidOperationException("Node doesn't have a candidate type");
        }

        public static bool IsKnownAttribute(SyntaxNode syntaxNode)
        {
            if (syntaxNode is AttributeSyntax
                {
                    Name: SimpleNameSyntax
                    {
                        Identifier: {} identifier
                    },
                    Parent: AttributeListSyntax
                    {
                        Parent: TypeDeclarationSyntax
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
                        return true;
                }
            }

            return false;
        }
    }
}