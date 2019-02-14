using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Xamarin.Forms
{
	internal class SelectionList : IList<object>
	{
		private readonly SelectableItemsView _selectableItemsView;
		private List<object> _internal;
		static readonly IList<object> s_empty = new List<object>(0);

		public SelectionList(SelectableItemsView selectableItemsView)
		{
			_selectableItemsView = selectableItemsView ?? throw new ArgumentNullException(nameof(selectableItemsView));
			_internal = new List<object>();
		}

		public object this[int index] { get => _internal[index]; set => _internal[index] = value; }

		public int Count => _internal.Count;
		public bool IsReadOnly => false;

		public void Add(object item)
		{
			var oldItems = Copy();

			_internal.Add(item);

			_selectableItemsView.SelectedItemsPropertyChanged(oldItems, Copy());
		}

		public void Clear()
		{
			var oldItems = Copy();

			_internal.Clear();

			_selectableItemsView.SelectedItemsPropertyChanged(oldItems, s_empty);
		}

		public bool Contains(object item)
		{
			return _internal.Contains(item);
		}

		public void CopyTo(object[] array, int arrayIndex)
		{
			_internal.CopyTo(array, arrayIndex);
		}

		public IEnumerator<object> GetEnumerator()
		{
			return _internal.GetEnumerator();
		}

		public int IndexOf(object item)
		{
			return _internal.IndexOf(item);
		}

		public void Insert(int index, object item)
		{
			var oldItems = Copy();

			_internal.Insert(index, item);

			_selectableItemsView.SelectedItemsPropertyChanged(oldItems, Copy());
		}

		public bool Remove(object item)
		{
			var oldItems = Copy();

			var removed = _internal.Remove(item);

			if (removed)
			{
				_selectableItemsView.SelectedItemsPropertyChanged(oldItems, Copy());
			}

			return removed;
		}

		public void RemoveAt(int index)
		{
			var oldItems = Copy();

			_internal.RemoveAt(index);

			_selectableItemsView.SelectedItemsPropertyChanged(oldItems, Copy());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _internal.GetEnumerator();
		}

		List<object> Copy()
		{
			var items = new List<object>();
			for (int n = 0; n < _internal.Count; n++)
			{
				items.Add(_internal[n]);
			}

			return items;
		}
	}

	public class SelectableItemsView : ItemsView
	{
		public static readonly BindableProperty SelectionModeProperty =
			BindableProperty.Create(nameof(SelectionMode), typeof(SelectionMode), typeof(SelectableItemsView),
				SelectionMode.None);

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SelectableItemsView), default(object),
				propertyChanged: SelectedItemPropertyChanged);

		public static readonly BindablePropertyKey SelectedItemsPropertyKey =
			BindableProperty.CreateReadOnly(nameof(SelectedItems), typeof(IList<object>), typeof(SelectableItemsView), null);

		public static readonly BindableProperty SelectedItemsProperty = SelectedItemsPropertyKey.BindableProperty;

		public static readonly BindableProperty SelectionChangedCommandProperty =
			BindableProperty.Create(nameof(SelectionChangedCommand), typeof(ICommand), typeof(SelectableItemsView));

		public static readonly BindableProperty SelectionChangedCommandParameterProperty =
			BindableProperty.Create(nameof(SelectionChangedCommandParameter), typeof(object),
				typeof(SelectableItemsView));

		public SelectableItemsView()
		{
			var selectionList = new SelectionList(this);
			SetValue(SelectedItemsPropertyKey, selectionList);
		}

		public object SelectedItem
		{
			get => GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public IList<object> SelectedItems
		{
			get => (IList<object>)GetValue(SelectedItemsProperty);
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

		internal void SelectedItemsPropertyChanged(IList<object> oldSelection, IList<object> newSelection)
		{
			SelectionPropertyChanged(this, new SelectionChangedEventArgs(oldSelection, newSelection));
			OnPropertyChanged(SelectedItemsProperty.PropertyName);
		}
	}
}
