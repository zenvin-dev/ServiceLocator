using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using Zenvin.Services.Core;

namespace Zenvin.Services
{
	internal class ServiceLocatorInspectorWindow : EditorWindow
	{
		[MenuItem ("Window/Zenvin/Service Inspector")]
		private static void Init ()
		{
			GetWindow<ServiceLocatorInspectorWindow> ().Show ();
		}

		private void CreateGUI ()
		{
			var root = rootVisualElement;

			if (!ServiceLocator.Initialized)
			{
				var label = new HelpBox ("Service Locator has not been initialized.", HelpBoxMessageType.Info);
				root.Add (label);
				return;
			}

			var globalScope = ServiceLocator.GlobalScope;
			BuildScopeFoldout (new KeyValuePair<IScopeKey, ServiceScope> (null, globalScope), root);

			var keyedScopes = ServiceLocator.KeyedScopes;
			if (keyedScopes != null)
			{
				foreach (var scope in keyedScopes)
				{
					BuildScopeFoldout (scope, root);
				}
			}
		}

		private void BuildScopeFoldout (KeyValuePair<IScopeKey, ServiceScope> kvp, VisualElement root)
		{

			var scope = kvp.Value;
			if (scope == null || scope.IsEmpty)
				return;

			var key = kvp.Key;
			var scopeName = key == null ? "GLOBAL" : key.ToString ();

			var foldout = new Foldout () { text = scopeName };
			root.Add (foldout);
			AlignContent (foldout.contentContainer, Justify.FlexStart, Align.Stretch, FlexDirection.Column);

			var instances = scope.Instances;
			foreach (var ins in instances)
			{
				var rowEle = new VisualElement ();
				Grow (rowEle);
				AlignContent (rowEle, Justify.FlexStart, Align.Stretch, FlexDirection.Row);
				foldout.contentContainer.Add (rowEle);

				var text = $"{ins.Key}: {ins}";
				var label = new Label (text);
				rowEle.Add (label);
			}
		}

		private void Grow (VisualElement ele)
		{
			var grow = ele.style.flexGrow;
			grow.value = 1f;
			ele.style.flexGrow = grow;
		}

		private void AlignContent (VisualElement ele, Justify justify, Align align, FlexDirection direction = FlexDirection.Row)
		{
			var stlJustify = ele.style.justifyContent;
			stlJustify.value = justify;
			ele.style.justifyContent = stlJustify;

			var stlAlign = ele.style.alignItems;
			stlAlign.value = align;
			ele.style.alignItems = stlAlign;

			var stlDir = ele.style.flexDirection;
			stlDir.value = direction;
			ele.style.flexDirection = stlDir;
		}
	}
}
