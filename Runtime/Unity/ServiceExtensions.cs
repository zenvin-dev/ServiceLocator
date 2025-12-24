using UnityEngine;
using Zenvin.Services.Core;

namespace Zenvin.Services.Unity
{
	public static class ServiceExtensions
	{
		public static ServiceScopeBuilder RegisterServiceInstance<TInstance> (this ServiceScopeBuilder builder, TInstance obj)
		{
			if (obj == null)
				return builder;

			var contractType = obj is IService service ? service.ContractType : obj.GetType ();
			if (contractType != null)
				builder.RegisterInstance (contractType, obj);

			return builder;
		}

		public static ServiceScopeBuilder RegisterPrefabByProxy (this ServiceScopeBuilder builder, GameObject obj)
		{
			if (obj == null)
				return builder;

			if (obj.TryGetComponent (out ServiceProxyComponent proxy))
				return proxy.RegisterInstance (builder);

			Debug.Log ($"[Service Proxy] Cannot register object '{obj}'.");
			return builder;
		}
	}
}
