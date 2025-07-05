using System;
using UnityEngine;
using Zenvin.Services.Providers;

namespace Zenvin.Services.Core
{
	public sealed class ServiceScopeBuilder
	{
		private readonly ServiceScope scope;
		private readonly bool isGlobal;

		private bool wasBuilt;


		private ServiceScopeBuilder ()
		{
			scope = new ServiceScope ();
		}

		internal ServiceScopeBuilder (bool isGlobal = false) : this ()
		{
			this.isGlobal = isGlobal;
		}


		internal ServiceScope Build ()
		{
			wasBuilt = true;
			return scope;
		}

		public ServiceScopeBuilder RegisterInstance<TInstance> (TInstance instance)
		{
			return RegisterInstance<TInstance, TInstance> (instance);
		}

		public ServiceScopeBuilder RegisterInstance<TContract, TInstance> (TInstance instance)
			where TInstance : TContract
		{
			AssertWasNotBuilt ();
			if (instance != null)
			{
				var provider = new ReferenceProvider (instance);
				scope.Add (typeof (TContract), provider);
			}
			return this;
		}

		public ServiceScopeBuilder RegisterInstance (object instance)
		{
			AssertWasNotBuilt ();
			if (instance != null)
			{
				var provider = new ReferenceProvider (instance);
				scope.Add (instance.GetType (), provider);
			}
			return this;
		}

		public ServiceScopeBuilder RegisterInstance (Type contractType, object instance)
		{
			AssertWasNotBuilt ();
			if (contractType != null && instance != null)
			{
				var instanceType = instance.GetType ();
				if (!contractType.IsAssignableFrom (instanceType))
					return this;

				var provider = new ReferenceProvider (instance);
				scope.Add (contractType, provider);
			}
			return this;
		}


		public ServiceScopeBuilder RegisterFactory<TInstance> (Func<TInstance> factory)
		{
			return RegisterFactory<TInstance, TInstance> (factory);
		}

		public ServiceScopeBuilder RegisterFactory<TContract, TInstance> (Func<TInstance> factory)
			where TInstance : TContract
		{
			AssertWasNotBuilt ();
			if (factory != null)
			{
				var provider = new FactoryProvider<TInstance> (factory);
				scope.Add (typeof (TContract), provider);
			}
			return this;
		}

		public ServiceScopeBuilder RegisterFactory (Type contractType, Func<object> factory)
		{
			AssertWasNotBuilt ();
			if (contractType != null && factory != null)
			{
				var provider = new FactoryProvider<object> (factory);
				scope.Add (contractType, provider);
			}
			return this;
		}


		public ServiceScopeBuilder SetParent (IScopeKey key)
		{
			AssertWasNotBuilt ();
			if (isGlobal && key != null)
			{
				Debug.LogWarning ("A global scope cannot have a parent.");
				return this;
			}

			scope.ParentKey = key;
			return this;
		}


		private void AssertWasNotBuilt ()
		{
			if (wasBuilt)
			{
				throw new InvalidOperationException (
					"The ServiceScopeBuilder already finished building its associated ServiceScope instance and may not be modified anymore."
				);
			}
		}
	}
}
