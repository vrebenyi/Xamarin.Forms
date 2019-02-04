using System;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class GroupableItemsViewController : SelectableItemsViewController
	{
		GroupableItemsView GroupableItemsView => (GroupableItemsView)ItemsView;
		
		public GroupableItemsViewController(GroupableItemsView groupableItemsView, ItemsViewLayout layout) 
			: base(groupableItemsView, layout)
		{
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			if (!GroupableItemsView.IsGroupingEnabled)
			{
				return 0;
			}

			return ItemsSource.GroupCount;
		}

		protected override IItemsViewSource CreateItemsViewSource()
		{
			if (GroupableItemsView.IsGroupingEnabled)
			{
				return ItemsSourceFactory.CreateGrouped(GroupableItemsView.ItemsSource, CollectionView);
			}

			return base.CreateItemsViewSource();
		}

		protected override void RegisterCells()
		{
			base.RegisterCells();
			CollectionView.RegisterClassForSupplementaryView(typeof(HorizontalTemplatedHeaderView),
				UICollectionElementKindSection.Header, HorizontalTemplatedHeaderView.ReuseId);
			CollectionView.RegisterClassForSupplementaryView(typeof(VerticalTemplatedHeaderView),
				UICollectionElementKindSection.Header, VerticalTemplatedHeaderView.ReuseId);
		}

		public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, 
			NSString elementKind, NSIndexPath indexPath)
		{
			var view = collectionView.DequeueReusableSupplementaryView(elementKind, DetermineViewReuseId(elementKind), indexPath) as UICollectionReusableView;

			switch (view)
			{
				case DefaultCell defaultCell:
					UpdateDefaultCell(defaultCell, indexPath);
					break;
				case TemplatedCell templatedCell:
					UpdateTemplatedSupplementaryView(templatedCell, elementKind, indexPath);
					break;
			}

			// TODO hartez Give the template some data and let it create the actual content

			return view;
		}

		void UpdateTemplatedSupplementaryView(TemplatedCell cell, NSString elementKind, NSIndexPath indexPath)
		{
			ApplyTemplateAndDataContext(cell, elementKind, indexPath);

			if (cell is ItemsViewCell constrainedCell)
			{
				ItemsViewLayout.PrepareCellForLayout(constrainedCell);
			}
		}

		void ApplyTemplateAndDataContext(TemplatedCell cell, NSString elementKind, NSIndexPath indexPath)
		{
			DataTemplate template;

			if (elementKind == UICollectionElementKindSectionKey.Header)
			{
				template = GroupableItemsView.HeaderTemplate;
			}
			else
			{
				template = null;
			}

			var templateElement = template.CreateContent() as View;
			var renderer = CreateRenderer(templateElement);

			BindableObject.SetInheritedBindingContext(renderer.Element, ItemsSource.Group(indexPath));
			cell.SetRenderer(renderer);
		}

		string DetermineViewReuseId(NSString elementKind)
		{
			if (elementKind == UICollectionElementKindSectionKey.Header)
			{
				// if kind is header, check scroll direction and header template
				return DetermineHeaderViewReuseId();

			}

			return DetermineFooterViewReuseId();

			// ...
		}

		private string DetermineFooterViewReuseId()
		{
			throw new NotImplementedException();
		}

		string DetermineHeaderViewReuseId()
		{
			if (GroupableItemsView.HeaderTemplate != null)
			{
				return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
					? HorizontalTemplatedHeaderView.ReuseId
					: VerticalTemplatedHeaderView.ReuseId;
			}

			throw new NotImplementedException();

			//return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
			//	? HorizontalDefaultCell.ReuseId
			//	: VerticalDefaultCell.ReuseId;
		}

		//public override nint GetItemsCount(UICollectionView collectionView, nint section)
		//{
		//	var totalCount = base.GetItemsCount(collectionView, section);

		//	if (totalCount > 0)
		//	{
		//		return ItemsSource.ItemCountInGroup(section);
		//	}
		//	else
		//	{
		//		return 0;
		//	}
		//}
	}

	public class SelectableItemsViewController : ItemsViewController
	{
		SelectableItemsView SelectableItemsView => (SelectableItemsView)ItemsView;

		public SelectableItemsViewController(SelectableItemsView selectableItemsView, ItemsViewLayout layout) 
			: base(selectableItemsView, layout)
		{
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

			switch (mode)
			{
				case SelectionMode.None:
					SelectableItemsView.SelectedItem = null;
					// TODO hartez Clear SelectedItems
					return;
				case SelectionMode.Single:
					var paths = CollectionView.GetIndexPathsForSelectedItems();
					if (paths.Length > 0)
					{
						SelectableItemsView.SelectedItem = GetItemAtIndex(paths[0]);
					}
					// TODO hartez Clear SelectedItems
					return;
				case SelectionMode.Multiple:
					// TODO hartez Handle setting SelectedItems to all the items at the selected paths	
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal void UpdateSelectionMode()
		{
			var mode = SelectableItemsView.SelectionMode;

			switch (mode)
			{
				case SelectionMode.None:
					CollectionView.AllowsSelection = false;
					CollectionView.AllowsMultipleSelection = false;
					break;
				case SelectionMode.Single:
					CollectionView.AllowsSelection = true;
					CollectionView.AllowsMultipleSelection = false;
					break;
				case SelectionMode.Multiple:
					CollectionView.AllowsSelection = true;
					CollectionView.AllowsMultipleSelection = true;
					break;
			}

			UpdateFormsSelection();
		}
	}
}