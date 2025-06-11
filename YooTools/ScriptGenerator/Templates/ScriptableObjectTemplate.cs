using UnityEditor;

namespace YooTools.ScriptGenerator.Templates {
    /// <summary>
    /// Template generator for scriptable object script.
    /// </summary>
    [ScriptTemplate("ScriptableObject", 300)]
    public sealed class ScriptableObjectTemplate : ScriptTemplateGenerator {
        private bool outputAwakeMethod;
        private bool outputOnEnableMethod;
        private bool outputOnDisableMethod;
        private bool outputOnDestroyMethod;

        private string fileName;
        private int order;

        /// <summary>
        /// Initialize new <see cref="ScriptableObjectTemplate"/> instance.
        /// </summary>
        public ScriptableObjectTemplate() {
            outputAwakeMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Awake", false);
            outputOnEnableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnEnable", false);
            outputOnDisableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDisable", false);
            outputOnDestroyMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDestroy", false);

            fileName = EditorPrefs.GetString("ScriptTemplates.Shared.FileName", "");
            order = EditorPrefs.GetInt("ScriptTemplates.Shared.Order", 0);
        }

        private void UpdateEditorPrefs() {
            EditorPrefs.SetBool("ScriptTemplates.Message.Awake", outputAwakeMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnEnable", outputOnEnableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDisable", outputOnDisableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDestroy", outputOnDestroyMethod);

            EditorPrefs.SetString("ScriptTemplates.Shared.FileName", fileName);
            EditorPrefs.SetInt("ScriptTemplates.Shared.Order", order);
        }

        /// <inheritdoc/>
        public override bool WillGenerateEditorScript => IsEditorScript;

        /// <inheritdoc/>
        public override void OnGUI() {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Output Options:", EditorStyles.boldLabel);

            EditorGUILayout.PrefixLabel("Create Asset Menu (optional)");
            fileName = EditorGUILayout.TextField("File Name", fileName);
            order = EditorGUILayout.IntField("Position of the menu item", order);

            EditorGUILayout.Space();

            IsEditorScript = EditorGUILayout.ToggleLeft("Editor Script", IsEditorScript);

            EditorGUI.BeginDisabledGroup(!IsEditorScript);
            OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
            EditorGUI.EndDisabledGroup();

            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);

            EditorGUILayout.Space();

            outputAwakeMethod = EditorGUILayout.ToggleLeft("Awake Method", outputAwakeMethod);
            outputOnEnableMethod = EditorGUILayout.ToggleLeft("OnEnable Method", outputOnEnableMethod);
            outputOnDisableMethod = EditorGUILayout.ToggleLeft("OnDisable Method", outputOnDisableMethod);
            outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", outputOnDestroyMethod);

            if (EditorGUI.EndChangeCheck()) {
                UpdateEditorPrefs();
            }
        }

        /// <inheritdoc/>
        public override string GenerateScript(string scriptName, string ns) {
            var sb = CreateScriptBuilder();

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEditor;");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(ns))
                sb.BeginNamespace("namespace " + ns + OpeningBraceInsertion);

            // Automatically initialize on load?
            if (IsEditorScript && OutputInitializeOnLoad)
                sb.AppendLine("[InitializeOnLoad]");

            if (!string.IsNullOrEmpty(fileName)) {
                string menuName = scriptName;
                if (!menuName.Contains("/"))
                    menuName = "ScriptableObject/" + menuName;

                sb.AppendLine("[CreateAssetMenu(fileName = " + "\"" + fileName + "\"" + ", " + "menuName = " + "\"" + menuName + "\"" + ", " + "order = " + order + ")]");
            }

            sb.BeginNamespace("public partial class " + scriptName + " : ScriptableObject" + OpeningBraceInsertion);

            // Automatically initialize on load?
            if (IsEditorScript && OutputInitializeOnLoad || OutputStaticConstructor)
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            if (outputOnEnableMethod)
                sb.AppendLine("private void OnEnable()" + OpeningBraceInsertion + "\n}\n");
            if (outputOnDisableMethod)
                sb.AppendLine("private void OnDisable()" + OpeningBraceInsertion + "\n}\n");
            if (outputOnDestroyMethod)
                sb.AppendLine("private void OnDestroy()" + OpeningBraceInsertion + "\n}\n");

            sb.EndNamespace("}");

            if (!string.IsNullOrEmpty(ns))
                sb.EndNamespace("}");

            return sb.ToString();
        }
    }

}
