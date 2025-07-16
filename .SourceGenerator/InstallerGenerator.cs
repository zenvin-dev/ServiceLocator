using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using static Zenvin.Services.SourceGenerator.GeneratorConstants;

namespace Zenvin.Services.SourceGenerator
{
	[Generator]
	public class InstallerGenerator : ISourceGenerator
	{
		void ISourceGenerator.Initialize (GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications (() => new ServiceInstallerReceiver ());
		}

		void ISourceGenerator.Execute (GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is ServiceInstallerReceiver rec))
				return;

			var comp = context.Compilation;
			var targets = new Dictionary<string, InjectClass> ();
			var members = new List<InjectMember> ();
			var callBase = new HashSet<string> ();

			CollectGenerationTargets (rec, comp, targets, members);
			CollectBaseClasses (targets, callBase);
			GenerateSources (ref context, targets, callBase);
			GenerateDebugSource (context, targets);
		}


		private static void GenerateDebugSource (
			GeneratorExecutionContext context,
			Dictionary<string, InjectClass> targets)
		{
			var sb = new StringBuilder ();

			sb.AppendLine ("namespace Zenvin.Services");
			sb.AppendLine ("{");
			sb.AppendLine ("\tpublic static class SourceGeneratorStats");
			sb.AppendLine ("\t{");
			sb.AppendLine ($"\t\tpublic const int TargetClassCount = {targets.Count};");
			sb.AppendLine ($"\t\tpublic const int TargetMemberCount = {targets.Select (t => t.Value.Members.Length).Sum ()};");
			sb.AppendLine ("\t}");
			sb.AppendLine ("}");
			
			context.AddSource ("SourceGeneratorStats.g.cs", SourceText.From (sb.ToString (), Encoding.UTF8));
		}

		private static void CollectGenerationTargets (
			ServiceInstallerReceiver rec,
			Compilation comp,
			Dictionary<string, InjectClass> targets,
			List<InjectMember> members)
		{
			foreach (var classDec in rec.CandidateClasses)
			{
				var classModel = comp.GetSemanticModel (classDec.SyntaxTree);

				if (!(classModel.GetDeclaredSymbol (classDec) is ITypeSymbol classSymbol))
					continue;

				var attrData = classSymbol.GetAttributes ().FirstOrDefault (attr => attr.AttributeClass.ToDisplayString () == ClassAttributeFullName);
				if (attrData == null)
					continue;

				if (!HasUnityBaseType (classSymbol))
					continue;

				members.Clear ();
				CollectGenerationMembers (classSymbol, members);
				if (members.Count == 0)
					continue;

				var target = new InjectClass (classSymbol, attrData, members.ToArray ());
				targets[classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = target;
			}
		}

		private static void CollectGenerationMembers (
			ITypeSymbol type,
			List<InjectMember> members)
		{
			var properties = type.GetMembers ().OfType<IPropertySymbol> ();
			foreach (var prop in properties)
			{
				if (prop.IsReadOnly)
					continue;

				var attrData = prop.GetAttributes ().FirstOrDefault (attr => attr.AttributeClass.ToDisplayString () == MemberAttributeFullName);
				if (attrData == null)
					continue;

				var inject = GetInjectMemberFromSymbol (prop, prop.Type, attrData);
				members.Add (inject);
			}

			var fields = type.GetMembers ().OfType<IFieldSymbol> ();
			foreach (var field in fields)
			{
				if (field.IsReadOnly)
					continue;

				var attrData = field.GetAttributes ().FirstOrDefault (attr => attr.AttributeClass.ToDisplayString () == MemberAttributeFullName);
				if (attrData == null)
					continue;

				var inject = GetInjectMemberFromSymbol (field, field.Type, attrData);
				members.Add (inject);
			}
		}

		private static void CollectBaseClasses (
			Dictionary<string, InjectClass> targets,
			HashSet<string> callBase)
		{
			foreach (var kvp in targets)
			{
				// If target base type is not an injection target
				if (!HasInjectionTargetBaseType (targets, kvp.Value.Class, callBase))
					continue;

				// Ensure that current target calls base implementation
				callBase.Add (kvp.Key);
			}
		}

		private static InjectMember GetInjectMemberFromSymbol (
			ISymbol member,
			ITypeSymbol type,
			AttributeData attrData)
		{
			bool required = true;
			ITypeSymbol contractType = null;

			foreach (var arg in attrData.NamedArguments)
			{
				if (TryGetNamedArgument (arg, AttrArgNameRequired, ref required))
					continue;
				if (TryGetNamedArgument (arg, AttrArgNameContractType, ref contractType))
					continue;
			}
			TryAssignPositionalArgument (attrData.ConstructorArguments, 0, ref required);
			TryAssignPositionalArgument (attrData.ConstructorArguments, 1, ref contractType);

			var inject = new InjectMember (member, type, contractType, required);
			return inject;
		}

		private static void GenerateSources (
			ref GeneratorExecutionContext context,
			Dictionary<string, InjectClass> targets,
			HashSet<string> callBase)
		{
			foreach (var target in targets)
			{
				if (target.Value.IsEmpty)
					continue;

				// sb.Clear ();
				GenerateSource (ref context, target.Value, callBase.Contains (target.Key), new StringBuilder ());
			}
		}

		private static void GenerateSource (ref GeneratorExecutionContext context, InjectClass target, bool callBase, StringBuilder sb)
		{
			// Write header
			sb.AppendLine ("/// <auto-generated>");
			sb.AppendLine ("/// This code was generated automatically by Zenvin.Services.SourceGenerator.InstallerGenerator.");
			sb.AppendLine ("/// </auto-generated>");
			sb.AppendLine ();

			// Open Namespace
			var classSymbol = target.Class;
			var ns = classSymbol.ContainingNamespace.IsGlobalNamespace ? null : classSymbol.ContainingNamespace.ToDisplayString ();
			var indent = '\0';
			if (ns != null)
			{
				indent = '\t';

				sb.Append ("namespace ");
				sb.AppendLine (ns);
				sb.AppendLine ("{");
			}


			// Open Class
			sb.Append (indent);
			sb.Append ("partial class ");
			sb.AppendLine (classSymbol.Name);
			sb.Append (indent);
			sb.AppendLine ("{");


			// Open Method
			sb.Append (indent);
			sb.Append ("\tprotected ");
			if (callBase)
				sb.Append ("new ");
			sb.Append ("void ");
			sb.Append (InjectServicesMethodName);
			sb.AppendLine ("()");
			sb.Append (indent);
			sb.AppendLine ("\t{");


			// Fill Method
			GenerateMethodSource (ref target, sb, indent, callBase);


			// Close Method
			sb.Append (indent);
			sb.AppendLine ("\t}");


			// Close Class
			sb.Append (indent);
			sb.AppendLine ("}");


			// Close Namespace
			if (ns != null)
			{
				sb.AppendLine ("}");
			}

			context.AddSource ($"{classSymbol.ToDisplayString ()}_ServiceInjector.g.cs", sb.ToString ());
		}

		private static void GenerateMethodSource (ref InjectClass target, StringBuilder sb, char indent, bool callBase)
		{
			if (callBase)
			{
				sb.Append (indent);
				sb.Append ("\t\tbase.");
				sb.Append (InjectServicesMethodName);
				sb.Append ("();");
				sb.AppendLine ();
			}

			foreach (var member in target.Members)
			{
				sb.Append (indent);
				sb.Append ("\t\t");
				sb.Append (member.Member.Name);
				sb.Append (" = global::");
				sb.Append (ServiceLocatorType);
				sb.Append ('.');
				sb.Append (GetMethodName);
				sb.Append ('<');
				if (member.ContractType != null)
				{
					sb.Append (member.ContractType.ToDisplayString (SymbolDisplayFormat.FullyQualifiedFormat));
					sb.Append (", ");
				}
				sb.Append (member.Type.ToDisplayString (SymbolDisplayFormat.FullyQualifiedFormat));
				sb.Append (">(");
				sb.Append (RequiredArgumentName);
				sb.Append (": ");
				sb.Append (member.Required ? "true" : "false");
				sb.AppendLine (");");
			}
		}


		private static bool HasUnityBaseType (ITypeSymbol symbol)
		{
			if (symbol == null)
				return false;

			symbol = symbol.BaseType;
			while (symbol != null)
			{
				var name = symbol.ToDisplayString ();
				if (name == "UnityEngine.MonoBehaviour" || name == "UnityEngine.ScriptableObject")
					return true;

				symbol = symbol.BaseType;
			}

			return false;
		}

		private static bool HasInjectionTargetBaseType (Dictionary<string, InjectClass> targets, ITypeSymbol type, HashSet<string> callBase)
		{
			type = type?.BaseType;
			while (type != null)
			{
				var baseTypeKey = type.ToDisplayString (SymbolDisplayFormat.FullyQualifiedFormat);
				if (baseTypeKey != null && targets.ContainsKey (baseTypeKey))
					return true;

				type = type.BaseType;
			}
			return false;
		}

		private static bool TryGetNamedArgument<T> (KeyValuePair<string, TypedConstant> arg, string name, ref T value)
		{
			if (arg.Key != name)
				return false;
			if (!(arg.Value.Value is T val))
				return false;

			value = val;
			return true;
		}

		private static void TryAssignPositionalArgument<T> (ImmutableArray<TypedConstant> constructorArguments, int position, ref T value)
		{
			if (position < 0)
				return;
			if (constructorArguments.Length <= position)
				return;
			if (!(constructorArguments[position].Value is T val))
				return;

			value = val;
		}
	}
}
