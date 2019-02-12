using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class SelectableItemsView : ItemsView
	{
		public static readonly BindableProperty SelectionModeProperty =
			BindableProperty.Create(nameof(SelectionMode), typeof(SelectionMode), typeof(SelectableItemsView),
				SelectionMode.None);

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SelectableItemsView), default(object),
				propertyChanged: SelectedItemPropertyChanged);

		public static readonly BindableProperty SelectedItemsProperty =
			BindableProperty.Create(nameof(SelectedItems), typeof(List<object>), typeof(SelectableItemsView), new List<object>(),
				propertyChanged: SelectedItemsPropertyChanged);

		public static readonly BindableProperty SelectionChangedCommandProperty =
			BindableProperty.Create(nameof(SelectionChangedCommand), typeof(ICommand), typeof(SelectableItemsView));

		public static readonly BindableProperty SelectionChangedCommandParameterProperty =
			BindableProperty.Create(nameof(SelectionChangedCommandParameter), typeof(object),
				typeof(SelectableItemsView));

		public object SelectedItem
		{
			get => GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public List<object> SelectedItems
		{
			get => (List<object>)GetValue(SelectedItemsProperty);
			set => SetValue(SelectedItemsProperty, value);
		}

		public ICommand SelectionChangedCommand
		{
			get => (ICommand)GetValue(SelectionChangedCommandProperty);
			set => SetValue(SelectionChangedCommandProperty, value);
		}

		public object SelectionChangedCommandParameter
		{
			get => GetValue(SelectionChangedCommandParameterProperty);
			set => SetValue(SelectionChangedCommandParameterProperty, value);
		}

		public SelectionMode SelectionMode
		{
			get => (SelectionMode)GetValue(SelectionModeProperty);
			set => SetValue(SelectionModeProperty, value);
		}

		public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

		protected virtual void OnSelectionChanged(SelectionChangedEventArgs args)
		{
		}

		static void SelectionPropertyChanged(SelectableItemsView selectableItemsView, SelectionChangedEventArgs args)
		{
			var command = selectableItemsView.SelectionChangedCommand;

			if (command != null)
			{
				var commandParameter = selectableItemsView.SelectionChangedCommandParameter;

				if (command.CanExecute(commandParameter))
				{
					command.Execute(commandParameter);
				}
			}
			
			selectableItemsView.SelectionChanged?.Invoke(selectableItemsView, args);

			selectableItemsView.OnSelectionChanged(args);
		}

		static void SelectedItemPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var selectableItemsView = (SelectableItemsView)bindable;

			var args = new SelectionChangedEventArgs(oldValue, newValue);

			SelectionPropertyChanged(selectableItemsView, args);
		}

		private static void SelectedItemsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var selectableItemsView = (SelectableItemsView)bindable;

			var oldList = (List<object>)oldValue;
			var newList = (List<object>)newValue;

			var args = new SelectionChangedEventArgs(oldList, newList);

			SelectionPropertyChanged(selectableItemsView, args);
		}
	}
}
