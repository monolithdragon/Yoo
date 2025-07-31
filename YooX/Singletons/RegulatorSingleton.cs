using UnityEngine;

namespace YooX {
	/// <summary>
	/// Persistent Regulator singleton, will destroy any other older components of the same type it finds on awake
	/// </summary>
	public class RegulatorSingleton<T> : MonoBehaviour where T : Component {
		private static T instance;

		public static bool HasInstance => instance is not null;

		public float InitializationTime { get; private set; }

		public static T Instance {
			get {
				if (!instance) {
					instance = FindAnyObjectByType<T>();
					if (!instance) {
						var go = new GameObject($"{typeof(T).Name} Auto-Generated") {
							hideFlags = HideFlags.HideAndDontSave
						};
						instance = go.AddComponent<T>();
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Make sure to call base.Awake() in override if you need awake.
		/// </summary>
		protected virtual void Awake() {
			InitializeSingleton();
		}

		private void InitializeSingleton() {
			if (!Application.isPlaying)
				return;

			InitializationTime = Time.time;
			DontDestroyOnLoad(gameObject);

			var oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
			foreach (var old in oldInstances) {
				if (old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime) {
					Destroy(old.gameObject);
				}
			}

			if (!instance) {
				instance = this as T;
			}
		}
	}

}