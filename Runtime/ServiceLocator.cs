using System;
using System.Collections.Generic;

namespace Zenvin.ServiceLocator {
	public sealed class ServiceLocator : IServiceLocator {
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
		public static ServiceLocator ForContext (IServiceContext context) {
			if (context == null)
				return Global;

			if (contextualized == null)
				contextualized = new Dictionary<IServiceContext, ServiceLocator> ();

			if (!contextualized.TryGetValue (context, out var locator))
				contextualized[context] = locator = new ServiceLocator ();

			return locator;
		}

		/// <inheritdoc/>
		public ServiceLocator Get<T> (out T instance) where T : class {
			return Get (typeof (T), out instance);
		}

		/// <inheritdoc/>
		public ServiceLocator Get<T> (out T instance, Action missingServiceCallback) where T : class {
			return Get (typeof(T), out instance, missingServiceCallback);
		}

		/// <inheritdoc/>
		public ServiceLocator Get<T> (Type type, out T instance) where T : class {
			Collection.Get (type, out instance);
			return this;
		}

		/// <inheritdoc/>
		public ServiceLocator Get<T> (Type type, out T instance, Action missingServiceCallback) where T : class {
			if (!Collection.Get (type, out instance))
				missingServiceCallback?.Invoke ();

			return this;
		}

		/// <inheritdoc/>
		public ServiceLocator Register<T> (T instance) where T : class {
			return Register (instance, false);
		}

		/// <inheritdoc/>
		public ServiceLocator Register<T> (T instance, bool allowReplace) where T : class {
			return Register (typeof (T), instance, allowReplace);
		}

		/// <inheritdoc/>
		public ServiceLocator Register<T> (Type type, T instance, bool allowReplace) where T : class {
			Collection.Register (type, instance, allowReplace);
			return this;
		}

		/// <inheritdoc/>
		public ServiceLocator Register<T> (Type type, T instance, bool allowReplace, Action registerFailedCallback) where T : class {
			if (!Collection.Register (type, instance, allowReplace))
				registerFailedCallback?.Invoke ();

			return this;
		}

		/// <inheritdoc/>
		public ServiceLocator Unregister<T> (T instance) where T : class {
			Collection.Unregister (instance);
			return this;
		}

		/// <inheritdoc/>
		public ServiceLocator Unregister<T> (Type type, T instance) where T : class {
			Collection.Unregister (type, instance);
			return this;
		}

		/// <inheritdoc/>
		public ServiceLocator Reset () {
			Collection.Clear ();
			return this;
		}
	}
}
