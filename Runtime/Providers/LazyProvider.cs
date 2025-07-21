using System;
using Zenvin.Services.Core;

namespace Zenvin.Services.Providers
{
	internal sealed class LazyProvider<T> : FactoryProvider, IServiceInstanceProvider
	{
		private readonly Func<T> factory;
		private T instance;

		public LazyProvider (Func<T> factory)
		{
			this.factory = factory;
		}


		bool IServiceInstanceProvider.IsValid => factory != null;

		object IServiceInstanceProvider.Get ()
		{
			if (instance != null)
				return instance;

			instance = factory.Invoke();
			return instance;
		}

		void IServiceInstanceProvider.Initialize (IScopeKey scope) { }
		void IDisposable.Dispose () { }

		public override string ToString ()
		{
			var ins = instance == null ? "[pending]" : instance.GetType().FullName;
			return $"Lazy<{typeof(T).FullName}, {ins}>";
		}
	}
}
