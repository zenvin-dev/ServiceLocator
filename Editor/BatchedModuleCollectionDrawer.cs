using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Zenvin.Services.Unity.Bootstrapping;

namespace Zenvin.Services
{
	[CustomPropertyDrawer (typeof (BatchedModuleCollection))]
	internal sealed class BatchedModuleCollectionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI (SerializedProperty property)
		{
			var ele = new VisualElement ();

			var prop = property.FindPropertyRelative ("originalModules");
			var propEle = new PropertyField (prop, property.displayName);
			ele.Add (propEle);

			var hintEle = new HelpBox (
				"Modules with the same priority will be batched together when the bootstrapper runs. Use separate order values to ensure execution order.\n" +
				"Each module can only appear once in the load order; duplicate and null references will be ignored.",
				HelpBoxMessageType.Info
			);
			SetVisibilityBinding (hintEle, prop);
			ele.Add (hintEle);

			return ele;
		}

		private void SetVisibilityBinding (VisualElement ele, SerializedProperty prop)
		{
			var bindingId = new BindingId (nameof (VisualElement.visible));
			var bindingPath = new PropertyPath (nameof (SerializedProperty.isExpanded));
			var binding = new DataBinding
			{
				bindingMode = BindingMode.ToTarget,
				dataSource = prop,
				dataSourcePath = bindingPath,
			};
			ele.SetBinding (bindingId, binding);
		}
	}
}
