using Zenvin.Services.Utility;

namespace Zenvin.Services.Core
{
	partial class ServiceLocator
	{
		public class Initializer
		{
			private BuildServiceScopeCallback callback;
			private IScopeContextProvider contextProvider;


			internal Initializer () { }


			public Initializer WithGlobalScopeCallback (BuildServiceScopeCallback callback)
			{
				this.callback = callback;
				return this;
			}

			public Initializer WithScopeContextProvider (IScopeContextProvider contextProvider)
			{
				this.contextProvider = contextProvider;
				return this;
			}


			public bool Execute ()
			{
				if (!LogAlreadyInitialized (logger))
					return false;

				loc = new ServiceLocator
				{
					logger = logger,
					scopeContextProvider = contextProvider
				};
				loc.InitializeInternal (callback);
				return true;
			}
		}
	}
}
