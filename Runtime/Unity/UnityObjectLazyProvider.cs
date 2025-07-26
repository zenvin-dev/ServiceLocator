using System;
using UnityEngine;
using Zenvin.Services.Core;
using Object = UnityEngine.Object;

namespace Zenvin.Services.Unity
{
	internal class UnityObjectLazyProvider<T> : IServiceInstanceProvider
		where T : Object
	{
		private readonly T prefab;
		private readonly bool dontDestroyOnLoad;
		private T instance;


		public UnityObjectLazyProvider (T prefab, bool dontDestroyOnLoad)
		{
			this.prefab = prefab;
			this.dontDestroyOnLoad = dontDestroyOnLoad;
		}


		bool IServiceInstanceProvider.IsValid => prefab != null;


		object IServiceInstanceProvider.Get ()
		{
			EnsureInstance ();
			return instance;
		}

		void IServiceInstanceProvider.Initialize (IScopeKey scope) { }

		void IDisposable.Dispose ()
		{
			if (instance == null)
				return;
			if (instance is IDisposable disposable)
				disposable.Dispose ();

			DestroyInstance ();
		}


		private void EnsureInstance ()
		{
			if (instance != null || prefab == null)
				return;

			instance = Object.Instantiate (prefab);
			if (dontDestroyOnLoad && instance != null && TryGetInstanceGO (out var go))
				Object.DontDestroyOnLoad (go);
		}

		private void DestroyInstance ()
		{
			switch (instance)
			{
				case GameObject go:
					Object.Destroy (go);
					break;
				case Component comp:
					Object.Destroy (comp.gameObject);
					break;
				case ScriptableObject so:
					Object.Destroy (so);
					break;
			}
		}

		private bool TryGetInstanceGO (out GameObject gameObject)
		{
			gameObject = instance switch
			{
				GameObject go => go,
				Component comp => comp.gameObject,
				_ => null,
			};
			return gameObject != null;
		}


		public override string ToString ()
		{
			var ins = instance == null ? "[pending]" : instance.GetType ().FullName;
			return $"UnityLazy<{typeof (T).FullName}, {prefab}, {ins}>";
		}
	}
}
