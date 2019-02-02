using System;

namespace Xamarin.Forms.Platform.iOS
{
	internal class EmptySource : IGroupedItemsViewSource
	{
		public int Count => 0;

		public int GroupCount => 0;

		public object this[int groupIndex, int itemIndex] => throw new IndexOutOfRangeException("IItemsViewSource is empty");

		public object this[int index] => throw new IndexOutOfRangeException("IItemsViewSource is empty");
	}
}