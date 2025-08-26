using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace YooX {
	static public class UQueryBuilderExtensions {
		/// <summary>
		/// Sorts the elements of a sequence in ascending order according 
		/// to a key and returns an ordered sequence.
		/// </summary>
		/// <param name="query">The elements to be sorted.</param>
		/// <param name="keySelector">A function to extract a sort key from an element.</param>
		/// <param name="default">The Comparer to compare keys.</param>
		static public IEnumerable<T> OrderBy<T, TKey>(
		this UQueryBuilder<T> query,
		Func<T, TKey> keySelector,
		Comparer<TKey> @default
		)
			where T : VisualElement =>
			query.ToList().OrderBy(keySelector, @default);

		/// <summary>
		/// Sorts the elements of a sequence in ascending order according 
		/// to a numeric key and returns an ordered sequence.
		/// </summary>
		/// <param name="query">The elements to be sorted.</param>
		/// <param name="keySelector">A function to extract a numeric key from an element.</param>
		static public IEnumerable<T> SortByNumericValue<T>(this UQueryBuilder<T> query, Func<T, float> keySelector)
			where T : VisualElement =>
			query.OrderBy(keySelector, Comparer<float>.Default);

		/// <summary>
		/// Returns the first element of a sequence, or a default value if no element is found.
		/// </summary>
		/// <param name="query">The elements to search in.</param>
		static public T FirstOrDefault<T>(this UQueryBuilder<T> query)
			where T : VisualElement =>
			query.ToList().FirstOrDefault();

		/// <summary>
		/// Counts the number of elements in the sequence that satisfy the condition specified by the predicate function.
		/// </summary>
		/// <param name="query">The sequence of elements to be processed.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		static public int CountWhere<T>(this UQueryBuilder<T> query, Func<T, bool> predicate)
			where T : VisualElement =>
			query.ToList().Count(predicate);
	}
}