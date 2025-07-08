using System;
using Zenvin.Services.Core;
using IServiceProvider = Zenvin.Services.Core.IServiceProvider;

namespace Zenvin.Services.Providers
{
	internal sealed class FactoryProvider<T> : IServiceProvider
	{
		private readonly Func<T> factory;


		public FactoryProvider (Func<T> factory)
		{
			this.factory = factory;
		}


		bool IServiceProvider.IsValid => factory != null;

		object IServiceProvider.Get () => factory.Invoke ();
		void IServiceProvider.Initialize (IScopeKey scope) { }
		void IDisposable.Dispose () { }

		public override string ToString ()
		{
			return $"Factory<{typeof(T).FullName}>";
		}
	}
}
