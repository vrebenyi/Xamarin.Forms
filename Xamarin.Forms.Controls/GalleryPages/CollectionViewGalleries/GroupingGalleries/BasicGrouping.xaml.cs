using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.GroupingGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	[Preserve (AllMembers = true)]
	public partial class BasicGrouping : ContentPage
	{
		
		
		// TODO Also need a gallery where we can turn grouping on/off to make sure that doesn't blow up

		// TODO Also need a gallery with no templates, so it falls back to the default cell stuff (make sure the font looks right)

		public BasicGrouping ()
		{
			InitializeComponent ();

			CollectionView.ItemsSource = new SuperTeams();

			CollectionView.IsGroupingEnabled = true;
		}
	}
}