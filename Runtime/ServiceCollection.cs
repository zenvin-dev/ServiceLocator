using System;
using System.Collections;
using System.Collections.Generic;

namespace Zenvin.ServiceLocator {
	public sealed class ServiceCollection : IEnumerable<KeyValuePair<Type, object>> {
		private readonly Dictionary<Type, object> instances = new Dictionary<Type, object> ();


		public bool Get<T> (Type type, out T instance) where T : class {
			instance = null;
			if (type is null)
				return false;

			if (!instances.TryGetValue (type, out var ins))
				return false;

			if (!(ins is T typedInstance))
				return false;

			instance = typedInstance;
			return true;
		}


		internal bool Register<T> (Type type, T instance, bool allowReplace) where T : class {
			if (type is null)
				return false;
			if (instance == null)
				return false;
			if (!type.IsInstanceOfType (instance))
				return false;

			if (!allowReplace && instances.ContainsKey (type))
				return false;

			if (instances.TryGetValue (type, out var oldInstance) && oldInstance is IService oldService)
				oldService.OnUnregister (true);

			instances[type] = instance;
			if (instance is IService service)
				service.OnRegister ();

			return true;
		}

		internal bool Unregister (Type type) {
			if (!instances.TryGetValue (type, out var instance))
				return false;

			if (instance is IService service)
				service.OnUnregister (true);

			instances.Remove (type);
			return true;
		}

		internal bool Unregister<T> (T instance) where T : class {
			foreach (var kvp in instances) {
				if (kvp.Value != instance)
					continue;

				instances.Remove (kvp.Key);
				if (kvp.Value is IService service)
					service.OnUnregister (false);

				return true;
			}
			return false;
		}

		internal bool Unregister<T> (Type type, T instance) where T : class {
			if (type is null)
				return false;
			if (instance == null)
				return false;

			if (!instances.TryGetValue (type, out var foundInstance))
				return false;
			if (foundInstance != instance)
				return false;

			instances.Remove (type);
			if (instance is IService service)
				service.OnUnregister (false);

			return true;
		}

		internal void Clear () {
			foreach (var instance in instances.Values) {
				if (instance is IService service)
					service.OnClear ();
			}
			instances.Clear ();
		}


		public IEnumerator<KeyValuePair<Type, object>> GetEnumerator () {
			return ((IEnumerable<KeyValuePair<Type, object>>)instances).GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable)instances).GetEnumerator ();
		}
	}
}