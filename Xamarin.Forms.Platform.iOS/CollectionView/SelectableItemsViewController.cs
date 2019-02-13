using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class SelectableItemsViewController : ItemsViewController
	{
		protected readonly SelectableItemsView SelectableItemsView;

		public SelectableItemsViewController(SelectableItemsView selectableItemsView, ItemsViewLayout layout) 
			: base(selectableItemsView, layout)
		{
			SelectableItemsView = selectableItemsView;
			Delegator.SelectableItemsViewController = this;
		}

		// _Only_ called if the user initiates the selection change; will not be called for programmatic selection
		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			UpdateFormsSelection();
		}

		// _Only_ called if the user initiates the selection change; will not be called for programmatic selection
		public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			UpdateFormsSelection();
		}

		internal void ClearSelection()
		{
			var paths = CollectionView.GetIndexPathsForSelectedItems();

			foreach (var path in paths)
			{
				CollectionView.DeselectItem(path, false);
			}
		}

		// Called by Forms to mark an item selected 
		internal void SelectItem(object selectedItem)
		{
			var index = GetIndexForItem(selectedItem);
			CollectionView.SelectItem(index, true, UICollectionViewScrollPosition.None);
		}

		void UpdateFormsSelection()
		{
			var mode = SelectableItemsView.SelectionMode;
			var selectedItems = SelectableItemsView.SelectedItems;
			var paths = CollectionView.GetIndexPathsForSelectedItems();
			//selectedItems.Clear();

			switch (mode)
			{
				case SelectionMode.None:
					//SelectableItemsView.SelectedItem = null;
					return;
				case SelectionMode.Single:
					if (paths.Length > 0)
					{
						SelectableItemsView.SelectedItem = GetItemAtIndex(paths[0]);
					}
					return;
				case SelectionMode.Multiple:
					
					var currentSelection = new List<object>();

					for (int n = 0; n < paths.Length; n++)
					{
						var path = paths[n];
						var item = GetItemAtIndex(paths[n]);
						currentSelection.Add(item);
					}

					SelectableItemsView.SelectedItems = currentSelection;

					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal void UpdateSelectionMode()
		{
			var mode = SelectableItemsView.SelectionMode;
			var oldNativeSelection = CollectionView.GetIndexPathsForSelectedItems();

			switch (mode)
			{
				case SelectionMode.None:
					CollectionView.AllowsSelection = false;
					CollectionView.AllowsMultipleSelection = false;
					break;
				case SelectionMode.Single:

					//System.Diagnostics.Debug.WriteLine($">>>>> Before");
					//DebugPaths();

					CollectionView.AllowsSelection = true;
					CollectionView.AllowsMultipleSelection = false;

					//System.Diagnostics.Debug.WriteLine($">>>>> After");
					//DebugPaths();

					break;
				case SelectionMode.Multiple:
					CollectionView.AllowsSelection = true;
					CollectionView.AllowsMultipleSelection = true;
					break;
			}

			var newNativeSelection = CollectionView.GetIndexPathsForSelectedItems();

			if (SelectedPathsEqual(oldNativeSelection, newNativeSelection))
			{
				// Changing the mode hasn't changed the selection at all, so no need to update the Forms selection
				return;
			}

			UpdateFormsSelection();
		}

		bool SelectedPathsEqual(NSIndexPath[] previous, NSIndexPath[] current)
		{
			if (current.Length != previous.Length)
			{
				return false;
			}

			for (int n = 0; n < current.Length; n++)
			{
				if (current[n] != previous[n])
				{
					return false;
				}
			}

			return true;
		}

		//void DebugPaths()
		//{
		//	var paths = CollectionView.GetIndexPathsForSelectedItems();

		//	for (int n = 0; n < paths.Length; n++)
		//	{
		//		var path = paths[n];
		//		var item = GetItemAtIndex(paths[n]);
		//		System.Diagnostics.Debug.WriteLine($">>>>> {n}: row is {path.Row}, description is {path.Description}, item is {item.ToString()} ");
		//	}
		//}
	}
}