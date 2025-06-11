using System.IO;
using UnityEditor;
using UnityEngine;

namespace YooTools.ImportAsset {
    public class ImportAssetEditor : Editor {
        [MenuItem("Yoo Tools/Import Essentials Asset")]
        public static void Execute() {
            ImportEssentials(new Asset("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation"));
        }

        private static void ImportEssentials(Asset asset) {
            Debug.Log("Importing: <b>" + asset.Name + "</b>");
            AssetDatabase.ImportPackage(Path.Combine(asset.StorePath, asset.SubFolder, asset.Name), false);
        }

    }
}
