using Zenvin.Services.Core;

namespace Zenvin.Services.Tests.Implementation
{
	internal class TestScopeProvider : IScopeContextProvider
	{
		public IScopeKey CurrentKey { get; set; } = null;

		IScopeKey IScopeContextProvider.GetActiveScope ()
		{
			return CurrentKey;
		}
	}
}
