using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;

namespace Xamarin.Forms.Platform.iOS
{
	internal class ListSource : List<object>, IItemsViewSource
	{
		public ListSource()
		{
		}

		public ListSource(IEnumerable<object> enumerable) : base(enumerable)
		{
			
		}

		public ListSource(IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				Add(item);
			}
		}

		public object this[NSIndexPath indexPath]
		{
			get
			{
				if (indexPath.Section > 0)
				{
					throw new ArgumentOutOfRangeException(nameof(indexPath));
				}

				return this[indexPath.Row];
			}
		}

		public int GroupCount => 1;

		public int ItemCount => Count;

		public object Group(NSIndexPath indexPath)
		{
			return null;
		}

		public int ItemCountInGroup(nint group)
		{
			if (group > 0)
			{
				throw new ArgumentOutOfRangeException(nameof(group));
			}

			return Count;
		}
	}
}