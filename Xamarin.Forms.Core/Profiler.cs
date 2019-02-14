using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Internal
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct Profile : IDisposable
	{
		const int Capacity = 1000;
		const string PROFILE = "PROFILE";
		public struct Datum
		{
			public string Name;
			public string Id;
			public long Ticks;
			public int Depth;
			public int Line;
		}
		public static List<Datum> Data = new List<Datum>(Capacity);

		static Stack<Profile> s_stack = new Stack<Profile>(Capacity);
		static int s_depth = 0;
		static bool s_stopped = false;
		static Stopwatch s_stopwatch = new Stopwatch();

		readonly long start;
		readonly string name;
		readonly int slot;
		readonly string id;
		readonly int line;

		public static void Stop()
		{
			// unwind stack
			s_stopped = true;
			while (s_stack.Count > 0)
				s_stack.Pop();
		}
		[Conditional(PROFILE)]
		public static void Push(
			[CallerMemberName] string name = "",
			string id = null,
			[CallerLineNumber] int line = 0)
		{
			if (s_stopped)
				return;

			if (!s_stopwatch.IsRunning)
				s_stopwatch.Start();

			s_stack.Push(new Profile(name, id, line));
		}
		[Conditional(PROFILE)]
		public static void Pop()
		{
			if (s_stopped)
				return;

			var profile = s_stack.Pop();
			profile.Dispose();
		}
		[Conditional(PROFILE)]
		public static void PopPush(
			string id,
			[CallerLineNumber] int line = 0)
		{
			if (s_stopped)
				return;

			var profile = s_stack.Pop();
			var name = profile.name;
			profile.Dispose();

			Push(name, id, line);
		}

		private Profile(
			string name,
			string id = null,
			int line = 0)
		{
			this = default(Profile);
			start = s_stopwatch.ElapsedTicks;

			this.name = name;
			this.id = id;
			this.line = line;

			slot = Data.Count;
			Data.Add(new Datum()
			{
				Depth = s_depth,
				Name = name,
				Id = id,
				Ticks = -1,
				Line = line
			});

			s_depth++;
		}
		public void Dispose()
		{
			if (s_stopped && start == 0)
				return;

			var ticks = s_stopwatch.ElapsedTicks - start;
			var depth = --s_depth;

			var datum = Data[slot];
			datum.Ticks = ticks;
			Data[slot] = datum;
		}
	}
}