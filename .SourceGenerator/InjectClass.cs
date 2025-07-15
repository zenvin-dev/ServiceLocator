using Microsoft.CodeAnalysis;

namespace Zenvin.Services.SourceGenerator
{
	internal readonly struct InjectClass
	{
		public readonly ITypeSymbol Class;
		public readonly AttributeData Attribute;
		public readonly InjectMember[] Members;


		public bool IsEmpty => Class == null || Members == null || Members.Length == 0;


		public InjectClass (ITypeSymbol @class, AttributeData attribute, InjectMember[] members)
		{
			Class = @class;
			Attribute = attribute;
			Members = members;
		}
	}
}
