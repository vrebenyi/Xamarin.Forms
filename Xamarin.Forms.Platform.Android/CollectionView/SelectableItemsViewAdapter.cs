using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android
{
	public class SelectableItemsViewAdapter : ItemsViewAdapter
	{
		protected readonly SelectableItemsView SelectableItemsView;

		internal SelectableItemsViewAdapter(SelectableItemsView selectableItemsView, 
			Func<IVisualElementRenderer, Context, global::Android.Views.View> createView = null) : base(selectableItemsView, createView)
		{
			SelectableItemsView = selectableItemsView;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			base.OnBindViewHolder(holder, position);

			if (!(holder is SelectableViewHolder selectable))
			{
				return;
			}

			// Watch for clicks so the user can select the item held by this ViewHolder
			selectable.Clicked += SelectableOnClicked;

			var selectedItem = SelectableItemsView.SelectedItem;
			if (selectedItem == null)
			{
				return;
			}

			// If there's a selected item, check to see if it's this one so we can mark it 'selected'
			if (GetPositionForItem(selectedItem) == position)
			{
				selectable.IsSelected = true;
			}
		}
	
		public override void OnViewRecycled(Object holder)
		{
			if (holder is SelectableViewHolder selectable)
			{
				selectable.Clicked -= SelectableOnClicked;
				selectable.IsSelected = false;
			}

			base.OnViewRecycled(holder);
		}

		void SelectableOnClicked(object sender, int adapterPosition)
		{
			UpdateFormsSelection(adapterPosition);
		}

		void UpdateFormsSelection(int adapterPosition)
		{
			var mode = SelectableItemsView.SelectionMode;

			switch (mode)
			{
				case SelectionMode.None:
					// Selection's not even on, so there's nothing to do here
					return;
				case SelectionMode.Single:
					SelectableItemsView.SelectedItem = ItemsSource[adapterPosition];
					return;
				case SelectionMode.Multiple:

					var currentSelection = new List<object>();

					foreach(var selectedItem in SelectableItemsView.SelectedItems)
					{
						currentSelection.Add(selectedItem);
					}

					var item = ItemsSource[adapterPosition];

					if(currentSelection.Contains(item))
					{
						currentSelection.Remove(item);
					}
					else
					{
						currentSelection.Add(item);
					}

					SelectableItemsView.SelectedItems = currentSelection;
					
					return;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}