using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YooTools.ScriptGenerator {
    public class CreateScriptWindow : EditorWindow {
        [MenuItem("Yoo Tools/Script Generator/Create Script from Template")]
        [MenuItem("Assets/Create/Yoo Tools/Create Script from Template", false, 1)]
        private static void ShowWindow() {
            GetWindow<CreateScriptWindow>("Create Script");
        }

        [SerializeField] private string scriptName = "";
        [SerializeField] private string scriptNamespace = "";
        [SerializeField] private bool namespaceForSubFolders;

        [NonSerialized] private string[] templateDescriptions;

        [SerializeField] private int templateIndex;
        [NonSerialized] private ScriptTemplateGenerator activeGenerator;

        private Vector2 scrollPosition;

        private void OnEnable() {
            minSize = new Vector2(230, 100);

            namespaceForSubFolders = EditorPrefs.GetBool("ScriptTemplates.NamespaceToDirectory", true);

            AutoFixActiveGenerator();
        }

        private void OnGUI() {
            EditorGUILayout.Space();
            DrawTemplateSelector();
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                DrawStandardInputs();

                EditorGUILayout.Space();
                activeGenerator?.OnGUI();

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Box(GUIContent.none, GUILayout.Height(2), GUILayout.ExpandWidth(true));
            activeGenerator?.OnStandardGUI();

            DrawButtonStrip();
            EditorGUILayout.Space();
        }

        private void DrawTemplateSelector() {
            templateDescriptions = ScriptGeneratorDescriptor.Descriptors
                    .Select(descriptor => descriptor.TemplateAttribute.Description)
                    .ToArray();

            // Allow user to select template from popup.
            EditorGUILayout.PrefixLabel("Template:");
            EditorGUI.BeginChangeCheck();

            templateIndex = EditorGUILayout.Popup(templateIndex, templateDescriptions);

            if (EditorGUI.EndChangeCheck())
                activeGenerator = null;

            AutoFixActiveGenerator();
        }

        private void DrawStandardInputs() {
            EditorGUILayout.PrefixLabel("Script Name:");
            scriptName = EditorGUILayout.TextField(scriptName);

            EditorGUILayout.PrefixLabel("Namespace:");
            scriptNamespace = EditorGUILayout.TextField(scriptNamespace);

            EditorGUI.BeginChangeCheck();

            namespaceForSubFolders = EditorGUILayout.ToggleLeft("Use namespace for sub-folders", namespaceForSubFolders);

            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool("ScriptTemplates.NamespaceToDirectory", namespaceForSubFolders);
        }

        private void DrawButtonStrip() {
            GUILayout.Box(GUIContent.none, GUILayout.Height(2), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Save at Default Path"))
                DoSaveAtDefaultPath();
            if (GUILayout.Button("Save As"))
                DoSaveAs();

            EditorGUILayout.Space();

            if (GUILayout.Button("Copy to Clipboard"))
                DoCopyToClipboard();
        }

        private void AutoFixActiveGenerator() {
            if (activeGenerator == null) {
                templateIndex = Mathf.Clamp(templateIndex, 0, ScriptGeneratorDescriptor.Descriptors.Count);
                activeGenerator = ScriptGeneratorDescriptor.Descriptors[templateIndex].CreateInstance();
            }
        }

        private void ResetInputs() {
            AutoFixActiveGenerator();

            scriptName = "";
            scriptNamespace = "";
        }

        private string GetDefaultOutputPath() {
            string assetFolder = Path.Combine("Assets", activeGenerator.WillGenerateEditorScript ? Application.productName + Path.DirectorySeparatorChar + "Scripts/Editor" : Application.productName + Path.DirectorySeparatorChar + "Scripts/Runtime");

            // Use namespace for sub-folders?
            if (namespaceForSubFolders && !string.IsNullOrEmpty(scriptNamespace)) {
                assetFolder = Path.Combine(assetFolder, scriptNamespace.Replace(".", "/"));
            } else {
                string selectedFolderPath = SelectedFolder();
                if (selectedFolderPath != null && selectedFolderPath != assetFolder) {
                    string[] splitResult = selectedFolderPath.Split('/');
                    selectedFolderPath = splitResult.LastOrDefault();
                    assetFolder = Path.Combine(assetFolder, selectedFolderPath);
                }
            }

            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), assetFolder);

            // Ensure that this path actually exists.
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            return outputPath;
        }

        private void DoSaveAtDefaultPath() {
            GenerateScriptFromTemplate(Path.Combine(GetDefaultOutputPath(), scriptName + ".cs"));
        }

        private void DoSaveAs() {
            // Prompt user to specify path to save script.
            string path = EditorUtility.SaveFilePanel("Save New Script", GetDefaultOutputPath(), scriptName + ".cs", ".cs");
            if (!string.IsNullOrEmpty(path))
                GenerateScriptFromTemplate(path);
        }

        private static bool IsClassNameUnique(string fullName) {
            if (string.IsNullOrEmpty(fullName))
                throw new InvalidOperationException("An empty or null string was specified.");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                    if (type.FullName == fullName)
                        return false;
            return true;
        }

        private void ValidateScriptInputs() {
            // Ensure that valid script name was specified.
            if (!Regex.IsMatch(scriptName, @"^[A-za-z_][A-za-z_0-9]*$")) {
                EditorUtility.DisplayDialog("Invalid Script Name", string.Format("'{0}' is not a valid type name.", scriptName), "OK");
                return;
            }
            // If a namespace was specified, ensure that it is valid!
            if (!string.IsNullOrEmpty(scriptNamespace) && !Regex.IsMatch(scriptNamespace, @"^[A-za-z_][A-za-z_0-9]*(\.[A-za-z_][A-za-z_0-9]*)*$")) {
                EditorUtility.DisplayDialog("Invalid Namespace", string.Format("'{0}' is not a valid namespace.", scriptNamespace), "OK");
                return;
            }
        }

        private void GenerateScriptFromTemplate(string path) {
            // Ensure that input focus is removed from text field.
            EditorGUIUtility.keyboardControl = 0;
            EditorGUIUtility.editingTextField = false;

            // Ensure that path ends with '.cs'.
            if (!path.EndsWith(".cs")) {
                EditorUtility.DisplayDialog("Invalid File Extension", "Could not save script because the wrong file extension was specified.\n\nPlease ensure to save as '.cs' file.", "OK");
                return;
            }
            // Ensure that base directory actually exists.
            if (!Directory.Exists(Path.GetDirectoryName(path))) {
                EditorUtility.DisplayDialog("Invalid Path", "Could not save script because the specified directory does not exist.", "OK");
                return;
            }

            ValidateScriptInputs();

            string fullName = !string.IsNullOrEmpty(scriptNamespace)
                ? scriptNamespace + "." + scriptName
                : scriptName;

            // Warn user if their type name is not unique.
            if (!IsClassNameUnique(fullName))
                if (!EditorUtility.DisplayDialog("Warning: Type Already Exists!", string.Format("A type already exists with the name '{0}'.\n\nIf you proceed then you will get compilation errors in the console window.", fullName), "Proceed", "Cancel"))
                    return;

            // Generate source code.
            string sourceCode = activeGenerator.GenerateScript(scriptName, scriptNamespace);
            // Write to file!
            File.WriteAllText(path, sourceCode, Encoding.UTF8);

            // Unity should now recompile its scripts!
            AssetDatabase.Refresh();

            Repaint();
        }

        private void DoCopyToClipboard() {
            // Ensure that input focus is removed from text field.
            EditorGUIUtility.keyboardControl = 0;
            EditorGUIUtility.editingTextField = false;

            ValidateScriptInputs();

            EditorGUIUtility.systemCopyBuffer = activeGenerator.GenerateScript(scriptName, scriptNamespace);
        }

        private string SelectedFolder() {
            string result = string.Empty;
            Object[] objs = Selection.GetFiltered<Object>(SelectionMode.Assets);
            foreach (Object obj in objs) {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path)) {
                    result = path;
                    break;
                }
            }

            return result;
        }

    }
}
