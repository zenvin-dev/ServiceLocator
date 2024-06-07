using System;
using System.Collections.Generic;

namespace Zenvin.ServiceLocator {
	/// <summary>
	/// An implementation of <see cref="IServiceLocator"/> that contains both a global service collection an arbitrary number of contextualized collections.
	/// </summary>
	public sealed partial class ServiceLocator : IServiceLocator {
		private static ServiceLocator global;
		private static Dictionary<IServiceContext, ServiceLocator> contextualized;

		private ServiceCollection collection;
		private ServiceCollection Collection => collection ??= new ServiceCollection ();

		/// <summary>
		/// A context-less instance of the <see cref="ServiceLocator"/>.
		/// </summary>
		public static ServiceLocator Global => global ??= new ServiceLocator ();

		/// <summary>
		/// Gets or creates a contextualized <see cref="ServiceLocator"/> instance for the given context.<br></br>
		/// Will fall back to <see cref="Global"/> when <paramref name="context"/> is <see langword="null"/>.
		/// </summary>
		/// <remarks>
		/// If the context does not have an existing <see cref="ServiceLocator"/> instance associated with it, a proxy object will be returned.
		/// This proxy will create a new locator for the given context on demand, once instances are registered with it.
		/// </remarks>
		public static IServiceLocator ForContext (IServiceContext context) {
			if (context == null)
				return Global;

			if (contextualized == null || !contextualized.TryGetValue (context, out var locator))
				return new Proxy (context);

			return locator;
		}

		/// <summary>
		/// Resets the Global and all contextualized <see cref="ServiceLocator"/> instances.
		/// </summary>
		public static void ResetAll () {
			global?.Reset ();
			if (contextualized != null) {
				foreach (var loc in contextualized.Values) {
					loc?.Reset ();
				}
				contextualized = null;
			}
		}


		/// <inheritdoc/>
		public IServiceLocator Get<T> (Type type, out T instance, Action missingServiceCallback) where T : class {
			if (!Collection.Get (type, out instance))
				missingServiceCallback?.Invoke ();

			return this;
		}

		/// <inheritdoc/>
		public IServiceLocator Register<T> (Type type, T instance, bool allowReplace, Action registerErrorCallback) where T : class {
			if (!Collection.Register (type, instance, allowReplace))
				registerErrorCallback?.Invoke ();

			return this;
		}

		/// <inheritdoc/>
		public IServiceLocator Unregister (Type type) {
			Collection.Unregister (type);
			return this;
		}

		/// <inheritdoc/>
		public IServiceLocator Unregister<T> (T instance) where T : class {
			Collection.Unregister (instance);
			return this;
		}

		/// <inheritdoc/>
		public IServiceLocator Unregister<T> (Type type, T instance) where T : class {
			Collection.Unregister (type, instance);
			return this;
		}

		/// <inheritdoc/>
		public IServiceLocator Reset () {
			Collection.Clear ();
			return this;
		}


		private static ServiceLocator CreateLocatorForContext (IServiceContext context) {
			if (contextualized == null)
				contextualized = new Dictionary<IServiceContext, ServiceLocator> ();

			if (!contextualized.TryGetValue (context, out var locator))
				contextualized[context] = locator = new ServiceLocator ();

			return locator;
		}
	}
}
