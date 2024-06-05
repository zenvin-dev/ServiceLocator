using System;

namespace Zenvin.ServiceLocator {
	public interface IServiceLocator {
		ServiceLocator Get<T> (out T instance) where T : class;
		ServiceLocator Get<T> (out T instance, Action missingServiceCallback) where T : class;
		ServiceLocator Get<T> (Type type, out T instance) where T : class;
		ServiceLocator Get<T> (Type type, out T instance, Action missingServiceCallback) where T : class;
		ServiceLocator Register<T> (T instance, bool allowReplace = false) where T : class;
		ServiceLocator Register<T> (Type type, T instance, bool allowReplace = false) where T : class;
		ServiceLocator Unregister<T> (T instance) where T : class;
		ServiceLocator Unregister<T> (Type type, T instance) where T : class;
		ServiceLocator Register<T> (Type type, T instance, bool allowReplace, Action registerFailedCallback) where T : class;
		ServiceLocator Reset ();
	}
}