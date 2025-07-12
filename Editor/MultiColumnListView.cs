using UnityEngine.UIElements;

namespace Zenvin.Services
{
	public class MultiColumnListView : UnityEngine.UIElements.MultiColumnListView
	{
		public new class UxmlFactory : UxmlFactory<MultiColumnListView, UxmlTraits> { }

		public new class UxmlTraits : UnityEngine.UIElements.MultiColumnListView.UxmlTraits { }
	}
}
