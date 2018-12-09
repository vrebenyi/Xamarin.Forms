using System;

namespace Xamarin.Forms.Controls.GalleryPages
{
	public static class GalleryBuilder
	{
		public static Button NavButton(string galleryName, Func<ContentPage> gallery, INavigation nav)
		{
			var automationId = galleryName.Replace(" ", "").Replace("(", "").Replace(")", "");
			var button = new Button { Text = $"{galleryName}", AutomationId = automationId };
			button.Clicked += (sender, args) => { nav.PushAsync(gallery()); };
			return button;
		}
	}
}