namespace Zenvin.Services.Utility
{
	public static class LoggerExtensions
	{
		public static T LogDebugPassing<T> (this ILogger logger, string message, T value)
		{
			logger?.LogDebug (message);
			return value;
		}

		public static T LogWarningPassing<T> (this ILogger logger, string warning, T value)
		{
			logger?.LogWarning (warning);
			return value;
		}

		public static T LogErrorPassing<T> (this ILogger logger, string error, T value)
		{
			logger?.LogWarning (error);
			return value;
		}
	}
}
