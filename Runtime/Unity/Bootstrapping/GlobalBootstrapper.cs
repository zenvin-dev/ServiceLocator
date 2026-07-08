using System.Runtime.CompilerServices;
using UnityEngine;
using Zenvin.Services.Core;

namespace Zenvin.Services.Unity.Bootstrapping
{
	public class GlobalBootstrapper : Bootstrapper
	{
		private protected sealed override bool CanExecute => !ServiceLocator.Initialized;


		protected virtual IScopeContextProvider GetScopeContextProvider () => null;


		private protected sealed override void Initialize (BatchedModuleCollection modules)
		{
			ServiceLocator.GetInitializer ()
				.WithGlobalScopeCallback ((builder) => BuildGlobalScope (builder, modules))
				.WithScopeContextProvider (GetScopeContextProvider ())
				.Execute ();

			HandleEditorPlaymodeEnd ();
		}


		[System.Diagnostics.Conditional ("UNITY_EDITOR")]
		private void HandleEditorPlaymodeEnd () => Application.quitting += ApplicationQuittingHandler;

		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		private void ApplicationQuittingHandler () => ServiceLocator.Dispose ();


		private static void BuildGlobalScope (ServiceScopeBuilder builder, BatchedModuleCollection modules)
		{
			foreach (var module in modules.IterateFlat ())
			{
				module.Execute (builder);
			}
		}
	}
}
