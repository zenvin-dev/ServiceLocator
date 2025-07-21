using System;
using UnityEditor;
using UnityEngine;
using Zenvin.Services.Core;
using Object = UnityEngine.Object;

namespace Zenvin.Services.Unity
{
	internal abstract class UnityObjectFactoryProvider { }

	internal class UnityObjectFactoryProvider<T> : UnityObjectFactoryProvider, IServiceInstanceProvider
		where T : Object
	{
		internal readonly T Prefab;


		public UnityObjectFactoryProvider (T prefab)
		{
			Prefab = prefab;
		}


		bool IServiceInstanceProvider.IsValid => Prefab != null;
		object IServiceInstanceProvider.Get () => Object.Instantiate (Prefab);
		void IServiceInstanceProvider.Initialize (IScopeKey scope) { }
		void IDisposable.Dispose () { }


		public override string ToString ()
		{
			return $"UnityFactory<{typeof (T).FullName}, {Prefab}>";
		}
	}
}
