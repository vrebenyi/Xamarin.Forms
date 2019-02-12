using System;
using System.ComponentModel;
using Android.Content;

namespace Xamarin.Forms.Platform.Android
{
	public class SelectableItemsViewRenderer : ItemsViewRenderer
	{
		SelectableItemsView SelectableItemsView => (SelectableItemsView)ItemsView;

		SelectableItemsViewAdapter SelectableItemsViewAdapter => (SelectableItemsViewAdapter)ItemsViewAdapter; 

		public SelectableItemsViewRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);
			
			if (changedProperty.IsOneOf(SelectableItemsView.SelectedItemProperty, SelectableItemsView.SelectedItemsProperty))
			{
				UpdateNativeSelection();
			}
		}

		protected override void SetUpNewElement(ItemsView newElement)
		{
			if (newElement != null && !(newElement is SelectableItemsView))
			{
				throw new ArgumentException($"{nameof(newElement)} must be of type {typeof(SelectableItemsView).Name}");
			}

			base.SetUpNewElement(newElement);

			UpdateNativeSelection();
		}

		protected override void UpdateAdapter()
		{
			ItemsViewAdapter = new SelectableItemsViewAdapter(SelectableItemsView);
			SwapAdapter(ItemsViewAdapter, true);
		}

		void ClearSelection()
		{
			for (int i = 0, size = ChildCount; i < size; i++)
			{
				var holder = GetChildViewHolder(GetChildAt(i));
				
				if (holder is SelectableViewHolder selectable)
				{
					selectable.IsSelected = false;
				}
			}
		}

		void MarkItemSelected(object selectedItem)
		{
			if(selectedItem == null)
			{
				return;
			}

			var position = ItemsViewAdapter.GetPositionForItem(selectedItem);
			var selectedHolder = FindViewHolderForAdapterPosition(position);
			if (selectedHolder == null)
			{
				return;
			}

			if (selectedHolder is SelectableViewHolder selectable)
			{
				selectable.IsSelected = true;
			}
		}

		void UpdateNativeSelection()
		{
			var mode = SelectableItemsView.SelectionMode;

			if(mode == SelectionMode.None)
			{
				ClearSelection();
				return;
			}

			if(mode == SelectionMode.Single)
			{
				var selectedItem = SelectableItemsView.SelectedItem;

				ClearSelection();
				MarkItemSelected(selectedItem);
				return;
			}

			var selectedItems = SelectableItemsView.SelectedItems;

			ClearSelection();

			foreach(var item in selectedItems)
			{
				MarkItemSelected(item);
			}
		}
	}
}