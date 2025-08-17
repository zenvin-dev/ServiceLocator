using System;
using UnityEditor;
using UnityEngine;
using Zenvin.Services.Core;
using Object = UnityEngine.Object;

namespace Zenvin.Services.Unity
{
	internal class UnityObjectFactoryProvider<T> : IServiceInstanceProvider
		where T : Object
	{
		private readonly T prefab;


		public UnityObjectFactoryProvider (T prefab)
		{
			this.prefab = prefab;
		}


		bool IServiceInstanceProvider.IsValid => prefab != null;
		object IServiceInstanceProvider.Get () => Object.Instantiate (prefab);
		void IDisposable.Dispose () { }


		public override string ToString ()
		{
			return $"UnityFactory<{typeof (T).FullName}, {prefab}>";
		}
	}
}
