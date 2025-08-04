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


			/// <summary>
			/// Register a callback that will be invoked when the global scope is initialized.
			/// </summary>
			/// <remarks>
			/// If the global scope was already initialized, the callback will be invoked immediately.<br></br>
			/// If the given key is <see langword="null"/>, the callback will be invoked for the global scope.
			/// </remarks>
			/// <param name="callback">The callback to register. Must not be <see langword="null"/>.</param>
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

			/// <summary>
			/// Unregister a callback that would have been invoked when the global scope was initialized.
			/// </summary>
			/// <param name="callback">The callback to unregister. Must not be <see langword="null"/>.</param>
			public ScopeEvents OffGlobalScopeInitialized (ScopeInitializedCallback callback)
			{
				if (callback != null)
				{
					globalScopeInitialized -= callback;
				}
				return this;
			}

			/// <summary>
			/// Register a callback that will be invoked when any scope (global or keyed) is created. <br></br>
			/// The callback will <b>not</b> immediately be invoked if the global scope as initialized already.
			/// </summary>
			/// <param name="callback">The callback to register. Must not be <see langword="null"/>.</param>
			/// <param name="once">If set to <see langword="true"/>, the <paramref name="callback"/> will be unsubscribed after it has been invoked.</param>
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

			/// <summary>
			/// Unregister a callback that would have been invoked if any scope with the given key was created.
			/// </summary>
			/// <param name="callback">The callback to unregister. Must not be <see langword="null"/>.</param>
			/// <param name="once">
			/// If <see langword="null"/>, the callback will be unsubscribed, no matter whether it was temporary or persistent.<br></br>
			/// Otherwise, the callback will only be unsubscribed from either the temporary (<see langword="true"/>) or persistent (<see langword="false"/>) event.
			/// </param>
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

			/// <summary>
			/// Register a callback that will be invoked when a scope with the given key is created.
			/// </summary>
			/// <remarks>
			/// If a scope with the given key did already exist, the callback will be invoked immediately.
			/// </remarks>
			/// <param name="scope">The key of the scope to invoke the callback for.</param>
			/// <param name="callback">The callback to register. Must not be <see langword="null"/>.</param>
			/// <param name="once">If set to <see langword="true"/>, the <paramref name="callback"/> will be unsubscribed after it has been invoked.</param>
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

			/// <summary>
			/// Unregister a callback that would have been invoked if a scope with the given key was created.
			/// </summary>
			/// <param name="scope">The key of the scope to invoke the callback for.</param>
			/// <param name="callback">The callback to unregister. Must not be <see langword="null"/>.</param>
			/// <param name="once">
			/// If <see langword="null"/>, the callback will be unsubscribed, no matter whether it was temporary or persistent.<br></br>
			/// Otherwise, the callback will only be unsubscribed from either the temporary (<see langword="true"/>) or persistent (<see langword="false"/>) event.
			/// </param>
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
