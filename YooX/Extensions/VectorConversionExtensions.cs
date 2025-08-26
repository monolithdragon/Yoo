using System.Runtime.CompilerServices;
using UnityEngine;

namespace YooX {
	static public class VectorConversionExtensions {
		/// <summary>
		/// Converts a <see cref="System.Numerics.Vector2"/> to a <see cref="Vector2"/>.
		/// </summary>
		/// <param name="vector">The System.Numerics.Vector2 to convert.</param>
		/// <returns>A UnityEngine.Vector2 with the same X and Y values.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public Vector2 ToUnityVector(this System.Numerics.Vector2 vector) => new(vector.X, vector.Y);

		/// <summary>
		/// Converts a <see cref="Vector2"/> to a <see cref="System.Numerics.Vector2"/>.
		/// </summary>
		/// <param name="vector">The UnityEngine.Vector2 to convert.</param>
		/// <returns>A System.Numerics.Vector2 with the same x and y values.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public System.Numerics.Vector2 ToSystemVector(this Vector2 vector) => new(vector.x, vector.y);

		/// <summary>
		/// Converts a <see cref="System.Numerics.Vector3"/> to a <see cref="Vector3"/>.
		/// </summary>
		/// <param name="vector">The System.Numerics.Vector3 to convert.</param>
		/// <returns>A UnityEngine.Vector3 with the same X, Y, and Z values.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public Vector3 ToUnityVector(this System.Numerics.Vector3 vector) => new(vector.X, vector.Y, vector.Z);

		/// <summary>
		/// Converts a <see cref="Vector3"/> to a <see cref="System.Numerics.Vector3"/>.
		/// </summary>
		/// <param name="vector">The UnityEngine.Vector3 to convert.</param>
		/// <returns>A System.Numerics.Vector3 with the same x, y, and z values.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static public System.Numerics.Vector3 ToSystemVector(this Vector3 vector) => new(vector.x, vector.y, vector.z);
	}
}