using System;

namespace Zenvin.Services.Unity
{
	public interface IService
	{
		Type ContractType => GetType ();
	}

	public interface IService<T> : IService
	{
		Type IService.ContractType => typeof (T);
	}
}
