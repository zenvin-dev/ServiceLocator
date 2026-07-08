using System.Collections.Generic;
using UnityEngine;
using Zenvin.Services.Core;

namespace Zenvin.Services.Unity
{
	public static class ServiceLocatorInitializerExtensions
	{
		public static ServiceLocator.Initializer WithUnityLogger (this ServiceLocator.Initializer init, ILogger logger = null)
		{
			var wrapper = new UnityLoggerWrapper (logger);
			return init?.WithLogger (wrapper);
		}

		public static void AddObjectsToScope (this ServiceScopeBuilder builder, IEnumerable<object> services, bool allowDirectServiceInterface, string source = null)
		{
			foreach (var obj in services)
			{
				if (!builder.AddObjectToScope (obj, allowDirectServiceInterface))
				{
					Debug.LogWarning ($"[Bootstrapper] Cannot register object '{obj}' ({obj.GetType ().FullName}) as Service (Source: {(source ?? "<unknown>")}).");
				}
			}
		}

		public static bool AddObjectToScope (this ServiceScopeBuilder builder, object service, bool allowDirectServiceInterface)
		{
			switch (service)
			{
				case ScriptableObject so:
					builder.RegisterServiceInstance (so);
					return true;
				case GameObject go:
					builder.RegisterPrefabByProxy (go);
					return true;
				case IService srv:
					if (allowDirectServiceInterface)
					{
						builder.RegisterInstance (srv.ContractType, srv);
						return true;
					}
					return false;
				default:
					return false;
			}
		}
	}
}
