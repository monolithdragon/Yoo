using UnityEngine;

namespace YooTools.VersionControl {
	[CreateAssetMenu(fileName = "Current Version", menuName = "Yoo Tools/Current Version")]
	public sealed class CurrentVersion : ScriptableObject {
		public Version version;

		private static CurrentVersion _instance;

		static public CurrentVersion Instance {
			get {
				if (_instance == null) {
					_instance = Resources.Load<CurrentVersion>(nameof(CurrentVersion));
				}

				if (_instance == null) {
					Debug.LogWarning($"No instance of {nameof(CurrentVersion)} found, using default values");
					_instance = CreateInstance<CurrentVersion>();
				}

				return _instance;
			}
		}

		private void OnEnable() {
			if (_instance == null) {
				_instance = this;
			}
		}

		private void OnDisable() {
			if (_instance == this) {
				_instance = null;
			}
		}

		private void OnDestroy() {
			if (_instance == this) {
				_instance = null;
			}
		}
	}
}