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
	}
}
