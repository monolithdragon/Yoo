using UnityEngine;

namespace YooX {
	static public class RigidbodyExtensions {
		/// <summary>
		/// Changes the direction of the Rigidbody's velocity while maintaining its speed.
		/// </summary>
		/// <param name="rigidbody">The Rigidbody to change direction.</param>
		/// <param name="direction">The new direction for the Rigidbody.</param>
		/// <returns>The modified Rigidbody for method chaining.</returns>
		static public Rigidbody ChangeDirection(this Rigidbody rigidbody, Vector3 direction) {
			if (direction.sqrMagnitude == 0f) {
				return rigidbody;
			}

			direction.Normalize();

			rigidbody.linearVelocity = direction * rigidbody.linearVelocity.magnitude;

			return rigidbody;
		}

		/// <summary>
		/// Stops the Rigidbody by setting its linear and angular velocities to zero.
		/// </summary>
		/// <param name="rigidbody">The Rigidbody to stop.</param>
		/// <returns>The modified Rigidbody for method chaining.</returns>
		static public Rigidbody Stop(this Rigidbody rigidbody) {
			rigidbody.linearVelocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;

			return rigidbody;
		}
	}
}