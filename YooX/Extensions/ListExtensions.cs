using System;
using System.Collections.Generic;
using System.Linq;

namespace YooX {
	public static class ListExtensions {
		private static Random _rng;

		/// <summary>
		/// Determines whether a collection is null or has no elements
		/// without having to enumerate the entire collection to get a count.
		///
		/// Uses LINQ's Any() method to determine if the collection is empty,
		/// so there is some GC overhead.
		/// </summary>
		/// <param name="list">List to evaluate</param>
		public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || !list.Any();

		/// <summary>
		/// Creates a new list that is a copy of the original list.
		/// </summary>
		/// <param name="list">The original list to be copied.</param>
		/// <returns>A new list that is a copy of the original list.</returns>
		public static List<T> Clone<T>(this IList<T> list) {
			var newList = new List<T>();
			newList.AddRange(list);
			return newList;
		}

		/// <summary>
		/// Swaps two elements in the list at the specified indices.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="indexA">The index of the first element.</param>
		/// <param name="indexB">The index of the second element.</param>
		public static void Swap<T>(this IList<T> list, int indexA, int indexB) {
			(list[indexA], list[indexB]) = (list[indexB], list[indexA]);
		}

		/// <summary>
		/// Shuffles the elements in the list using the Durstenfeld implementation of the Fisher-Yates algorithm.
		/// This method modifies the input list in-place, ensuring each permutation is equally likely, and returns the list for method chaining.
		/// Reference: http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
		/// </summary>
		/// <param name="list">The list to be shuffled.</param>
		/// <typeparam name="T">The type of the elements in the list.</typeparam>
		/// <returns>The shuffled list.</returns>
		public static IList<T> Shuffle<T>(this IList<T> list) {
			_rng ??= new Random();
			int count = list.Count;
			while(count > 1) {
				--count;
				int index = _rng.Next(count + 1);
				(list[index], list[count]) = (list[count], list[index]);
			}
			return list;
		}

		/// <summary>
		/// Filters a collection based on a predicate and returns a new list
		/// containing the elements that match the specified condition.
		/// </summary>
		/// <param name="source">The collection to filter.</param>
		/// <param name="predicate">The condition that each element is tested against.</param>
		/// <returns>A new list containing elements that satisfy the predicate.</returns>
		public static IList<T> Filter<T>(this IList<T> source, Predicate<T> predicate) {
			return source.Where(item => predicate(item)).ToList();
		}

		/// <summary>
		/// Replaces the contents of the list with the specified items from an IEnumerable<T>.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The list to refresh.</param>
		/// <param name="items">The collection of items to add to the list.</param>
		public static void RefreshWith<T>(this List<T> list, IEnumerable<T> items) {
			list.Clear();
			list.AddRange(items);
		}

	}
}