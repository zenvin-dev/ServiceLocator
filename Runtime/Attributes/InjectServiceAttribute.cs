using System;

namespace Zenvin.Services.Attributes
{
	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class InjectServiceAttribute : Attribute
	{ 
		public bool Required { get; private set; }
		public Type ContractType { get; private set; }


		public InjectServiceAttribute (bool required = true, Type contractType = null)
		{
			Required = required;
			ContractType = contractType;
		}
	}
}
