
namespace Jab;

internal class SyntaxCollector : ISyntaxReceiver
{
    public List<InvocationExpressionSyntax> InvocationExpressions { get; } = new();
    public List<TypeDeclarationSyntax> CandidateTypes { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (IsCandidateType(syntaxNode))
        {
            CandidateTypes.Add((TypeDeclarationSyntax)syntaxNode);
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

    public static bool IsCandidateType(SyntaxNode syntax)
    {
        if (syntax is not TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return false;
        }

        foreach (var attributeList in typeDeclarationSyntax.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (IsKnownAttribute(attribute))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsKnownAttribute(SyntaxNode syntaxNode)
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