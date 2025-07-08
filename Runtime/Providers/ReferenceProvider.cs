using System;
using Zenvin.Services.Core;
using IServiceProvider = Zenvin.Services.Core.IServiceProvider;

namespace Zenvin.Services.Providers
{
	internal sealed class ReferenceProvider : IServiceProvider
	{
		private readonly object instance;


		public ReferenceProvider (object instance)
		{
			this.instance = instance;
		}


		bool IServiceProvider.IsValid => instance != null;

		object IServiceProvider.Get () => instance;
		
		void IServiceProvider.Initialize (IScopeKey scope)
		{
			if (!(instance is IInitializable init))
				return;

			init.Initialize (scope);
		}

		void IDisposable.Dispose ()
		{
			if (!(instance is IDisposable disp))
				return;

			try
			{
				disp.Dispose ();
			}
			catch
			{
				// Do nothing
			}
		}

		public override string ToString ()
		{
			return instance?.ToString () ?? "Missing reference.";
		}
	}
}
