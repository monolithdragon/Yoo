using System.Runtime.CompilerServices;
using UnityEngine;

namespace YooX {
    public static class QuaternionConversionExtensions {
        /// <summary>
        /// Converts a <see cref="System.Numerics.Quaternion"/> to a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion">The System.Numerics.Quaternion to convert.</param>
        /// <returns>A UnityEngine.Quaternion with the same component values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ToUnityQuaternion(this System.Numerics.Quaternion quaternion) {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        /// <summary>
        /// Converts a <see cref="Quaternion"/> to a <see cref="System.Numerics.Quaternion"/>.
        /// </summary>
        /// <param name="quaternion">The UnityEngine.Quaternion to convert.</param>
        /// <returns>A System.Numerics.Quaternion with the same component values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Quaternion ToSystemQuaternion(this Quaternion quaternion) {
            return new System.Numerics.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }

}
