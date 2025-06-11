using UnityEngine;
#if ENABLED_UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace YooX {
    public static class NumberExtensions {
        /// <summary>
        /// Calculates the percentage that <paramref name="part"/> is of <paramref name="whole"/>.
        /// Returns 0 if <paramref name="whole"/> is zero to avoid division by zero.
        /// </summary>
        /// <param name="part">The part value.</param>
        /// <param name="whole">The whole value.</param>
        /// <returns>The percentage as a float.</returns>
        public static float PercentageOf(this int part, int whole) {
            if (whole == 0)
                return 0; // Handling division by zero
            return (float)part / whole;
        }

        /// <summary>
        /// Determines whether two float values are approximately equal using Unity's Mathf.Approximately.
        /// </summary>
        /// <param name="f1">The first float value.</param>
        /// <param name="f2">The second float value.</param>
        /// <returns>True if the values are approximately equal; otherwise, false.</returns>
        public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);

        /// <summary>
        /// Determines whether the integer is odd.
        /// </summary>
        /// <param name="i">The integer value.</param>
        /// <returns>True if the value is odd; otherwise, false.</returns>
        public static bool IsOdd(this int i) => i % 2 == 1;

        /// <summary>
        /// Determines whether the integer is even.
        /// </summary>
        /// <param name="i">The integer value.</param>
        /// <returns>True if the value is even; otherwise, false.</returns>
        public static bool IsEven(this int i) => i % 2 == 0;

        /// <summary>
        /// Returns the greater of the integer value and the specified minimum.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The minimum value.</param>
        /// <returns>The greater value.</returns>
        public static int AtLeast(this int value, int min) => Mathf.Max(value, min);

        /// <summary>
        /// Returns the lesser of the integer value and the specified maximum.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The lesser value.</returns>
        public static int AtMost(this int value, int max) => Mathf.Min(value, max);

#if ENABLED_UNITY_MATHEMATICS
/// <summary>
/// Returns the greater of the half value and the specified minimum.
/// </summary>
/// <param name="value">The value to compare.</param>
/// <param name="max">The minimum value.</param>
/// <returns>The greater value.</returns>
public static half AtLeast(this half value, half max) => MathfExtension.Max(value, max);

/// <summary>
/// Returns the lesser of the half value and the specified maximum.
/// </summary>
/// <param name="value">The value to compare.</param>
/// <param name="max">The maximum value.</param>
/// <returns>The lesser value.</returns>
public static half AtMost(this half value, half max) => MathfExtension.Min(value, max);
#endif

        /// <summary>
        /// Returns the greater of the float value and the specified minimum.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The minimum value.</param>
        /// <returns>The greater value.</returns>
        public static float AtLeast(this float value, float min) => Mathf.Max(value, min);

        /// <summary>
        /// Returns the lesser of the float value and the specified maximum.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The lesser value.</returns>
        public static float AtMost(this float value, float max) => Mathf.Min(value, max);

        /// <summary>
        /// Returns the greater of the double value and the specified minimum.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The minimum value.</param>
        /// <returns>The greater value.</returns>
        public static double AtLeast(this double value, double min) => MathfExtension.Max(value, min);

        /// <summary>
        /// Returns the lesser of the double value and the specified maximum.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The maximum value.</param>
        /// <returns>The lesser value.</returns>
        public static double AtMost(this double value, double min) => MathfExtension.Min(value, min);
    }

}
