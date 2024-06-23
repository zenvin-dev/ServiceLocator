using System;

namespace Zenvin.ServiceLocator {
	public static class IServiceLocatorExtensions {
		public static IServiceLocator Get<T> (this IServiceLocator locator, out T instance) where T : class =>
			locator.Get (typeof (T), out instance, out _, null);

		public static IServiceLocator Get<T> (this IServiceLocator locator, out T instance, out bool serviceFound) where T : class =>
			locator.Get (typeof (T), out instance, out serviceFound, null);

		public static IServiceLocator Get<T> (this IServiceLocator locator, out T instance, Action serviceNotFoundCallback) where T : class =>
			locator.Get (typeof (T), out instance, out _, serviceNotFoundCallback);

		public static IServiceLocator Get<T> (this IServiceLocator locator, Type type, out T instance) where T : class =>
			locator.Get (type, out instance, out _, null);

		public static IServiceLocator Get<T> (this IServiceLocator locator, Type type, out T instance, out bool serviceFound) where T : class =>
			locator.Get (type, out instance, out serviceFound, null);


		public static IServiceLocator Register<T> (this IServiceLocator locator, T instance) where T : class =>
			locator.Register (typeof (T), instance, false, null, out _);

		public static IServiceLocator Register<T> (this IServiceLocator locator, T instance, out bool success) where T : class =>
			locator.Register (typeof (T), instance, false, null, out success);

		public static IServiceLocator Register<T> (this IServiceLocator locator, Type type, T instance) where T : class =>
			locator.Register (type, instance, false, null, out _);

		public static IServiceLocator Register<T> (this IServiceLocator locator, Type type, T instance, out bool success) where T : class =>
			locator.Register (type, instance, false, null, out success);

		public static IServiceLocator Register<T> (this IServiceLocator locator, Type type, T instance, bool allowReplace, out bool success) where T : class =>
			locator.Register (type, instance, allowReplace, null, out success);
	}
}
