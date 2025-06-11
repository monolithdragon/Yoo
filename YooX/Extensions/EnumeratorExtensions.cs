using System.Collections.Generic;

namespace YooX {
    public static class EnumeratorExtensions {
        /// <summary>
        /// Converts an <see cref="IEnumerator{T}"/> to an <see cref="IEnumerable{T}"/>, 
        /// allowing enumeration from the current position to the end.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
        /// <param name="e">The enumerator to convert.</param>
        /// <returns>An enumerable sequence starting from the enumerator's current position.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> e) {
            while (e.MoveNext()) {
                yield return e.Current;
            }
        }
    }

}
