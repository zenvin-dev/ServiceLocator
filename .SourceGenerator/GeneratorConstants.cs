namespace Zenvin.Services.SourceGenerator
{
	internal static class GeneratorConstants
	{
		public const string InjectServicesMethodName = "InjectServices";
		public const string GetMethodName = "Get";
		public const string RequiredArgumentName = "required";
		public const string AttrArgNameContractType = "ContractType";
		public const string AttrArgNameRequired = "Required";

		public const string ClassAttributeName = "Zenvin.Services.Attributes.ServiceDependencyAttribute";
		public const string MemberAttributeName = "Zenvin.Services.Attributes.InjectServiceAttribute";
		public const string ServiceLocatorType = "Zenvin.Services.ServiceLocator";
	}
}
