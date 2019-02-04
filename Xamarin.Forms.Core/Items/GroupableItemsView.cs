namespace Xamarin.Forms
{
	public class GroupableItemsView : SelectableItemsView
	{
		public static readonly BindableProperty IsGroupingEnabledProperty =
			BindableProperty.Create(nameof(IsGroupingEnabled), typeof(bool), typeof(GroupableItemsView), false);

		public bool IsGroupingEnabled
		{
			get => (bool)GetValue(IsGroupingEnabledProperty);
			set => SetValue(IsGroupingEnabledProperty, value);
		}

		public static readonly BindableProperty HeaderTemplateProperty =
			BindableProperty.Create(nameof(HeaderTemplate), typeof(DataTemplate), typeof(GroupableItemsView));

		public DataTemplate HeaderTemplate
		{
			get => (DataTemplate)GetValue(HeaderTemplateProperty);
			set => SetValue(HeaderTemplateProperty, value);
		}

		public static readonly BindableProperty FooterTemplateProperty =
			BindableProperty.Create(nameof(FooterTemplate), typeof(DataTemplate), typeof(GroupableItemsView));

		public DataTemplate FooterTemplate
		{
			get => (DataTemplate)GetValue(FooterTemplateProperty);
			set => SetValue(FooterTemplateProperty, value);
		}
	}
}
