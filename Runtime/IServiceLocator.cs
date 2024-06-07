using System;

namespace Zenvin.ServiceLocator {
	public interface IServiceLocator {
		IServiceLocator Get<T> (Type type, out T instance, Action missingServiceCallback) where T : class;
		IServiceLocator Register<T> (Type type, T instance, bool allowReplace, Action registerErrorCallback) where T : class;
		IServiceLocator Unregister<T> (T instance) where T : class;
		IServiceLocator Unregister<T> (Type type, T instance) where T : class;
		IServiceLocator Reset ();
	}
}
