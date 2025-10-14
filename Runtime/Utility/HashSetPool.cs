using System;
using System.Collections.Generic;

namespace Zenvin.Services.Utility
{
	public static class HashSetPool<T>
	{
		private static readonly Stack<HashSet<T>> sets = new Stack<HashSet<T>> ();

		internal static IDisposable Get (out HashSet<T> set)
		{
			set = sets.TryPop (out var _set) ? _set : new HashSet<T> ();
			return new Entry (set);
		}


		private readonly struct Entry : IDisposable
		{
			private readonly HashSet<T> set;

			internal Entry (HashSet<T> set)
			{
				this.set = set;
			}

			void IDisposable.Dispose ()
			{
				if (set == null)
					return;

				set.Clear ();
				sets.Push (set);
			}
		}
	}
}
