using System.Linq;
using UnityEngine;

namespace YooX {
	static public class GameObjectExtensions {
		/// <summary>
		/// Hides the GameObject in the Unity Hierarchy window without disabling it.
		/// </summary>
		/// <param name="gameObject">The GameObject to hide in the hierarchy.</param>
		static public void HideInHierarchy(this GameObject gameObject) => gameObject.hideFlags = HideFlags.HideInHierarchy;

		/// <summary>
		/// Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds one.
		/// </summary>
		/// <remarks>
		/// This method is useful when you don't know if a GameObject has a specific type of component,
		/// but you want to work with that component regardless. Instead of checking and adding the component manually,
		/// you can use this method to do both operations in one line.
		/// </remarks>
		/// <typeparam name="T">The type of the component to get or add.</typeparam>
		/// <param name="gameObject">The GameObject to get the component from or add the component to.</param>
		/// <returns>The existing component of the given type, or a new one if no such component exists.</returns>    
		static public T GetOrAdd<T>(this GameObject gameObject) where T : Component {
			var component = gameObject.GetComponent<T>();

			if (!component) {
				component = gameObject.AddComponent<T>();
			}

			return component;
		}

		/// <summary>
		/// Returns the object itself if it exists, null otherwise.
		/// </summary>
		/// <remarks>
		/// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
		/// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
		/// Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is returned,
		/// aiding in correctly chaining operations and preventing NullReferenceExceptions.
		/// </remarks>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <param name="obj">The object being checked.</param>
		/// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
		static public T OrNull<T>(this T obj) where T : Object =>
			obj
				? obj
				: null;

		/// <summary>
		/// Destroys all children of the game object
		/// </summary>
		/// <param name="gameObject">GameObject whose children are to be destroyed.</param>
		static public void DestroyChildren(this GameObject gameObject) => gameObject.transform.DestroyChildren();

		/// <summary>
		/// Immediately destroys all children of the given GameObject.
		/// </summary>
		/// <param name="gameObject">GameObject whose children are to be destroyed.</param>
		static public void DestroyChildrenImmediate(this GameObject gameObject) => gameObject.transform.DestroyChildrenImmediate();

		/// <summary>
		/// Enables all child GameObjects associated with the given GameObject.
		/// </summary>
		/// <param name="gameObject">GameObject whose child GameObjects are to be enabled.</param>
		static public void EnableChildren(this GameObject gameObject) => gameObject.transform.EnableChildren();

		/// <summary>
		/// Disables all child GameObjects associated with the given GameObject.
		/// </summary>
		/// <param name="gameObject">GameObject whose child GameObjects are to be disabled.</param>
		static public void DisableChildren(this GameObject gameObject) => gameObject.transform.DisableChildren();

		/// <summary>
		/// Resets the GameObject's transform's position, rotation, and scale to their default values.
		/// </summary>
		/// <param name="gameObject">GameObject whose transformation is to be reset.</param>
		static public void ResetTransformation(this GameObject gameObject) => gameObject.transform.Reset();

		/// <summary>
		/// Returns the hierarchical path in the Unity scene hierarchy for this GameObject.
		/// </summary>
		/// <param name="gameObject">The GameObject to get the path for.</param>
		/// <returns>A string representing the full hierarchical path of this GameObject in the Unity scene.
		/// This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
		/// with the name of the specified GameObjects parent.</returns>
		static public string Path(this GameObject gameObject) =>
			"/"
			+ string.Join(
				"/",
				gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray()
			);

		/// <summary>
		/// Returns the full hierarchical path in the Unity scene hierarchy for this GameObject.
		/// </summary>
		/// <param name="gameObject">The GameObject to get the path for.</param>
		/// <returns>A string representing the full hierarchical path of this GameObject in the Unity scene.
		/// This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
		/// with the name of the specified GameObject itself.</returns>
		static public string PathFull(this GameObject gameObject) => gameObject.Path() + "/" + gameObject.name;

		/// <summary>
		/// Recursively sets the provided layer for this GameObject and all of its descendants in the Unity scene hierarchy.
		/// </summary>
		/// <param name="gameObject">The GameObject to set layers for.</param>
		/// <param name="layer">The layer number to set for GameObject and all of its descendants.</param>
		static public void SetLayersRecursively(this GameObject gameObject, int layer) {
			gameObject.layer = layer;
			gameObject.transform.ForEveryChild(child => child.gameObject.SetLayersRecursively(layer));
		}

		/// <summary>
		/// Activates the GameObject associated with the MonoBehaviour and returns the instance.
		/// </summary>
		/// <typeparam name="T">The type of the MonoBehaviour.</typeparam>
		/// <param name="obj">The MonoBehaviour whose GameObject will be activated.</param>
		/// <returns>The instance of the MonoBehaviour.</returns>
		static public T SetActive<T>(this T obj) where T : MonoBehaviour {
			obj.gameObject.SetActive(true);

			return obj;
		}

		/// <summary>
		/// Deactivates the GameObject associated with the MonoBehaviour and returns the instance.
		/// </summary>
		/// <typeparam name="T">The type of the MonoBehaviour.</typeparam>
		/// <param name="obj">The MonoBehaviour whose GameObject will be deactivated.</param>
		/// <returns>The instance of the MonoBehaviour.</returns>
		static public T SetInactive<T>(this T obj) where T : MonoBehaviour {
			obj.gameObject.SetActive(false);

			return obj;
		}
	}
}