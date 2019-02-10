using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms
{
	public static class PickerElement
	{
		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create(nameof(IPickerElement.Title), typeof(string), typeof(IPickerElement), default(string));

		public static readonly BindableProperty TitleColorProperty =
			BindableProperty.Create(nameof(IPickerElement.TitleColor), typeof(Color), typeof(IPickerElement), default(Color));
	}
}
