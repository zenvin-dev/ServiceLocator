using System;
using Zenvin.Services.Core;

namespace Zenvin.Services.Providers
{
	internal sealed class ReferenceProvider : IServiceInstanceProvider
	{
		private readonly object instance;


		public ReferenceProvider (object instance)
		{
			this.instance = instance;
		}


		bool IServiceInstanceProvider.IsValid => instance != null;

		object IServiceInstanceProvider.Get () => instance;
		
		void IServiceInstanceProvider.Initialize (IScopeKey scope)
		{
			if (!(instance is IInitializable init))
				return;

			try
			{
				init.Initialize (scope);
			}
			catch
			{ 
				// Do nothing
			}
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


		public Type GetInstanceType ()
		{
			return instance?.GetType ();
		}

		public override string ToString ()
		{
			return instance?.ToString () ?? "Missing reference.";
		}
	}
}
