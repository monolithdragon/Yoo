using UnityEngine;

namespace YooX {
    public static class CameraExtensions {
        ///<summary>
        /// Calculates the viewport extents at the camera's near clip plane, including an optional margin.
        /// The result is a Vector2 where x is the horizontal extent and y is the vertical extent, both in world units.
        /// </summary>
        /// <param name="camera">The camera for which to calculate the extents.</param>
        /// <param name="viewportMargin">
        /// Optional margin to add to the extents (x: horizontal, y: vertical). 
        /// If not specified, a default margin of (0.2, 0.2) is used.
        /// </param>
        /// <returns>
        /// A Vector2 representing the extents (x: horizontal, y: vertical) at the near clip plane, including the margin.
        /// </returns>
        public static Vector2 GetViewportExtentsWithMargin(this Camera camera, Vector2? viewportMargin = null) {
            Vector2 margin = viewportMargin ?? new Vector2(0.2f, 0.2f);

            Vector2 result;
            float halfFieldOfView = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            result.y = camera.nearClipPlane * Mathf.Tan(halfFieldOfView);
            result.x = result.y * camera.aspect + margin.x;
            result.y += margin.y;
            return result;
        }
    }

}
