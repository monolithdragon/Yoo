﻿using UnityEngine;

namespace YooX {
	public class Singleton<T> : MonoBehaviour where T : Component {
		private static T _instance;

		public static bool HasInstance => _instance is not null;
		public static T TryGetInstance() => HasInstance ? _instance : null;

		public static T Instance {
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
		protected virtual void Awake() {
			InitializeSingleton();
		}

		private void InitializeSingleton() {
			if (!Application.isPlaying) return;

			_instance ??= this as T;
		}
	}

}