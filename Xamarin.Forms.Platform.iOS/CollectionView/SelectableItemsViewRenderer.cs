using System;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.iOS
{
	public class SelectableItemsViewRenderer : ItemsViewRenderer
	{
		SelectableItemsView SelectableItemsView => (SelectableItemsView)Element;
		SelectableItemsViewController SelectableItemsViewController => (SelectableItemsViewController)ItemsViewController;

		protected override ItemsViewController CreateController(ItemsView itemsView, ItemsViewLayout layout)
		{
			return new SelectableItemsViewController(itemsView as SelectableItemsView, layout);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.Is(SelectableItemsView.SelectedItemProperty))
			{
				UpdateNativeSelection();
			}
			else if (changedProperty.Is(SelectableItemsView.SelectionModeProperty))
			{
				SelectableItemsViewController.UpdateSelectionMode();
			}
		}

		protected override void SetUpNewElement(ItemsView newElement)
		{
			if (newElement != null && !(newElement is SelectableItemsView))
			{
				throw new ArgumentException($"{nameof(newElement)} must be of type {typeof(SelectableItemsView).Name}");
			}

			base.SetUpNewElement(newElement);

			SelectableItemsViewController.UpdateSelectionMode();
			UpdateNativeSelection();
		}

		void UpdateNativeSelection()
		{
			if (SelectableItemsView == null)
			{
				return;
			}

			var mode = SelectableItemsView.SelectionMode;

			if(mode == SelectionMode.None)
			{
				ClearSelection();
				return;
			}

			if(mode == SelectionMode.Single)
			{
				var selectedItem = SelectableItemsView.SelectedItem;

				ClearSelection();
				SelectableItemsViewController.SelectItem(selectedItem);
				return;
			}

			var selectedItems = SelectableItemsView.SelectedItems;

			ClearSelection();

			foreach(var item in selectedItems)
			{
				SelectableItemsViewController.SelectItem(item);
			}
		}

		void ClearSelection()
		{
			SelectableItemsViewController.ClearSelection();
		}
	}
}