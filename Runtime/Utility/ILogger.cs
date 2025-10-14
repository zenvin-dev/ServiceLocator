using System;

namespace Zenvin.Services.Utility
{
	public interface ILogger
	{
		void LogDebug (string message);
		void LogWarning (string warning);
		void LogError (Exception exception);
		void LogError (string error);
	}
}
