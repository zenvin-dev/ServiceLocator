#if UNITY_EDITOR
using UnityEditor;

namespace Zenvin.ServiceLocator {
	public partial class ServiceLocator {
		static partial void InitializeStatic () {
			EditorApplication.playModeStateChanged -= PlayModeStateChangedHandler;
			EditorApplication.playModeStateChanged += PlayModeStateChangedHandler;
		}

		private static void PlayModeStateChangedHandler (PlayModeStateChange change) {
			if (change != PlayModeStateChange.ExitingPlayMode)
				return;

			ResetAll ();
		}
	}
}
#endif
