using UnityEngine;

namespace YooTools.VersionControl {
    [CreateAssetMenu(fileName = "Current Version", menuName = "Yoo Tools/Current Version")]
    public class CurrentVersion : ScriptableObject {
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

        protected virtual void OnEnable() {
            if (instance == null) {
                instance = this;
            }
        }

        protected virtual void OnDisable() {
            if (instance == this) {
                instance = null;
            }
        }

        protected virtual void OnDestroy() {
            if (instance == this) {
                instance = null;
            }
        }

    }
}
