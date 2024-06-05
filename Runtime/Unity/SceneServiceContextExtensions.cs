using UnityEngine;

namespace Zenvin.ServiceLocator.Unity {
	public static class SceneServiceContextExtensions {
		public static SceneServiceContext? GetSceneContext (this Component component) {
			if (component == null)
				return null;

			return new SceneServiceContext (component.gameObject.scene);
		}

		public static SceneServiceContext? GetSceneContext (this GameObject gameObject) {
			if (gameObject == null)
				return null;

			return new SceneServiceContext (gameObject.scene);
		}
	}
}
