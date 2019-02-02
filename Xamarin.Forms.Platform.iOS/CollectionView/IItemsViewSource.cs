namespace Xamarin.Forms.Platform.iOS
{
	public interface IItemsViewSource
	{
		int Count { get; }
		object this[int itemIndex] { get; }
	}

	public interface IGroupedItemsViewSource : IItemsViewSource
	{
		int GroupCount { get; }
		object this[int groupIndex, int itemIndex] { get; }
	}
}