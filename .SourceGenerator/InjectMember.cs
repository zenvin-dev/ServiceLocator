using Microsoft.CodeAnalysis;

namespace Zenvin.Services.SourceGenerator
{
	internal readonly struct InjectMember
	{
		public readonly ISymbol Member;
		public readonly ITypeSymbol Type;
		public readonly ITypeSymbol ContractType;
		public readonly bool Required;


		public InjectMember (ISymbol member, ITypeSymbol type, ITypeSymbol contractType, bool required)
		{
			Member = member;
			ContractType = contractType;
			Type = type;
			Required = required;
		}
	}
}
