using Zenvin.Services.Core;

namespace Zenvin.Services.Unity.Bootstrapping
{
	public abstract class SceneBootstrapper : Bootstrapper
	{
		private protected sealed override bool CanExecute => ServiceLocator.Initialized;


		protected abstract IScopeKey GetScopeKey ();


		private protected sealed override void Initialize (BatchedModuleCollection modules)
		{
			var key = GetScopeKey ();
			if (key == null)
				return;

			ServiceLocator.AddScope (key, builder => BuildScopeCallback (builder, modules));
		}

		private void BuildScopeCallback (ServiceScopeBuilder builder, BatchedModuleCollection modules)
		{
			foreach (var module in modules.IterateFlat ())
			{
				module.Execute (builder);
			}
		}
	}
}
