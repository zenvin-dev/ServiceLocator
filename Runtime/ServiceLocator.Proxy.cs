using System;
using UnityEngine;

namespace Zenvin.ServiceLocator {
	public sealed partial class ServiceLocator {
		internal class Proxy : IServiceLocator {
			private readonly IServiceContext context;


			private Proxy () { }

			internal Proxy (IServiceContext context) => this.context = context;


			public IServiceLocator Get<T> (Type type, out T instance, Action missingServiceCallback) where T : class {
				instance = null;
				missingServiceCallback?.Invoke ();
				return this;
			}

			public IServiceLocator Register<T> (Type type, T instance, bool allowReplace, Action registerErrorCallback) where T : class {
				if (context == null)
					return this;

				var locator = CreateLocatorForContext (context);
				return locator.Register (instance);
			}

			public IServiceLocator Unregister (Type type) {
				Debug.LogWarning ("Cannot unregister value from proxy.");
				return this;
			}

			public IServiceLocator Unregister<T> (Type type, T instance) where T : class {
				// Since proxies are effectively null-placeholders for locators, they will not contain a value that could be unregistered
				Debug.LogWarning ("Cannot unregister value from proxy.");
				return this;
			}

			public IServiceLocator Unregister<T> (T instance) where T : class {
				Debug.LogWarning ("Cannot unregister value from proxy.");
				return this;
			}

			public IServiceLocator Reset () {
				Debug.LogWarning ("Cannot reset Locator Proxy.");
				return this;
			}
		}
	}
}
