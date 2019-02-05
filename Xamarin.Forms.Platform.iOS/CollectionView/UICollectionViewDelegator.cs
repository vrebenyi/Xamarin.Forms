using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class UICollectionViewDelegator : UICollectionViewDelegateFlowLayout
	{
		public ItemsViewLayout ItemsViewLayout { get; private set; }
		public SelectableItemsViewController SelectableItemsViewController { get; set; }
		public GroupableItemsViewController GroupableItemsViewController { get; set; }

		public UICollectionViewDelegator(ItemsViewLayout itemsViewLayout) => ItemsViewLayout = itemsViewLayout;

		public override void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath path)
		{
			ItemsViewLayout?.WillDisplayCell(collectionView, cell, path);
		}

		public override UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
			nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(UIEdgeInsets);
			}

			return ItemsViewLayout.GetInsetForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(nfloat);
			}

			return ItemsViewLayout.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
		}

		public override nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout, nint section)
		{
			if (ItemsViewLayout == null)
			{
				return default(nfloat);
			}

			return ItemsViewLayout.GetMinimumLineSpacingForSection(collectionView, layout, section);
		}

		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			SelectableItemsViewController?.ItemSelected(collectionView, indexPath);
		}

		public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			SelectableItemsViewController?.ItemDeselected(collectionView, indexPath);
		}

		public override CGSize GetReferenceSizeForHeader(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			if (GroupableItemsViewController == null)
			{
				return CGSize.Empty;
			}

			return GroupableItemsViewController.GetReferenceSizeForHeader(collectionView, layout, section);
		}

		public override CGSize GetReferenceSizeForFooter(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			if (GroupableItemsViewController == null)
			{
				return CGSize.Empty;
			}

			return GroupableItemsViewController.GetReferenceSizeForFooter(collectionView, layout, section);
		}
	}
}