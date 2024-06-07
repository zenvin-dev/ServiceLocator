using System;

namespace Zenvin.ServiceLocator {
	/// <summary>
	/// Interface for a <see href="https://en.wikipedia.org/wiki/Service_locator_pattern">Service Locator</see>.
	/// </summary>
	public interface IServiceLocator {
		/// <summary>
		/// Attempts to retrieve an object instance that is associated with the given type in the service locator.
		/// </summary>
		/// <typeparam name="T">The type of instance to retrieve.</typeparam>
		/// <param name="type">The type that the returned instance should be associated with. Should be assignable from <typeparamref name="T"/>.</param>
		/// <param name="instance">The instance registered in the service locator.</param>
		/// <param name="missingServiceCallback">A callback that is invoked when there is no instance associated with <paramref name="type"/>. Optional.</param>
		/// <returns>The <see cref="IServiceLocator"/> that the method was called on. Useful for chaining multiple calls.</returns>
		IServiceLocator Get<T> (Type type, out T instance, Action missingServiceCallback) where T : class;
		/// <summary>
		/// Attempts to register an object to the service locator. 
		/// </summary>
		/// <typeparam name="T">The type of object to register.</typeparam>
		/// <param name="type">The type that the <paramref name="instance"/> will be associated with. Registration will fail if the object is not an instance of <typeparamref name="T"/>.</param>
		/// <param name="instance">The object to register.</param>
		/// <param name="allowReplace">Whether the object already registered for <paramref name="type"/> (if there is any) may be replaced.</param>
		/// <param name="registerErrorCallback">A callback that is invoked when registration failed. </param>
		/// <returns>The <see cref="IServiceLocator"/> that the method was called on. Useful for chaining multiple calls.</returns>
		IServiceLocator Register<T> (Type type, T instance, bool allowReplace, Action registerErrorCallback) where T : class;
		/// <summary>
		/// Unregisters the object associated with the given <paramref name="type"/>.
		/// </summary>
		/// <returns>The <see cref="IServiceLocator"/> that the method was called on. Useful for chaining multiple calls.</returns>
		IServiceLocator Unregister (Type type);
		/// <summary>
		/// Unregisters the given instance.
		/// </summary>
		/// <param name="instance">The instance to unregister.</param>
		/// <returns>The <see cref="IServiceLocator"/> that the method was called on. Useful for chaining multiple calls.</returns>
		IServiceLocator Unregister<T> (T instance) where T : class;
		/// <summary>
		/// Unregisters the object associated with the given <paramref name="type"/>, if that object is equal to <paramref name="instance"/>.
		/// </summary>
		/// <returns>The <see cref="IServiceLocator"/> that the method was called on. Useful for chaining multiple calls.</returns>
		IServiceLocator Unregister<T> (Type type, T instance) where T : class;
		/// <summary>
		/// Removes all object associations from the service locator.
		/// </summary>
		/// <returns>The <see cref="IServiceLocator"/> that the method was called on. Useful for chaining multiple calls.</returns>
		IServiceLocator Reset ();
	}
}
