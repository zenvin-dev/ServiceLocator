using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Zenvin.Services.Unity.Bootstrapping;

namespace Zenvin.Services
{
	[CustomPropertyDrawer (typeof (Bootstrapper.OrderedModule))]
	internal sealed class BootstrapperOrderedModuleDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI (SerializedProperty property)
		{
			var ele = new VisualElement ();
			SetWrapperElementStyle (ele);

			var moduleElement = new PropertyField (property.FindPropertyRelative ("module"), string.Empty);
			SetModuleElementStyle (moduleElement);
			ele.Add (moduleElement);

			var orderElement = new PropertyField (property.FindPropertyRelative ("order"), string.Empty);
			SetOrderElementStyle (orderElement);
			ele.Add (orderElement);

			return ele;
		}

		private void SetWrapperElementStyle (VisualElement ele)
		{
			var flexDirection = ele.style.flexDirection;
			flexDirection.value = FlexDirection.Row;
			ele.style.flexDirection = flexDirection;
		}

		private void SetModuleElementStyle (PropertyField ele)
		{
			var flexGrow = ele.style.flexGrow;
			flexGrow.value = 4f;
			ele.style.flexGrow = flexGrow;
		}

		private void SetOrderElementStyle (PropertyField ele)
		{
			var flexGrow = ele.style.flexGrow;
			flexGrow.value = 1f;
			ele.style.flexGrow = flexGrow;
		}
	}
}
