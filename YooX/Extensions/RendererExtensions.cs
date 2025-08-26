using UnityEngine;
using UnityEngine.Rendering;

namespace YooX {
	static public class RendererExtensions {
		/// <summary>
		/// Enables ZWrite for materials in this Renderer that have a '_Color' property. This will allow the materials 
		/// to write to the Z buffer, which could be used to affect how subsequent rendering is handled, 
		/// for instance, ensuring correct layering of transparent objects.
		/// </summary>    
		static public void EnableZWrite(this Renderer renderer) {
			foreach (var material in renderer.materials) {
				if (material.HasProperty("_Color")) {
					material.SetInt("_ZWrite", 1);
					material.renderQueue = (int)RenderQueue.Transparent;
				}
			}
		}

		/// <summary>
		/// Disables ZWrite for materials in this Renderer that have a '_Color' property. This would stop 
		/// the materials from writing to the Z buffer, which may be desirable in some cases to prevent subsequent 
		/// rendering from being occluded, like in rendering of semi-transparent or layered objects.
		/// </summary>
		static public void DisableZWrite(this Renderer renderer) {
			foreach (var material in renderer.materials) {
				if (material.HasProperty("_Color")) {
					material.SetInt("_ZWrite", 0);
					material.renderQueue = (int)RenderQueue.Transparent + 100;
				}
			}
		}
	}
}