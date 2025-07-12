namespace Zenvin.Services.Core
{
	partial class ServiceLocator
	{
		public readonly ref struct FluentConfigurator
		{
			private readonly ServiceLocator loc;

			public readonly bool WasInitialized;


			internal FluentConfigurator (ServiceLocator loc, bool initSuccess)
			{
				this.loc = loc;
				WasInitialized = initSuccess;
			}


			public readonly FluentConfigurator WithScopeContextProvider (IScopeContextProvider provider)
			{
				loc.scopeContextProvider = provider;
				return this;
			}


			/// <inheritdoc/>
			public static implicit operator bool (FluentConfigurator config)
			{
				return config.WasInitialized;
			}
		}
	}
}
