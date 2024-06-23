using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zenvin.ServiceLocator.Editor {
	public class RegisteredServicesWindow : EditorWindow {

		private IServiceContext selectedContext = null;
		private ServiceCollection globalCollection;

		private Vector2 contextScroll;
		private Vector2 serviceScroll;


		[MenuItem ("Window/Zenvin/Service Locator Insight")]
		private static void Init () {
			var win = GetWindow<RegisteredServicesWindow> ();
			win.titleContent = new GUIContent ("Registered Services");
			win.Show ();
		}


		private void OnGUI () {
			using (new GUILayout.HorizontalScope ()) {
				using (new GUILayout.VerticalScope ()) {
					DrawContexts ();
				}
				using (new GUILayout.VerticalScope ()) {
					DrawServices ();
				}
			}
		}

		private void DrawContexts () {
			GUILayout.Label ("Contexts");

			serviceScroll = EditorGUILayout.BeginScrollView (serviceScroll);
			DrawContextButton (null);
			foreach (var context in ServiceLocator.GetContexts ()) {
				DrawContextButton (context);
			}
			EditorGUILayout.EndScrollView ();
		}

		private void DrawContextButton (IServiceContext context) {
			GUI.backgroundColor = context == selectedContext ? Color.cyan : Color.white;
			if (GUILayout.Button (context?.ToString () ?? "<Global>", GUILayout.MaxWidth(200))) {
				selectedContext = context;
			}
		}

		private void DrawServices () {
			GUILayout.Label ("Services");

			var coll = selectedContext == null ? ServiceLocator.Global.GetCollection () : ServiceLocator.GetCollection (selectedContext);
			if (coll == null) {
				GUILayout.Label ("No collection has been initialized for this context.");
				return;
			}

			serviceScroll = EditorGUILayout.BeginScrollView (serviceScroll);
			foreach (var service in coll) {
				GUILayout.Label (service.ToString ());
			}
			EditorGUILayout.EndScrollView ();
		}

	}
}