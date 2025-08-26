using UnityEngine;

namespace YooX {
	public class PersistentSingleton<T> : MonoBehaviour where T : Component {
		[SerializeField] private bool autoUnparentOnAwake = true;

		private static T instance;

		static public bool HasInstance => instance is not null;
		static public T TryGetInstance() =>
			HasInstance
				? instance
				: null;

		static public T Instance {
			get {
				if (!instance) {
					instance = FindAnyObjectByType<T>();

					if (!instance) {
						var go = new GameObject($"{typeof(T).Name} Auto-Generated");
						instance = go.AddComponent<T>();
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Make sure to call base.Awake() in override if you need awake.
		/// </summary>
		virtual protected void Awake() => InitializeSingleton();

		private void InitializeSingleton() {
			if (!Application.isPlaying) {
				return;
			}

			if (autoUnparentOnAwake) {
				transform.SetParent(null);
			}

			if (!instance) {
				instance = this as T;
				DontDestroyOnLoad(gameObject);
			} else {
				if (instance != this) {
					Destroy(gameObject);
				}
			}
		}
	}
}