using System;

namespace Zenvin.Services.Exceptions
{
	public class ServiceException : Exception
	{
		public readonly Type ContractType;
		public readonly Type InstanceType;


		public ServiceException (Type contractType, Type instanceType, string message) : base (message)
		{
			ContractType = contractType;
			InstanceType = instanceType;
		}
	}
}
