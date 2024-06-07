using System;

namespace Zenvin.ServiceLocator {
	public static class IServiceLocatorExtensions {
		public static IServiceLocator Get<T> (this IServiceLocator locator, out T instance) where T : class => 
			locator.Get (typeof(T), out instance, null);

		public static IServiceLocator Get<T> (this IServiceLocator locator, out T instance, Action serviceNotFoundCallback) where T : class => 
			locator.Get (typeof(T), out instance, serviceNotFoundCallback);

		public static IServiceLocator Get<T> (this IServiceLocator locator, Type type, out T instance) where T : class =>
			locator.Get (type, out instance, null);

		public static IServiceLocator Register<T> (this IServiceLocator locator, T instance) where T : class => 
			locator.Register (typeof (T), instance, false, null);

		public static IServiceLocator Register<T> (this IServiceLocator locator, Type type, T instance) where T : class => 
			locator.Register (type, instance, false, null);
	}
}
