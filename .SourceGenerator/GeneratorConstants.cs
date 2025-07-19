namespace Zenvin.Services.SourceGenerator
{
	internal static class GeneratorConstants
	{
		public static class InjectionConstants
		{
			public const string MethodName = "InjectServices__";
			
			public const string ScopeParameterType = "Zenvin.Services.Core.IScopeKey";
			public const string ScopeParameterName = "scope";
			public const string ScopeParameterDefault = "null";

			public const string FallbackParameterType = "System.Boolean";
			public const string FallbackParameterName = "fallbackToGlobal";
			public const string FallbackParameterDefault = "true";
		}

		public static class IntegrationConstants
		{
			public const string ServiceLocatorType = "Zenvin.Services.Core.ServiceLocator";
			public const string GetMethodName = "Get";
			public const string RequiredArgumentName = "required";
			public const string ScopeArgumentName = "scope";
			public const string FallbackArgumentName = "fallbackToGlobalScope";
		}

		public static class AnalysisConstants
		{
			public const string ClassAttributeName = "InjectServices";
			public const string ClassAttributeFullName = "Zenvin.Services.Attributes.InjectServicesAttribute";
			public const string MemberAttributeFullName = "Zenvin.Services.Attributes.InjectServiceAttribute";
			public const string AttributeParameterNameContractType = "ContractType";
			public const string AttributeParameterNameRequired = "Required";
		}
	}
}
