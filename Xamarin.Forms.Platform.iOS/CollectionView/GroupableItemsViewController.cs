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
			Delegator.GroupableItemsViewController = this;
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			if (!GroupableItemsView.IsGroupingEnabled)
			{
				return 1;
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

		// TODO hartez Maybe change this to registerviewtypes or something
		protected override void RegisterCells()
		{
			base.RegisterCells();

			RegisterSupplementaryViews(UICollectionElementKindSection.Header);
			RegisterSupplementaryViews(UICollectionElementKindSection.Footer);
		}

		private void RegisterSupplementaryViews(UICollectionElementKindSection kind)
		{
			CollectionView.RegisterClassForSupplementaryView(typeof(HorizontalTemplatedSupplementalView),
				kind, HorizontalTemplatedSupplementalView.ReuseId);
			CollectionView.RegisterClassForSupplementaryView(typeof(VerticalTemplatedSupplementalView),
				kind, VerticalTemplatedSupplementalView.ReuseId);
			CollectionView.RegisterClassForSupplementaryView(typeof(HorizontalDefaultSupplementalView),
				kind, HorizontalDefaultSupplementalView.ReuseId);
			CollectionView.RegisterClassForSupplementaryView(typeof(VerticalDefaultSupplementalView),
				kind, VerticalDefaultSupplementalView.ReuseId);
		}

		public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, 
			NSString elementKind, NSIndexPath indexPath)
		{
			var reuseId = DetermineViewReuseId(elementKind);

			var view = collectionView.DequeueReusableSupplementaryView(elementKind, reuseId, indexPath) as UICollectionReusableView;

			switch (view)
			{
				case DefaultCell defaultCell:
					UpdateDefaultSupplementaryView(defaultCell, elementKind, indexPath);
					break;
				case TemplatedCell templatedCell:
					UpdateTemplatedSupplementaryView(templatedCell, elementKind, indexPath);
					break;
			}

			return view;
		}

		void UpdateDefaultSupplementaryView(DefaultCell cell, NSString elementKind, NSIndexPath indexPath)
		{
			cell.Label.Text = ItemsSource.Group(indexPath).ToString();

			if (cell is ItemsViewCell constrainedCell)
			{
				cell.ConstrainTo(ItemsViewLayout.ConstrainedDimension);
			}
		}

		void UpdateTemplatedSupplementaryView(TemplatedCell cell, NSString elementKind, NSIndexPath indexPath)
		{
			ApplyTemplateAndDataContext(cell, elementKind, indexPath);

			if (cell is ItemsViewCell constrainedCell)
			{
				cell.ConstrainTo(ItemsViewLayout.ConstrainedDimension);
			}
		}

		void ApplyTemplateAndDataContext(TemplatedCell cell, NSString elementKind, NSIndexPath indexPath)
		{
			DataTemplate template;

			if (elementKind == UICollectionElementKindSectionKey.Header)
			{
				template = GroupableItemsView.GroupHeaderTemplate;
			}
			else
			{
				template = GroupableItemsView.GroupFooterTemplate;
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
				return DetermineViewReuseId(GroupableItemsView.GroupHeaderTemplate);
			}

			return DetermineViewReuseId(GroupableItemsView.GroupFooterTemplate);
		}

		string DetermineViewReuseId(DataTemplate template)
		{
			if (template == null)
			{
				// No template, fall back the the default supplemental views
				return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
					? HorizontalDefaultSupplementalView.ReuseId
					: VerticalDefaultSupplementalView.ReuseId;
			}

			return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
				? HorizontalTemplatedSupplementalView.ReuseId
				: VerticalTemplatedSupplementalView.ReuseId;
		}

		// TODO hartez These next two methods can turn headers/footers on/off by returning CGSize.Empty; need to be checking
		// IsGroupingEnabled here to do that

		internal CGSize GetReferenceSizeForHeader(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			// TODO hartez This will fully measure every header, but possibly twice, which is not amazing for performance
			// Verify that this is a double measure, and if so see if we can find a way around it
			// Long-term, we might be looking at more performance hints for headers/footers (if the dev knows for sure they'll 
			// all the be the same size)
			var cell = GetViewForSupplementaryElement(collectionView, UICollectionElementKindSectionKey.Header, NSIndexPath.FromItemSection(0, section)) as ItemsViewCell;

			return cell.Measure();
		}

		internal CGSize GetReferenceSizeForFooter(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			var cell = GetViewForSupplementaryElement(collectionView, UICollectionElementKindSectionKey.Footer, NSIndexPath.FromItemSection(0, section)) as ItemsViewCell;

			return cell.Measure();
		}
	}
}