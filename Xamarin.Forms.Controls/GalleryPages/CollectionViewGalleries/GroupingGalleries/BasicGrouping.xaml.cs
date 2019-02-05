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
		// TODO hartez Need at least one gallery where the first group is empty
		// to make sure we can find a cell to prototype properly
		
		// TODO Also need a gallery where we can turn grouping on/off to make sure that doesn't blow up

		// TODO Also need a gallery with no templates, so it falls back to the default cell stuff (make sure the font looks right)

		public BasicGrouping ()
		{
			InitializeComponent ();

			List<Team> teams = new List<Team>();

			teams.Add(new Team("Avengers", 
				new List<Member>
				{
					new Member("Thor"),
					new Member("Captain America")
				}
			));

			teams.Add(new Team("Fantastic Four", 
				new List<Member>
				{
					new Member("The Thing"),
					new Member("The Human Torch"),
					new Member("The Invisible Woman"),
					new Member("Mr. Fantastic"),
				}
			));

			CollectionView.ItemsSource = teams;

			CollectionView.IsGroupingEnabled = true;
		}

		[Preserve (AllMembers = true)]
		class Team : List<Member>
		{
			public Team(string name, List<Member> members) : base (members)
			{
				Name = name;
			}

			public string Name { get; set; }
		}

		[Preserve (AllMembers = true)]
		class Member
		{
			public Member(string name) => Name = name;

			public string Name { get; set; }
		}
	}
}