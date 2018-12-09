using NUnit.Framework;

namespace Xamarin.Forms.Core.UITests
{
	[Category(UITestCategories.CollectionView)]
	internal class CollectionViewUITests : BaseTestFixture
	{
		string _defaultTextGalleries = "Default Text Galleries";
		string _defaultTextGalleriesVerticalListCode = "VerticalListCode";
		string _defaultTextGalleriesHorizontalListCode = "HorizontalListCode";
		string _defaultTextGalleriesVerticalGridCode = "VerticalGridCode";
		string _defaultTextGalleriesHorizontalLGridCode = "HorizontalGridCode";
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
			//ResetApp();
			//NavigateToGallery();
		}

		[Test]
		public void DefaultTextGalleries()
		{
			App.WaitForElement(t => t.Marked(_enableCollectionView));
			App.Tap(t => t.Marked(_enableCollectionView));

			App.WaitForElement(t => t.Marked(_defaultTextGalleries));
			App.Tap(t => t.Marked(_defaultTextGalleries));

			App.WaitForElement(t => t.Marked(_defaultTextGalleriesVerticalListCode));
			App.Tap(t => t.Marked(_defaultTextGalleriesVerticalListCode));

			App.WaitForElement(t => t.Marked(_btnUpdate));
			//App.Repl();
			App.WaitForElement(t => t.Marked(_entryUpdate));
			App.EnterText(_entryUpdate, "50");
			App.Tap(t => t.Marked(_btnUpdate));

			App.NavigateBack();

			App.WaitForElement(t => t.Marked(_defaultTextGalleriesHorizontalListCode));
			App.Tap(t => t.Marked(_defaultTextGalleriesHorizontalListCode));

			App.NavigateBack();


			App.WaitForElement(t => t.Marked(_defaultTextGalleriesVerticalGridCode));
			App.Tap(t => t.Marked(_defaultTextGalleriesVerticalGridCode));


			App.NavigateBack();

			App.WaitForElement(t => t.Marked(_defaultTextGalleriesHorizontalLGridCode));
			App.Tap(t => t.Marked(_defaultTextGalleriesHorizontalLGridCode));

			App.NavigateBack();



			//App.WaitForElement("Appearing NavAppearingPage");
			//App.WaitForElement("Appearing Page 1");
			//App.Tap(t => t.Marked("Push new Page"));
			//App.WaitForElement("Disappearing Page 1");
			//App.WaitForElement("Appearing Page 2");
			//App.Tap(t => t.Marked("Change Main Page"));
			//App.WaitForElement("Disappearing Page 2");
			//App.WaitForElement("Disappearing NavAppearingPage");
			//App.WaitForElement("Appearing Page 3");
		}
	}
}