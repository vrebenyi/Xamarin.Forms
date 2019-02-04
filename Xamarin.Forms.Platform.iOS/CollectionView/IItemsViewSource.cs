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
		int CountInGroup(int group);
		object this[Foundation.NSIndexPath indexPath] { get; }
	}
}


// TODO save you progress and try merging these interfaces