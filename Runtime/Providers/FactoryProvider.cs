using System;
using Zenvin.Services.Core;

namespace Zenvin.Services.Providers
{
	internal abstract class FactoryProvider { }

	internal sealed class FactoryProvider<T> : FactoryProvider, IServiceInstanceProvider
	{
		private readonly Func<T> factory;


		public FactoryProvider (Func<T> factory)
		{
			this.factory = factory;
		}


		bool IServiceInstanceProvider.IsValid => factory != null;

		object IServiceInstanceProvider.Get () => factory.Invoke ();
		void IServiceInstanceProvider.Initialize (IScopeKey scope) { }
		void IDisposable.Dispose () { }

		public override string ToString ()
		{
			return $"Factory<{typeof(T).FullName}>";
		}
	}
}
