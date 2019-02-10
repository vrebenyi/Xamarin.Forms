using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Orientation = Android.Widget.Orientation;
using Android.Content;
using AColor = Android.Graphics.Color;
using Android.Text;
using Android.Text.Style;

namespace Xamarin.Forms.Platform.Android
{
	public abstract class PickerRendererBase<TControl> : ViewRenderer<Picker, TControl>, IPickerRenderer
		where TControl : global::Android.Views.View
	{
		AlertDialog _dialog;
		bool _isDisposed;
		TextColorSwitcher _textColorSwitcher;
		int _originalHintTextColor;

		public PickerRendererBase(Context context) : base(context)
		{
			AutoPackage = false;
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use PickerRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PickerRendererBase()
		{
			AutoPackage = false;
		}

		protected abstract EditText EditText { get; }

		IElementController ElementController => Element as IElementController;

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_isDisposed)
			{
				_isDisposed = true;
				((INotifyCollectionChanged)Element.Items).CollectionChanged -= RowsCollectionChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			if (e.OldElement != null)
				((INotifyCollectionChanged)e.OldElement.Items).CollectionChanged -= RowsCollectionChanged;

			if (e.NewElement != null)
			{
				((INotifyCollectionChanged)e.NewElement.Items).CollectionChanged += RowsCollectionChanged;
				if (Control == null)
				{
					var textField = CreateNativeControl();
					SetNativeControl(textField);

					var useLegacyColorManagement = e.NewElement.UseLegacyColorManagement();
					_textColorSwitcher = new TextColorSwitcher(EditText.TextColors, useLegacyColorManagement);

					_originalHintTextColor = EditText.CurrentHintTextColor;
				}

				UpdateFont();
				UpdatePicker();
				UpdateTextColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Picker.TitleProperty.PropertyName || e.PropertyName == Picker.TitleColorProperty.PropertyName)
				UpdatePicker();
			else if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
				UpdatePicker();
			else if (e.PropertyName == Picker.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == Picker.FontAttributesProperty.PropertyName || e.PropertyName == Picker.FontFamilyProperty.PropertyName || e.PropertyName == Picker.FontSizeProperty.PropertyName)
				UpdateFont();
		}

		protected override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			base.OnFocusChangeRequested(sender, e);

			if (e.Focus)
				CallOnClick();
			else if (_dialog != null)
			{
				_dialog.Hide();
				ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
				_dialog = null;
			}
		}

		void IPickerRenderer.OnClick()
		{
			Picker model = Element;

			var picker = new NumberPicker(Context);
			if (model.Items != null && model.Items.Any())
			{
				picker.MaxValue = model.Items.Count - 1;
				picker.MinValue = 0;
				picker.SetDisplayedValues(model.Items.ToArray());
				picker.WrapSelectorWheel = false;
				picker.DescendantFocusability = DescendantFocusability.BlockDescendants;
				picker.Value = model.SelectedIndex;
			}

			var layout = new LinearLayout(Context) { Orientation = Orientation.Vertical };
			layout.AddView(picker);

			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);

			var builder = new AlertDialog.Builder(Context);
			builder.SetView(layout);
			builder.SetTitle(PickerManager.GetTitle(model.TitleColor, model.Title));

			builder.SetNegativeButton(global::Android.Resource.String.Cancel, (s, a) =>
			{
				ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
				_dialog = null;
			});
			builder.SetPositiveButton(global::Android.Resource.String.Ok, (s, a) =>
			{
				ElementController.SetValueFromRenderer(Picker.SelectedIndexProperty, picker.Value);
				// It is possible for the Content of the Page to be changed on SelectedIndexChanged. 
				// In this case, the Element & Control will no longer exist.
				if (Element != null)
				{
					if (model.Items.Count > 0 && Element.SelectedIndex >= 0)
						EditText.Text = model.Items[Element.SelectedIndex];
					ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
				}
				_dialog = null;
			});

			_dialog = builder.Create();			
			_dialog.DismissEvent += (sender, args) =>
			{
				ElementController?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
			};
			_dialog.Show();
		}

		void RowsCollectionChanged(object sender, EventArgs e)
		{
			UpdatePicker();
		}

		void UpdateFont()
		{
			EditText.Typeface = Element.ToTypeface();
			EditText.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
		}

		void UpdatePicker()
		{
			UpdatePlaceHolderText();
			UpdateTitleColor();

			string oldText = EditText.Text;

			if (Element.SelectedIndex == -1 || Element.Items == null || Element.SelectedIndex >= Element.Items.Count)
				EditText.Text = null;
			else
				EditText.Text = Element.Items[Element.SelectedIndex];

			if (oldText != EditText.Text)
				((IVisualElementController)Element).NativeSizeChanged();
		}

		protected internal virtual void UpdatePlaceHolderText()
		{
			EditText.Hint = Element.Title;
		}

		abstract protected void UpdateTextColor();
		internal protected virtual void UpdateTitleColor()
		{
			if (Element.IsSet(Picker.TitleColorProperty))
				EditText.SetHintTextColor(Element.TitleColor.ToAndroid());
			else
				EditText.SetHintTextColor(new AColor(_originalHintTextColor));
		}
	}


	public class PickerRenderer : PickerRendererBase<EditText>
	{
		private TextColorSwitcher _textColorSwitcher;

		[Obsolete("This constructor is obsolete as of version 2.5. Please use PickerRenderer(Context) instead.")]
		public PickerRenderer()
		{
		}

		public PickerRenderer(Context context) : base(context)
		{
		}

		protected override EditText CreateNativeControl()
		{
			return new PickerEditText(Context);
		}

		protected override EditText EditText => Control;

		protected override void UpdateTextColor()
		{
			_textColorSwitcher = _textColorSwitcher ?? new TextColorSwitcher(EditText.TextColors, Element.UseLegacyColorManagement());
			_textColorSwitcher.UpdateTextColor(EditText, Element.TextColor);
		}
	}
}