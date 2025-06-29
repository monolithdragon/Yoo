using UnityEditor;
using UnityEngine;

namespace YooTools.ProjectFolders {
    public class FolderEditor : Editor {
        [MenuItem("Yoo Tools/Create Folder")]
        public static void Execute() {
            var assets = GenerateFolderStructure();
            CreateFolders(assets);

            AssetDatabase.Refresh();

            if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}")) {
                if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}/Settings")) {
                    if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}/Settings/Renderer")) {
                        AssetDatabase.MoveAsset("Assets/DefaultVolumeProfile.asset", $"Assets/{Application.productName}/Settings/Renderer/DefaultVolumeProfile.asset");
                        AssetDatabase.MoveAsset("Assets/UniversalRenderPipelineGlobalSettings.asset", $"Assets/{Application.productName}/Settings/Renderer/UniversalRenderPipelineGlobalSettings.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/Renderer2D.asset", $"Assets/{Application.productName}/Settings/Renderer/Renderer2D.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/UniversalRP.asset", $"Assets/{Application.productName}/Settings/Renderer/UniversalRP.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/Lit2DSceneTemplate.scenetemplate", $"Assets/{Application.productName}/Settings/Renderer/Lit2DSceneTemplate.scenetemplate");
                        AssetDatabase.MoveAsset($"Assets/Settings/Scenes/URP2DSceneTemplate.unity", $"Assets/{Application.productName}/Settings/Renderer/Scenes/URP2DSceneTemplate.unity");

                        AssetDatabase.Refresh();

                        AssetDatabase.MoveAsset("Assets/Settings/Mobile_Renderer.asset", $"Assets/{Application.productName}/Settings/Renderer/Mobile_Renderer.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/Mobile_RPAsset.asset", $"Assets/{Application.productName}/Settings/Renderer/Mobile_RPAsset.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/PC_Renderer.asset", $"Assets/{Application.productName}/Settings/Renderer/PC_Renderer.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/PC_RPAsset.asset", $"Assets/{Application.productName}/Settings/Renderer/PC_RPAsset.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/SampleSceneProfile.asset", $"Assets/{Application.productName}/Settings/Renderer/SampleSceneProfile.asset");

                        AssetDatabase.Refresh();

                        AssetDatabase.MoveAsset("Assets/Settings/HDRP Balanced.asset", $"Assets/{Application.productName}/Settings/Renderer/HDRP Balanced.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/HDRP High Fidelity.asset", $"Assets/{Application.productName}/Settings/Renderer/HDRP High Fidelity.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/HDRP Performant.asset", $"Assets/{Application.productName}/Settings/Renderer/HDRP Performant.asset");
                        AssetDatabase.MoveAsset("Assets/Settings/SkyandFogSettingsProfile.asset", $"Assets/{Application.productName}/Settings/Renderer/SkyandFogSettingsProfile.asset");

                        AssetDatabase.Refresh();

                        AssetDatabase.DeleteAsset("Assets/Settings");
                    }

                    if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}/Settings/Resources")) {
                        AssetDatabase.MoveAsset("Assets/InputSystem_Actions.inputactions", $"Assets/{Application.productName}/Settings/Resources/InputSystem_Actions.inputactions");
                        AssetDatabase.Refresh();
                    }
                }

                if (AssetDatabase.IsValidFolder($"Assets/{Application.productName}/Scenes")) {
                    AssetDatabase.MoveAsset("Assets/Scenes/SampleScene.unity", $"Assets/{Application.productName}/Scenes/SampleScene.unity");
                    AssetDatabase.MoveAsset("Assets/OutDoorsScene.unity", $"Assets/{Application.productName}/Scenes/OutDoorsScene.unity");
                    AssetDatabase.DeleteAsset("Assets/Scenes");
                    AssetDatabase.Refresh();
                }

                AssetDatabase.MoveAsset("Assets/YooTools.dll", $"Assets/{Application.productName}/Plugins/Editor/YooTools.dll");

                AssetDatabase.DeleteAsset("Assets/Readme.asset");

                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Yoo Tools/Create Folder", true, 0)]
        public static bool ValidateExecute() {
            return !AssetDatabase.IsValidFolder($"Assets/{Application.productName}");
        }

        public static void Move(string newParentFolder, string folderName) {
            var sourcePath = $"Assets/{folderName}";

            if (AssetDatabase.IsValidFolder(sourcePath)) {
                var destinationFolder = $"Assets/{newParentFolder}/{folderName}";

                var error = AssetDatabase.MoveAsset(sourcePath, destinationFolder);

                if (!string.IsNullOrEmpty(error)) {
                    Debug.LogError($"Failed to move {folderName}: {error}");
                }
            }
        }

        public static void Delete(string folderName) {
            var pathToDelete = $"Assets/{folderName}";

            if (AssetDatabase.IsValidFolder(pathToDelete)) {
                AssetDatabase.DeleteAsset(pathToDelete);
            }
        }

        private static void CreateFolders(Folder rootFolder) {
            if (!AssetDatabase.IsValidFolder(rootFolder.CurrentFolder)) {
                Debug.Log($"Creating: <b>{rootFolder.CurrentFolder}</b>");

                AssetDatabase.CreateFolder(rootFolder.ParentFolder, rootFolder.Name);

                //File.Create(Directory.GetCurrentDirectory()
                //                + Path.DirectorySeparatorChar
                //                + rootFolder.CurrentFolder
                //                + Path.DirectorySeparatorChar
                //                + ".keep");

                //Debug.Log($"Creating '.keep' file in: <b>{rootFolder.CurrentFolder}</b>");
                //} else {
                //    if (Directory.GetFiles(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + rootFolder.CurrentFolder).Length < 1) {

                //        File.Create(Directory.GetCurrentDirectory()
                //                    + Path.DirectorySeparatorChar
                //                    + rootFolder.CurrentFolder
                //                    + Path.DirectorySeparatorChar
                //                    + ".keep");


                //    } else {
                //        Debug.Log($"Directory <b>{rootFolder.CurrentFolder}</b> already exists");
                //    }
            }

            foreach (var folder in rootFolder.Folders) {
                CreateFolders(folder);
            }
        }

        private static Folder GenerateFolderStructure() {
            var rootFolder = new Folder("Assets", "");

            var subFolder = rootFolder.Add(Application.productName);
            subFolder.Add("Effects");
            subFolder.Add("Scenes");

            var pluginFolder = subFolder.Add("Plugins");
            pluginFolder.Add("Editor");

            subFolder.Add("Prefabs");
            subFolder.Add("Scriptables");

            var scriptFolder = subFolder.Add("Scripts");
            scriptFolder.Add("Editor");
            scriptFolder.Add("Runtime");

            var testFolder = subFolder.Add("Tests");
            testFolder.Add("Editor");
            testFolder.Add("Runtime");

            var artFolder = subFolder.Add("Art");
            artFolder.Add("Animations");
            artFolder.Add("Fonts");
            artFolder.Add("Materials");
            artFolder.Add("Meshes");
            artFolder.Add("Textures");
            artFolder.Add("Shaders");
            artFolder.Add("Sprites");
            artFolder.Add("Audio");

            var uiFolder = subFolder.Add("UIToolkit");
            uiFolder.Add("Layouts");
            uiFolder.Add("Settings");
            uiFolder.Add("Styles");
            uiFolder.Add("Theme");

            var settingsFolder = subFolder.Add("Settings");
            var rendererFolder = settingsFolder.Add("Renderer");
            rendererFolder.Add("Scenes");

            settingsFolder.Add("Presets");
            settingsFolder.Add("Resources");

            return rootFolder;
        }

    }
}
