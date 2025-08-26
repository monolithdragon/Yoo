using UnityEngine;

namespace YooX {
	public class Singleton<T> : MonoBehaviour where T : Component {
		private static T _instance;

		static public bool HasInstance => _instance is not null;
		static public T TryGetInstance() =>
			HasInstance
				? _instance
				: null;

		static public T Instance {
			get {
				if (!_instance) {
					_instance = FindAnyObjectByType<T>();

					if (!_instance) {
						var go = new GameObject($"{typeof(T).Name} Auto-Generated");
						_instance = go.AddComponent<T>();
					}
				}

				return _instance;
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

			_instance ??= this as T;
		}
	}
}