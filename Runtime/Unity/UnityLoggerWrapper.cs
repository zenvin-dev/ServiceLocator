using System;
using UnityEngine;
using Zenvin.Services.Core;
using IServiceLogger = Zenvin.Services.Utility.ILogger;

namespace Zenvin.Services.Unity
{
	internal class UnityLoggerWrapper : IServiceLogger
	{
		private const string ServiceLocatorTag = nameof (ServiceLocator);

		private readonly ILogger logger;
		private ILogger Logger => logger ?? Debug.unityLogger;

		internal UnityLoggerWrapper (ILogger logger) => this.logger = logger;

		void IServiceLogger.LogDebug (string message) => Logger?.Log (message);
		void IServiceLogger.LogError (Exception exception) => Logger?.LogException (exception);
		void IServiceLogger.LogError (string error) => Logger?.LogError (ServiceLocatorTag, error);
		void IServiceLogger.LogWarning (string warning) => Logger?.LogWarning (ServiceLocatorTag, warning);
	}
}
