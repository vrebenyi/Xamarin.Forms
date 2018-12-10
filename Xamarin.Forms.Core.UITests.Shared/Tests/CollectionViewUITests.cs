using NUnit.Framework;

namespace Xamarin.Forms.Core.UITests
{
	[Category(UITestCategories.CollectionView)]
	internal class CollectionViewUITests : BaseTestFixture
	{
		string _collectionViewId = "collectionview";
		string _enableCollectionView = "Enable CollectionView";
		string _btnUpdate = "Update";
		string _entryUpdate = "entryUpdate";

		public CollectionViewUITests()
		{
		}

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.CollectionViewGallery);
		}

		protected override void TestTearDown()
		{
			base.TestTearDown();
			ResetApp();
			NavigateToGallery();
		}

		[TestCase("CarouselView", new string[] { "CarouselViewCode,Horizontal", "CarouselViewCode,Vertical" }, 19, 6)]
		[TestCase("ScrollTo", new string[] {
			"ScrollToIndexCode,HorizontalList", "ScrollToIndexCode,VerticalList", "ScrollToIndexCode,HorizontalGrid", "ScrollToIndexCode,VerticalGrid",
			"ScrollToItemCode,HorizontalList", "ScrollToItemCode,VerticalList", "ScrollToItemCode,HorizontalGrid", "ScrollToItemCode,VerticalGrid",
		  }, 19, 3)]
		[TestCase("Snap Points", new string[] { "SnapPointsCode,HorizontalList", "SnapPointsCode,VerticalList", "SnapPointsCode,HorizontalGrid", "SnapPointsCode,VerticalGrid" }, 19, 2)]
		[TestCase("Observable Collection", new string[] { "FilterItems", "Add/RemoveItemsList", "Add/RemoveItemsGrid" }, 19, 6)]
		[TestCase("Default Text", new string[] { "VerticalListCode", "HorizontalListCode", "VerticalGridCode", "HorizontalGridCode" }, 101, 11)]
		[TestCase("DataTemplate", new string[] { "VerticalListCode", "HorizontalListCode", "VerticalGridCode", "HorizontalGridCode" }, 19, 6)]
		public void VisitAndUpdateItemsSource(string collectionTestName, string[] subGalleries, int firstItem, int lastItem)
		{
			VisitInitialGallery(collectionTestName);

			foreach (var gallery in subGalleries)
			{
				if (gallery == "FilterItems")
				{

				}
				else
				{
					VisitSubGallery(gallery, !gallery.Contains("Horizontal"), $"Item: {firstItem}", $"Item: {lastItem}", lastItem - 1);
					App.NavigateBack();
				}
			}
		}

		void VisitInitialGallery(string collectionTestName)
		{
			var galeryName = $"{collectionTestName} Galleries";
			App.WaitForElement(t => t.Marked(_enableCollectionView));
			App.Tap(t => t.Marked(_enableCollectionView));

			App.WaitForElement(t => t.Marked(galeryName));
			App.Tap(t => t.Marked(galeryName));
		}

		void VisitSubGallery(string galleryName, bool scrollDown, string lastItem, string firstPageItem, int updateItemsCount, bool testItemSource = true)
		{
			App.WaitForElement(t => t.Marked(galleryName));
			App.Tap(t => t.Marked(galleryName));

			//let's test the update
			if (testItemSource)
			{
				UITest.Queries.AppRect collectionViewFrame = TestItemsExist(scrollDown, lastItem);
				TestUpdateItemsWorks(scrollDown, firstPageItem, updateItemsCount.ToString(), collectionViewFrame);
			}
		}

		void TestUpdateItemsWorks(bool scrollDown, string itemMarked, string updateItemsCount, UITest.Queries.AppRect collectionViewFrame)
		{
			App.WaitForElement(t => t.Marked(_entryUpdate));
			App.ScrollForElement($"* marked:'{itemMarked}'", new Drag(collectionViewFrame, scrollDown ? Drag.Direction.TopToBottom : Drag.Direction.LeftToRight, Drag.DragLength.Long), 50);

			App.ClearText(_entryUpdate);
			App.EnterText(_entryUpdate, updateItemsCount);
			App.DismissKeyboard();
			App.Tap(_btnUpdate);
			App.WaitForNoElement(t => t.Marked(itemMarked));
		}

		UITest.Queries.AppRect TestItemsExist(bool scrollDown, string itemMarked)
		{
			App.WaitForElement(t => t.Marked(_btnUpdate));

			var collectionViewFrame = App.Query(q => q.Marked(_collectionViewId))[0].Rect;
			App.ScrollForElement($"* marked:'{itemMarked}'", new Drag(collectionViewFrame, scrollDown ? Drag.Direction.BottomToTop : Drag.Direction.RightToLeft, Drag.DragLength.Long));
			return collectionViewFrame;
		}
	}
}