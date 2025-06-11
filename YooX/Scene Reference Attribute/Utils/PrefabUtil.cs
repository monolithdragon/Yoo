using UnityEngine;

namespace YooX.SceneReferenceAttribute.Utils {
    public class PrefabUtil {
        public static bool IsUninstantiatedPrefab(GameObject obj)
            => obj.scene.rootCount == 0;
    }

}
