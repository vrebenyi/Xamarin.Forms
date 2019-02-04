using System;
using CoreGraphics;
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
			layout.HeaderReferenceSize = new CGSize(1, 1);
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

		
	}
}