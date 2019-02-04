using System;

namespace Xamarin.Forms.Platform.iOS
{
	public interface IItemsViewSource
	{
		int ItemCount { get; }
		int ItemCountInGroup(nint group);
		int GroupCount { get; }
		object this[Foundation.NSIndexPath indexPath] { get; }
		object Group(Foundation.NSIndexPath indexPath);
	}

	//public interface IGroupedItemsViewSource : IItemsViewSource
	//{
	//	int GroupCount { get; }
	//	int CountInGroup(int group);
		
	//}
}


// TODO save your progress and try merging these interfaces