using System;
using Zenvin.Services.Core;
using Object = UnityEngine.Object;

namespace Zenvin.Services.Unity
{
	internal class UnityObjectLazyProvider<T> : IServiceInstanceProvider
		where T : Object
	{
		private readonly T prefab;
		private T instance;


		public UnityObjectLazyProvider (T prefab)
		{
			this.prefab = prefab;
		}


		bool IServiceInstanceProvider.IsValid => prefab != null;


		object IServiceInstanceProvider.Get ()
		{
			if (instance == null && prefab != null)
				instance = Object.Instantiate (prefab);

			return instance;
		}

		void IServiceInstanceProvider.Initialize (IScopeKey scope) { }
		void IDisposable.Dispose () { }


		public override string ToString ()
		{
			var ins = instance == null ? "[pending]" : instance.GetType ().FullName;
			return $"UnityLazy<{typeof(T).FullName}, {prefab}, {ins}>";
		}
	}
}
