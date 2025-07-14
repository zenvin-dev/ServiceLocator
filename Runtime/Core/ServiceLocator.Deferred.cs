using System.Collections.Generic;
using UnityEditor;

namespace Zenvin.Services.Core
{
	partial class ServiceLocator
	{
		/// <summary>
		/// Represents a callback method that is invoked to notify other systems of the creation of new service scopes.
		/// </summary>
		/// <param name="key">The key of the newly created scope. Will usually be <see langword="null"/> if the scope in question was the global scope.</param>
		public delegate void ScopeInitializedCallback (IScopeKey key);

		private static ScopeEvents events;

		/// <summary>
		/// Gets the global instance of <see cref="ScopeEvents"/> used to subscribe to and unsubscribe from scope-related events.<br></br>
		/// This can be used without first initializing the <see cref="ServiceLocator"/>.
		/// </summary>
		public static ScopeEvents Events
		{
			get => events ??= new ScopeEvents ();
		}

		//private static ScopeInitializedCallback globalTemporaryCallbacks;
		//private static readonly Dictionary<IScopeKey, ScopeInitializedCallback> temporaryCallbacks = new Dictionary<IScopeKey, ScopeInitializedCallback> ();

		///// <summary>
		///// Fired whenever a new scope is initialized.<br></br>
		///// If the scope in question was the global scope, the <c>key</c> will be <see langword="null"/>.
		///// </summary>
		//public static event ScopeInitializedCallback ScopeInitialized;


		///// <summary>
		///// Register a temporary callback that will be invoked when a scope with the given key is created.
		///// </summary>
		///// <remarks>
		///// If a scope with the given key did already exist, the callback will be invoked immediately.<br></br>
		///// If the given key is <see langword="null"/>, the callback will be invoked for the global scope.
		///// </remarks>
		///// <param name="callback">The callback to register. Must not be <see langword="null"/>.</param>
		///// <param name="filter">The key of the scope to invoke the callback for.</param>
		//public static void AddTemporaryScopeCallback (ScopeInitializedCallback callback, IScopeKey filter = null)
		//{
		//	if (callback == null)
		//		return;

		//	if (filter == null)
		//	{
		//		if (Initialized)
		//		{
		//			callback.Invoke (null);
		//		}
		//		else
		//		{
		//			globalTemporaryCallbacks += callback;
		//		}
		//		return;
		//	}

		//	if (filter != null && Initialized && loc.scopes.ContainsKey (filter))
		//	{
		//		callback.Invoke (filter);
		//		return;
		//	}

		//	if (temporaryCallbacks.ContainsKey (filter))
		//	{
		//		temporaryCallbacks[filter] += callback;
		//		return;
		//	}
		//	temporaryCallbacks[filter] = callback;
		//}

		///// <summary>
		///// Unregister a temporary callback that would have been invoked if a scope with the given key was created.
		///// </summary>
		///// <param name="callback">The callback to unregister. Must not be <see langword="null"/>.</param>
		///// <param name="filter">The key of the scope to invoke the callback for.</param>
		//public static void RemoveTemporaryScopeCallback (ScopeInitializedCallback callback, IScopeKey filter = null)
		//{
		//	if (callback == null)
		//		return;

		//	if (filter == null)
		//	{
		//		globalTemporaryCallbacks -= callback;
		//		return;
		//	}
		//	if (temporaryCallbacks.ContainsKey (filter))
		//	{
		//		temporaryCallbacks[filter] -= callback;
		//	}
		//}


		//private static void InvokeScopeInitialized (IScopeKey scope)
		//{
		//	ScopeInitialized?.Invoke (scope);

		//	if (scope == null)
		//	{
		//		globalTemporaryCallbacks?.Invoke (null);
		//		globalTemporaryCallbacks = null;
		//		return;
		//	}

		//	if (temporaryCallbacks.TryGetValue (scope, out var callback))
		//	{
		//		callback?.Invoke (scope);
		//		temporaryCallbacks.Remove (scope);
		//		return;
		//	}
		//}

		//private static void ResetCallbacks ()
		//{
		//	ScopeInitialized = null;
		//	globalTemporaryCallbacks = null;
		//	temporaryCallbacks.Clear ();
		//}


		/// <summary>
		/// Class representing a collection of events related to service scopes.<br></br>
		/// Use <see cref="ServiceLocator.Events"/> to retrieve an instance.
		/// </summary>
		public class ScopeEvents
		{
			private ScopeInitializedCallback globalScopeInitialized;
			private ScopeInitializedCallback anyScopeInitialized;
			private ScopeInitializedCallback anyScopeInitializedOnce;
			private readonly Dictionary<IScopeKey, SpecificEvents> specificScopeInitialized;


			internal ScopeEvents ()
			{
				specificScopeInitialized = new Dictionary<IScopeKey, SpecificEvents> ();
			}


			public ScopeEvents OnGlobalScopeInitialized (ScopeInitializedCallback callback)
			{
				if (callback == null)
					return this;

				if (Initialized)
				{
					callback.Invoke (null);
					return this;
				}

				globalScopeInitialized += callback;
				return this;
			}

			public ScopeEvents OffGlobalScopeInitialized (ScopeInitializedCallback callback)
			{
				if (callback != null)
				{
					globalScopeInitialized -= callback;
				}
				return this;
			}

			public ScopeEvents OnAnyScopeInitialized (ScopeInitializedCallback callback, bool once)
			{
				if (callback == null)
					return this;

				if (once)
				{
					anyScopeInitializedOnce += callback;
				}
				else
				{
					anyScopeInitialized += callback;
				}
				return this;
			}

			public ScopeEvents OffAnyScopeInitialized (ScopeInitializedCallback callback, bool? once)
			{
				if (callback == null)
					return this;

				if (once == null || once == true)
					anyScopeInitializedOnce -= callback;
				if (once == null || once == false)
					anyScopeInitialized -= callback;

				return this;
			}

			public ScopeEvents OnScopeInitialized (IScopeKey scope, ScopeInitializedCallback callback, bool once)
			{
				if (scope == null || callback == null)
					return this;

				var scopeExists = HasScope (scope);
				if (scopeExists)
					callback.Invoke (scope);

				if (scopeExists && once)
					return this;

				var events = specificScopeInitialized.TryGetValue (scope, out var evts) ? evts : new SpecificEvents ();

				if (once)
					events.Temporary += callback;
				else
					events.Persistent += callback;

				specificScopeInitialized[scope] = events;
				return this;
			}

			public ScopeEvents OffScopeInitialized (IScopeKey scope, ScopeInitializedCallback callback, bool? once)
			{
				if (scope == null || callback == null)
					return this;
				if (!specificScopeInitialized.TryGetValue (scope, out var events))
					return this;

				if (once == null || once == false)
					events.Persistent -= callback;
				if (once == null || once == true)
					events.Temporary -= callback;

				if (events.Empty)
					specificScopeInitialized.Remove (scope);
				else
					specificScopeInitialized[scope] = events;

				return this;
			}


			internal void Invoke (IScopeKey scope)
			{
				if (scope == null)
					globalScopeInitialized?.Invoke (null);

				anyScopeInitialized?.Invoke (scope);
				anyScopeInitializedOnce?.Invoke (scope);

				if (scope != null && specificScopeInitialized.TryGetValue (scope, out var events))
				{
					events.Persistent?.Invoke (scope);
					events.Temporary?.Invoke (scope);
					events.Temporary = null;
					specificScopeInitialized[scope] = events;
				}

				globalScopeInitialized = null;
				anyScopeInitializedOnce = null;
			}

			internal void Reset ()
			{
				globalScopeInitialized = null;
				anyScopeInitialized = null;
				anyScopeInitializedOnce = null;
				specificScopeInitialized.Clear ();
			}


			private struct SpecificEvents
			{
				public ScopeInitializedCallback Persistent;
				public ScopeInitializedCallback Temporary;

				public readonly bool Empty => Persistent == null && Temporary == null;
			}
		}
	}
}
