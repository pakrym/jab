using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jab
{
    internal class GetServiceSyntaxCollector : ISyntaxReceiver
    {
        public List<InvocationExpressionSyntax> InvocationExpressions { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name: { Identifier: { Text: "GetService" } } } } invocationExpressionSyntax)
            {
                InvocationExpressions.Add(invocationExpressionSyntax);
            }
        }
    }
}