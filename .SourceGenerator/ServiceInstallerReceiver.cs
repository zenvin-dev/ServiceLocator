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
		public readonly List<ClassDeclarationSyntax> CandidateClasses = new List<ClassDeclarationSyntax> ();


		void ISyntaxReceiver.OnVisitSyntaxNode (SyntaxNode syntaxNode)
		{
			// Node is not a class declaration
			if (!(syntaxNode is ClassDeclarationSyntax classNode))
				return;

			// Class is a nested type
			if (!(classNode.Parent is CompilationUnitSyntax) && !(classNode.Parent is NamespaceDeclarationSyntax))
				return;

			// Class is not partial
			if (!classNode.Modifiers.Any (mod => mod.IsKind (SyntaxKind.PartialKeyword)))
				return;

			// Class does not have relevant attribute
			if (!HasAttribute (classNode.AttributeLists, ClassAttributeName))
				return;

			CandidateClasses.Add (classNode);
		}

		private static bool HasAttribute (SyntaxList<AttributeListSyntax> lists, string attributeName)
		{
			if (lists.Count == 0)
				return false;

			return lists
				.SelectMany (list => list.Attributes)
				.Any (attr =>
				{
					var name = "";
					switch (attr.Name)
					{
						case IdentifierNameSyntax id:
							name = id.Identifier.Text;
							break;
						case QualifiedNameSyntax qn:
							name = qn.Right.Identifier.Text;
							break;
						case AliasQualifiedNameSyntax an:
							name = an.Name.Identifier.Text;
							break;
						default:
							name = attr.Name.ToString ();
							break;
					}
					return name == attributeName || name == $"{attributeName}Attribute";
				});
		}
	}
}
