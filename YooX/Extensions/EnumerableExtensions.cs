using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YooX {
	public static class EnumerableExtensions {
		/// <summary>
		/// Performs the specified action on each element of the sequence.
		/// </summary>
		/// <typeparam name="T">The type of elements in the sequence.</typeparam>
		/// <param name="sequence">The sequence whose elements the action will be performed on.</param>
		/// <param name="action">The action to perform on each element.</param>
		/// <exception cref="ArgumentNullException">Thrown if the sequence or action is null.</exception>
		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action) {
			if (sequence == null) throw new ArgumentNullException(nameof(sequence));

			foreach (var item in sequence) {
				action(item);
			}
		}


		/// <summary>
		/// Returns a random element from the sequence.
		/// </summary>
		/// <typeparam name="T">The type of elements in the sequence.</typeparam>
		/// <param name="sequence">The sequence to select a random element from.</param>
		/// <returns>A randomly selected element from the sequence.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the sequence is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the sequence is empty.</exception>
		public static T Random<T>(this IEnumerable<T> sequence) {
			if (sequence == null) throw new ArgumentNullException(nameof(sequence));

			if (sequence is IList<T> list) {
				if (list.Count == 0) throw new InvalidOperationException("Cannot get a random element from an empty collection.");
				return list[UnityEngine.Random.Range(0, list.Count)];
			}

			// Use reservoir sampling when the input is not an IList<T> ie: a stream or lazy sequence
			using var enumerator = sequence.GetEnumerator();
			if (!enumerator.MoveNext()) throw new InvalidOperationException("Cannot get a random element from an empty collection.");

			var result = enumerator.Current;
			var count = 1;
			while(enumerator.MoveNext()) {
				if (UnityEngine.Random.Range(0, ++count) == 0) {
					result = enumerator.Current;
				}
			}
			return result;
		}


		/// <summary>
		/// Counts the number of elements in a non-generic <see cref="IEnumerable"/> sequence.
		/// </summary>
		/// <param name="enumerable">The non-generic sequence to count elements from.</param>
		/// <returns>The number of elements in the sequence, or 0 if the sequence is null.</returns>
		public static int CountEnumerable(this IEnumerable enumerable) {
			var count = 0;
			if (enumerable != null) {
				count += enumerable.Cast<object>().Count();
			}
			return count;
		}

	}
}