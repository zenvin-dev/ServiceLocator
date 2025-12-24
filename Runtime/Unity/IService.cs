using System;

namespace Zenvin.Services.Unity
{
	public interface IService
	{
		Type ContractType => GetType ();
	}
}
