using System;
using System.Collections.Generic;
using UnityEngine;
using Zenvin.Services.Exceptions;

namespace Zenvin.Services.Core
{
	public sealed partial class ServiceLocator
	{
		public delegate void BuildServiceScopeCallback (ServiceScopeBuilder builder);

		private static ServiceLocator loc;

		private ServiceScope globalScope;
		private readonly Dictionary<IScopeKey, ServiceScope> scopes;
		private IScopeContextProvider scopeContextProvider;


		public static IScopeContextProvider ScopeContextProvider
		{
			get
			{
				AssertInitialized ();
				return loc.scopeContextProvider;
			}
			set
			{
				AssertInitialized ();
				loc.scopeContextProvider = value;
			}
		}

		public static bool Initialized
		{
			get => loc != null;
		}

		public static int ActiveScopeCount
		{
			get
			{
				AssertInitialized ();
				return loc.scopes.Count + 1;
			}
		}


		private ServiceLocator (ServiceScope globalScope)
		{
			this.globalScope = globalScope;
			scopes = new Dictionary<IScopeKey, ServiceScope> ();
		}


		public static FluentConfigurator Initialize (BuildServiceScopeCallback buildGlobalScopeCallback)
		{
			if (buildGlobalScopeCallback == null)
				throw new ArgumentNullException (nameof (buildGlobalScopeCallback));
			if (loc != null)
				return new FluentConfigurator (loc, false);

			var builder = new ServiceScopeBuilder (true);
			buildGlobalScopeCallback?.Invoke (builder);

			var scope = builder.Build ();
			loc = new ServiceLocator (scope);

			return new FluentConfigurator (loc, true);
		}


		public static bool AddScope (IScopeKey key, BuildServiceScopeCallback buildScopeCallback)
		{
			AssertInitialized ();

			if (key == null)
				return false;
			if (buildScopeCallback == null)
				return false;

			var scopes = loc.scopes;
			if (scopes.ContainsKey (key))
				return false;

			var builder = new ServiceScopeBuilder ();
			buildScopeCallback.Invoke (builder);

			var scope = builder.Build ();
			if (scope.IsEmpty)
				return false;

			scopes.Add (key, scope);
			return true;
		}

		public static bool HasScope (IScopeKey key)
		{
			AssertInitialized ();
			return key != null && loc.scopes.ContainsKey (key);
		}

		public static bool RemoveScope (IScopeKey key)
		{
			AssertInitialized ();
			var scopes = loc.scopes;

			if (!scopes.TryGetValue (key, out var scope))
				return false;

			scopes.Remove (key);
			scope.Dispose ();
			return true;
		}


		public static TInstance Get<TInstance> ()
		{
			return Get<TInstance, TInstance> (null, true, true);
		}

		public static TInstance Get<TContract, TInstance> ()
			where TInstance : TContract
		{
			return Get<TContract, TInstance> (null, true, true);
		}

		public static TInstance Get<TInstance> (bool required)
		{
			return Get<TInstance, TInstance> (null, true, required);
		}

		public static TInstance Get<TContract, TInstance> (bool required)
			where TInstance : TContract
		{
			return Get<TContract, TInstance> (null, true, required);
		}

		public static TInstance Get<TInstance> (IScopeKey scope, bool required)
		{
			return Get<TInstance, TInstance> (scope, true, required);
		}

		public static TInstance Get<TContract, TInstance> (IScopeKey scope, bool required)
			where TInstance : TContract
		{
			return Get<TContract, TInstance> (scope, true, required);
		}

		public static TInstance Get<TInstance> (IScopeKey scope, bool fallbackToGlobalScope, bool required)
		{
			return Get<TInstance, TInstance> (scope, fallbackToGlobalScope, required);
		}

		public static TInstance Get<TContract, TInstance> (IScopeKey scope, bool fallbackToGlobalScope, bool required)
			where TInstance : TContract
		{
			AssertInitialized ();
			if (!loc.TryGetInternal<TContract, TInstance> (scope, fallbackToGlobalScope, out var instance, out var exception))
			{
				if (required)
				{
					var contractType = typeof (TContract);
					var instanceType = typeof (TInstance);

					throw
						exception ??
						new ServiceException (
							contractType,
							instanceType,
							$"Could not resolve required service for type '{typeof (TContract).AssemblyQualifiedName}'."
						);
				}

				return default;
			}

			return instance;
		}


		public static bool TryGet<TInstance> (out TInstance instance)
		{
			AssertInitialized ();
			return loc.TryGetInternal<TInstance, TInstance> (null, true, out instance, out _);
		}

		public static bool TryGet<TInstance> (IScopeKey scope, out TInstance instance)
		{
			AssertInitialized ();
			return loc.TryGetInternal<TInstance, TInstance> (scope, true, out instance, out _);
		}

		public static bool TryGet<TInstance> (IScopeKey scope, bool fallbackToGlobalScope, out TInstance instance)
		{
			AssertInitialized ();
			return loc.TryGetInternal<TInstance, TInstance> (scope, fallbackToGlobalScope, out instance, out _);
		}

		public static bool TryGet<TContract, TInstance> (out TInstance instance)
			where TInstance : TContract
		{
			AssertInitialized ();
			return loc.TryGetInternal<TContract, TInstance> (null, true, out instance, out _);
		}

		public static bool TryGet<TContract, TInstance> (IScopeKey scope, out TInstance instance)
			where TInstance : TContract
		{
			AssertInitialized ();
			return loc.TryGetInternal<TContract, TInstance> (scope, true, out instance, out _);
		}

		public static bool TryGet<TContract, TInstance> (IScopeKey scope, bool fallbackToGlobalScope, out TInstance instance)
			where TInstance : TContract
		{
			AssertInitialized ();
			return loc.TryGetInternal<TContract, TInstance> (scope, fallbackToGlobalScope, out instance, out _);
		}


		internal static void Reset ()
		{
			AssertInitialized ();

			loc.globalScope.Dispose ();
			foreach (var kvp in loc.scopes)
			{
				if (kvp.Key is IDisposable keyDisposable)
					keyDisposable.Dispose ();

				kvp.Value.Dispose ();
			}

			loc.scopes.Clear ();
			loc.globalScope = null;
			loc.scopeContextProvider = null;

			loc = null;
		}


		private bool TryGetInternal<TContract, TInstance> (
			IScopeKey scopeKey,
			bool fallbackToGlobalValue,
			out TInstance instance,
			out ServiceException exception)
			where TInstance : TContract
		{
			instance = default;

			var contractType = typeof (TContract);
			var instanceType = typeof (TInstance);

			if (!loc.TryGetInternal (contractType, instanceType, out var objInstance, false, fallbackToGlobalValue, scopeKey, out exception))
				return false;

			if (!TryCast (objInstance, contractType, instanceType, out instance, out exception))
				return false;

			return true;
		}

		private bool TryGetInternal (
			Type contractType,
			Type instanceType,
			out object instance,
			bool doTypeCheck,
			bool fallback,
			IScopeKey scopeKey,
			out ServiceException exception)
		{
			instance = null;
			if (TryGetTypesException (contractType, instanceType, doTypeCheck, out exception))
				return false;

			scopeKey ??= scopeContextProvider?.GetActiveScope ();
			var hasKey = scopeKey != null;

			ServiceScope scope;
			if (hasKey)
			{
				var visited = new HashSet<IScopeKey> ();
				do
				{
					if (scopes.TryGetValue (scopeKey, out scope) && scope.TryGet (contractType, out instance))
						return true;

					scopeKey = scope?.ParentKey;
				}
				while (scope != null && scopeKey != null && visited.Add (scopeKey));
			}

			if (!fallback && hasKey)
				return false;

			return globalScope.TryGet (contractType, out instance);
		}

		private static bool TryGetTypesException (Type contractType, Type instanceType, bool doTypeCheck, out ServiceException exception)
		{
			exception = null;

			if (contractType == null)
				exception = new ServiceException (contractType, instanceType, "Could not resolve service because no contract type was given.");

			else if (instanceType == null)
				exception = new ServiceException (contractType, instanceType, "Could not resolve service because no instance type was given.");

			else if (doTypeCheck && !contractType.IsAssignableFrom (instanceType))
				exception = new ServiceException (contractType, instanceType, $"Could not resolve service because the given instance type is not assignable to the contract type.");

			return exception != null;
		}

		private static bool TryCast<T> (object value, Type contractType, Type instanceType, out T instance, out ServiceException exception)
		{
			if (value is T typedValue)
			{
				instance = typedValue;
				exception = null;
				return true;
			}

			instance = default;
			exception = new ServiceException (
				contractType,
				instanceType,
				$"Could not resolve contract type '{contractType.AssemblyQualifiedName}' to instance of type {instanceType.AssemblyQualifiedName}."
			);
			return false;
		}

		private static void AssertInitialized ()
		{
			if (loc == null)
			{
				throw new ServiceLocatorNotInitializedException ();
			}
		}
	}
}
