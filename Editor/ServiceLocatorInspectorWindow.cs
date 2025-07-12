using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Zenvin.Services.Core;
using Zenvin.Services.Providers;
using IServiceProvider = Zenvin.Services.Core.IServiceProvider;

namespace Zenvin.Services
{
	internal class ServiceLocatorInspectorWindow : EditorWindow
	{
		[SerializeField] private VisualTreeAsset visualTree;
		[SerializeField] private StyleSheet styleSheet;

		[SerializeField] private Sprite modeIconSingleton;
		[SerializeField] private Sprite modeIconTransient;
		[SerializeField] private Sprite modeIconUnknown;
		[SerializeField] private Sprite globalScopeIcon;

		private HelpBox errorMessage;
		private MultiColumnListView listView;


		[MenuItem ("Window/Zenvin/Service Inspector")]
		private static void Init ()
		{
			var win = CreateWindow<ServiceLocatorInspectorWindow> ();
			win.titleContent = new GUIContent ("Service Locator");
			win.Show ();
		}


		private void CreateGUI ()
		{
			if (visualTree == null)
			{
				rootVisualElement.Add (new HelpBox
				{
					text = "VisualTree Asset is missing. Window cannot be initialized.",
					messageType = HelpBoxMessageType.Error,
				});
				return;
			}

			var initialized = ServiceLocator.Initialized;

			var root = rootVisualElement;
			visualTree.CloneTree (root);
			root.styleSheets.Add (styleSheet);

			SetupListView (root);
			SetupNotInitializedMessage (root);
			SetupRefreshButton (root);
			RefreshListView (initialized);
		}

		private void OnEnable ()
		{
			EditorApplication.playModeStateChanged += PlayModeStateChangedHandler;
		}

		private void OnDisable ()
		{
			EditorApplication.playModeStateChanged -= PlayModeStateChangedHandler;
		}


		private void SetupNotInitializedMessage (VisualElement root)
		{
			errorMessage = new HelpBox
			{
				text = "Service Locator has not been initialized.",
				messageType = HelpBoxMessageType.Warning,
			};
			root.Q (name: "message-container").Add (errorMessage);
		}

		private void SetupListView (VisualElement root)
		{
			listView = root.Q<MultiColumnListView> ();
			listView.columns.Add (new Column
			{
				title = "Scope",
				makeCell = MakeLabelCell,
				bindCell = BindScopeCell,
				stretchable = true,
				resizable = true,
				sortable = true,
			});
			listView.columns.Add (new Column
			{
				title = "Parent Scope",
				makeCell = MakeLabelCell,
				bindCell = BindParentScopeCell,
				stretchable = false,
				resizable = true,
				sortable = false,
				minWidth = 100,
			});
			listView.columns.Add (new Column
			{
				title = "Mode",
				makeCell = MakeIconCell,
				bindCell = BindModeCell,
				stretchable = false,
				resizable = false,
				sortable = false,
				width = 50,
			});
			listView.columns.Add (new Column
			{
				title = "Contract Type",
				makeCell = MakeLabelCell,
				bindCell = BindContractCell,
				stretchable = true,
				resizable = true,
			});
			listView.columns.Add (new Column
			{
				title = "Instance Type",
				makeCell = MakeLabelCell,
				bindCell = BindInstanceTypeCell,
				stretchable = true,
				resizable = true,
			});
			listView.columns.Add (new Column
			{
				title = "Instance",
				makeCell = MakeLabelCell,
				bindCell = BindInstanceCell,
				stretchable = true,
				resizable = true,
			});
		}

		private void SetupRefreshButton (VisualElement root)
		{
			var btn = root.Q<ToolbarButton> (name: "btn-refresh");
			btn.clicked += RefreshClickedHandler;
		}

		private VisualElement MakeLabelCell ()
		{
			return new Label ();
		}

		private VisualElement MakeIconCell ()
		{
			var label = new Label ();
			label.AddToClassList ("icon-cell");
			return label;
		}

		private void BindScopeCell (VisualElement element, int index)
		{
			if (!(listView.itemsSource[index] is RegisteredService item))
				return;
			if (!(element is Label lbl))
				return;

			(string text, Sprite icon) = item.Key == null ?
				("Global", globalScopeIcon) :
				(item.Key.ToString (), null);

			SetCellBackground (lbl, icon, true);
			lbl.text = text;
		}

		private void BindParentScopeCell (VisualElement element, int index)
		{
			if (!(listView.itemsSource[index] is RegisteredService item))
				return;

			(element as Label).text = item.Scope?.ParentKey?.ToString () ?? "";
		}

		private void BindModeCell (VisualElement element, int index)
		{
			if (!(listView.itemsSource[index] is RegisteredService item))
				return;
			if (!(element is Label lbl))
				return;

			(Sprite sprite, string tooltip, bool visible) = item.Provider switch
			{
				ReferenceProvider => (modeIconSingleton, "Singleton", true),
				FactoryProvider => (modeIconTransient, "Transient / Factory", true),
				_ => (modeIconUnknown, "N/A", modeIconUnknown != null),
			};

			SetCellBackground (lbl, sprite);
			lbl.tooltip = tooltip;
			lbl.visible = visible;
		}

		private void BindContractCell (VisualElement element, int index)
		{
			if (!(listView.itemsSource[index] is RegisteredService item))
				return;

			(element as Label).text = item.ContractType.FullName;
		}

		private void BindInstanceTypeCell (VisualElement element, int index)
		{
			if (!(listView.itemsSource[index] is RegisteredService item))
				return;

			var value = 1 switch { _ => "1" };

			(element as Label).text = item.Provider switch
			{
				ReferenceProvider rp => rp.GetInstanceType ()?.FullName,
				_ => "N/A",
			};
		}

		private void BindInstanceCell (VisualElement element, int index)
		{
			if (!(listView.itemsSource[index] is RegisteredService item))
				return;

			(element as Label).text = item.Provider.ToString ();
		}

		private void RefreshListView (bool? initialized = null)
		{
			if (listView == null)
				return;
			initialized ??= ServiceLocator.Initialized;

			var list = (listView.itemsSource as List<RegisteredService>) ?? new List<RegisteredService> ();
			BuildServiceList (list, initialized.Value);
			listView.itemsSource = list;

			var display = errorMessage.style.display;
			display.value = initialized.Value ? DisplayStyle.None : DisplayStyle.Flex;
			errorMessage.style.display = display;
		}

		private void BuildServiceList (List<RegisteredService> list, bool initialized)
		{
			list.Clear ();
			if (!initialized)
				return;

			BuildScopeServiceList (null, ServiceLocator.GlobalScope, list);

			var keyedScopes = ServiceLocator.KeyedScopes;
			if (keyedScopes == null)
				return;

			foreach (var scope in keyedScopes)
				BuildScopeServiceList (scope.Key, scope.Value, list);
		}

		private void BuildScopeServiceList (IScopeKey key, ServiceScope scope, List<RegisteredService> list)
		{
			foreach (var instance in scope.Instances)
			{
				var reg = new RegisteredService
				{
					Key = key,
					Scope = scope,
					ContractType = instance.Key,
					Provider = instance.Value,
				};
				list.Add (reg);
			}
		}


		private void RefreshClickedHandler ()
		{
			RefreshListView ();
		}

		private void PlayModeStateChangedHandler (PlayModeStateChange change)
		{
			EditorApplication.delayCall += () => RefreshListView ();
		}


		private static void SetCellBackground (VisualElement ele, Sprite sprite, bool alignLeft = false)
		{
			var background = ele.style.backgroundImage.value;
			background.sprite = sprite;
			ele.style.backgroundImage = background;

			if (sprite != null)
			{
				ele.AddToClassList ("icon-cell");

				if (alignLeft)
				{
					ele.AddToClassList ("icon-left");
				}
				else
				{
					ele.RemoveFromClassList ("icon-left");
				}
			}
			else
			{
				ele.RemoveFromClassList ("icon-cell");
				ele.RemoveFromClassList ("icon-left");
			}
		}
	}

	internal class RegisteredService
	{
		public IScopeKey Key { get; set; }
		public ServiceScope Scope { get; set; }
		public Type ContractType { get; set; }
		public IServiceProvider Provider { get; set; }
	}
}
