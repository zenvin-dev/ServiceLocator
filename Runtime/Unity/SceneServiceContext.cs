using System;
using UnityEngine.SceneManagement;

namespace Zenvin.ServiceLocator.Unity {
	/// <summary>
	/// An implementation of <see cref="IServiceContext"/> that refers to a Unity scene.
	/// </summary>
	public readonly struct SceneServiceContext : IServiceContext {
		/// <summary>
		/// Handle on the scene that the context refers to.
		/// </summary>
		public readonly Scene Scene;


		/// <summary>
		/// Returns a <see cref="SceneServiceContext"/> referring to the currently active scene.
		/// </summary>
		public static SceneServiceContext Active => new SceneServiceContext (SceneManager.GetActiveScene ());

		/// <summary>
		/// Returns a <see cref="SceneServiceContext"/> referring to the scene with the given <paramref name="buildIndex"/>.
		/// </summary>
		public static SceneServiceContext FromIndex (int buildIndex) => new SceneServiceContext(SceneManager.GetSceneByBuildIndex (buildIndex));


		public SceneServiceContext (Scene scene) {
			Scene = scene;
		}

		bool IEquatable<IServiceContext>.Equals (IServiceContext other) {
			return other is SceneServiceContext context &&
				   context.Scene.buildIndex == Scene.buildIndex;
		}
	}
}
