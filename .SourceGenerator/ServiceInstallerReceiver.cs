using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Zenvin.Services.SourceGenerator.GeneratorConstants;

namespace Zenvin.Services.SourceGenerator
{
	internal class ServiceInstallerReceiver : ISyntaxReceiver
	{
		public readonly List<ClassDeclarationSyntax> DependentClasses = new List<ClassDeclarationSyntax> ();


		void ISyntaxReceiver.OnVisitSyntaxNode (SyntaxNode syntaxNode)
		{
			// Node is not a class declaration
			if (!(syntaxNode is ClassDeclarationSyntax classNode))
				return;

			// Class is a nested type
			if (!(classNode.Parent is CompilationUnitSyntax) && !(classNode.Parent is NamespaceDeclarationSyntax))
				return;

			// Class is not partial
			if (!classNode.HasLeadingTrivia || !classNode.GetLeadingTrivia ().Any (trivia => trivia.IsKind (SyntaxKind.PartialKeyword)))
				return;

			// Class has no attributes
			if (classNode.AttributeLists.Count == 0)
				return;

			// Class does not have RELEVANT attribute
			if (!HasAttribute (classNode.AttributeLists, ClassAttributeName))
				return;

			DependentClasses.Add (classNode);
		}

		private static bool HasAttribute (SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
		{
			return attributeLists
			.SelectMany (al => al.Attributes)
			.Any (attr =>
			{
				var name = attr.Name.ToString ();
				return name == attributeName ||
					   name == $"{attributeName}Attribute" ||
					   name.EndsWith ($".{attributeName}") ||
					   name.EndsWith ($".{attributeName}Attribute");
			});
		}
	}
}
