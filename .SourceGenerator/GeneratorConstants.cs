namespace Zenvin.Services.SourceGenerator
{
	internal static class GeneratorConstants
	{
		public const string InjectServicesMethodName = "InjectServices__";
		public const string GetMethodName = "Get";
		public const string RequiredArgumentName = "required";
		public const string AttrArgNameContractType = "ContractType";
		public const string AttrArgNameRequired = "Required";

		public const string ClassAttributeName = "InjectServices";
		public const string ClassAttributeFullName = "Zenvin.Services.Attributes.InjectServicesAttribute";
		public const string MemberAttributeFullName = "Zenvin.Services.Attributes.InjectServiceAttribute";
		public const string ServiceLocatorType = "Zenvin.Services.Core.ServiceLocator";
	}
}
