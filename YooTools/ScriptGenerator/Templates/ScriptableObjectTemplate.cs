using UnityEditor;

namespace YooTools.ScriptGenerator.Templates {
    /// <summary>
    /// Template generator for scriptable object script.
    /// </summary>
    [ScriptTemplate("ScriptableObject", 300)]
    public sealed class ScriptableObjectTemplate : ScriptTemplateGenerator {
        private bool _outputAwakeMethod;
        private bool _outputOnEnableMethod;
        private bool _outputOnDisableMethod;
        private bool _outputOnDestroyMethod;

        private string _fileName;
        private int _order;

        /// <summary>
        /// Initialize new <see cref="ScriptableObjectTemplate"/> instance.
        /// </summary>
        public ScriptableObjectTemplate() {
            _outputAwakeMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Awake", false);
            _outputOnEnableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnEnable", false);
            _outputOnDisableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDisable", false);
            _outputOnDestroyMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDestroy", false);

            _fileName = EditorPrefs.GetString("ScriptTemplates.Shared.FileName", "");
            _order = EditorPrefs.GetInt("ScriptTemplates.Shared.Order", 0);
        }

        private void UpdateEditorPrefs() {
            EditorPrefs.SetBool("ScriptTemplates.Message.Awake", _outputAwakeMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnEnable", _outputOnEnableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDisable", _outputOnDisableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDestroy", _outputOnDestroyMethod);

            EditorPrefs.SetString("ScriptTemplates.Shared.FileName", _fileName);
            EditorPrefs.SetInt("ScriptTemplates.Shared.Order", _order);
        }

        /// <inheritdoc/>
        public override bool WillGenerateEditorScript => IsEditorScript;

        /// <inheritdoc/>
        public override void OnGUI() {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Output Options:", EditorStyles.boldLabel);

            EditorGUILayout.PrefixLabel("Create Asset Menu (optional)");
            _fileName = EditorGUILayout.TextField("File Name", _fileName);
            _order = EditorGUILayout.IntField("Position of the menu item", _order);

            EditorGUILayout.Space();

            IsEditorScript = EditorGUILayout.ToggleLeft("Editor Script", IsEditorScript);

            EditorGUI.BeginDisabledGroup(!IsEditorScript);
            OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
            EditorGUI.EndDisabledGroup();

            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);

            EditorGUILayout.Space();

            _outputAwakeMethod = EditorGUILayout.ToggleLeft("Awake Method", _outputAwakeMethod);
            _outputOnEnableMethod = EditorGUILayout.ToggleLeft("OnEnable Method", _outputOnEnableMethod);
            _outputOnDisableMethod = EditorGUILayout.ToggleLeft("OnDisable Method", _outputOnDisableMethod);
            _outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", _outputOnDestroyMethod);

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

            if (!string.IsNullOrEmpty(_fileName)) {
                string menuName = scriptName;
                if (!menuName.Contains("/"))
                    menuName = "ScriptableObject/" + menuName;

                sb.AppendLine("[CreateAssetMenu(fileName = " + "\"" + _fileName + "\"" + ", " + "menuName = " + "\"" + menuName + "\"" + ", " + "order = " + _order + ")]");
            }

            sb.BeginNamespace("public partial class " + scriptName + " : ScriptableObject" + OpeningBraceInsertion);

            // Automatically initialize on load?
            if (IsEditorScript && OutputInitializeOnLoad || OutputStaticConstructor)
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            if (_outputOnEnableMethod)
                sb.AppendLine("private void OnEnable()" + OpeningBraceInsertion + "\n}\n");
            if (_outputOnDisableMethod)
                sb.AppendLine("private void OnDisable()" + OpeningBraceInsertion + "\n}\n");
            if (_outputOnDestroyMethod)
                sb.AppendLine("private void OnDestroy()" + OpeningBraceInsertion + "\n}\n");

            sb.EndNamespace("}");

            if (!string.IsNullOrEmpty(ns))
                sb.EndNamespace("}");

            return sb.ToString();
        }
    }

}
