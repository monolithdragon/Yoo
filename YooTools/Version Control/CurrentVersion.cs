using UnityEngine;

namespace YooTools.VersionControl {
    [CreateAssetMenu(fileName = "Current Version", menuName = "Yoo Tools/Current Version")]
    public sealed class CurrentVersion : ScriptableObject {
        public Version version;

        private static CurrentVersion instance;

        public static CurrentVersion Instance {
            get {
                if (instance == null)
                    instance = Resources.Load<CurrentVersion>(typeof(CurrentVersion).Name);
                if (instance == null) {
                    Debug.LogWarning($"No instance of {typeof(CurrentVersion).Name} found, using default values");
                    instance = CreateInstance<CurrentVersion>();
                }

                return instance;
            }
        }

        private void OnEnable() {
            if (instance == null) {
                instance = this;
            }
        }

        private void OnDisable() {
            if (instance == this) {
                instance = null;
            }
        }

        private void OnDestroy() {
            if (instance == this) {
                instance = null;
            }
        }

    }
}
