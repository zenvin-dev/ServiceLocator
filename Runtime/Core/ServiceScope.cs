using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zenvin.Services.Core
{
	internal class ServiceScope : IDisposable
	{
		private Dictionary<Type, IServiceProvider> instances;

		internal IScopeKey ParentKey { get; set; }
		internal bool IsEmpty => instances == null || instances.Count == 0;
		internal IEnumerable<KeyValuePair<Type, IServiceProvider>> Instances => instances;


		public void Initialize (IScopeKey scope, ILogger logger)
		{
			if (IsEmpty)
				return;

			foreach (var provider in instances)
			{
				try
				{
					provider.Value.Initialize (scope);
				}
				catch (Exception e)
				{
					logger?.LogException (new Exception("Error while initializing service.", e));
				}
			}
		}

		public void Dispose ()
		{
			if (instances == null)
				return;

			foreach (var provider in instances.Values)
			{
				provider?.Dispose ();
			}
		}

		public bool Add (Type contractType, IServiceProvider provider)
		{
			if (contractType == null)
				return false;
			if (provider == null)
				return false;

			instances ??= new Dictionary<Type, IServiceProvider> (1);

#if UNITY_2021_OR_NEWER
			return instances.TryAdd(contractType, provider);
#else
			if (instances.ContainsKey (contractType))
				return false;

			instances.Add (contractType, provider);
			return true;
#endif
		}

		public bool TryGet (Type contractType, out object instance)
		{
			if (instances != null && instances.TryGetValue (contractType, out var provider))
			{
				if (provider == null || !provider.IsValid)
				{
					instances.Remove (contractType);
					instance = null;
					return false;
				}

				instance = provider.Get ();
				return instance != null;
			}

			instance = null;
			return false;
		}
	}
}
