using UnityEditor;

namespace YooTools.ScriptGenerator.Templates {
    /// <summary>
    /// Template generator for MonoBehaviour script.
    /// </summary>
    [ScriptTemplate("MonoBehaviour", 0)]
    public sealed class MonoBehaviourTemplate : ScriptTemplateGenerator {
        private bool _outputAwakeMethod;
        private bool _outputStartMethod;
        private bool _outputUpdateMethod;
        private bool _outputOnEnableMethod;
        private bool _outputOnDisableMethod;
        private bool _outputOnDestroyMethod;

        /// <inheritdoc/>
        public override bool WillGenerateEditorScript => false;

        /// <summary>
        /// Initialize new <see cref="MonoBehaviourTemplate"/> instance.
        /// </summary>
        public MonoBehaviourTemplate() {
            _outputAwakeMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Awake", false);
            _outputStartMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Start", false);
            _outputUpdateMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Update", false);
            _outputOnEnableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnEnable", false);
            _outputOnDisableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDisable", false);
            _outputOnDestroyMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDestroy", false);
        }

        private void UpdateEditorPrefs() {
            EditorPrefs.SetBool("ScriptTemplates.Message.Awake", _outputAwakeMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.Start", _outputStartMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.Update", _outputUpdateMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnEnable", _outputOnEnableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDisable", _outputOnDisableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDestroy", _outputOnDestroyMethod);
        }

        /// <inheritdoc/>
        public override void OnGUI() {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Output Options:", EditorStyles.boldLabel);

            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor);

            EditorGUILayout.Space();

            _outputAwakeMethod = EditorGUILayout.ToggleLeft("Awake Method", _outputAwakeMethod);
            _outputStartMethod = EditorGUILayout.ToggleLeft("Start Method", _outputStartMethod);
            _outputUpdateMethod = EditorGUILayout.ToggleLeft("Update Method", _outputUpdateMethod);
            _outputOnEnableMethod = EditorGUILayout.ToggleLeft("OnEnable Method", _outputOnEnableMethod);
            _outputOnDisableMethod = EditorGUILayout.ToggleLeft("OnDisable Method", _outputOnDisableMethod);
            _outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", _outputOnDestroyMethod);

            if (EditorGUI.EndChangeCheck())
                UpdateEditorPrefs();
        }

        /// <inheritdoc/>
        public override string GenerateScript(string scriptName, string ns) {
            var sb = CreateScriptBuilder();

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(ns))
                sb.BeginNamespace("namespace " + ns + OpeningBraceInsertion);

            sb.BeginNamespace("public class " + scriptName + " : MonoBehaviour" + OpeningBraceInsertion);

            // Automatically initialize on load?
            if (OutputStaticConstructor)
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            if (_outputAwakeMethod)
                sb.AppendLine("private void Awake()" + OpeningBraceInsertion + "\n}\n");
            if (_outputStartMethod)
                sb.AppendLine("private void Start()" + OpeningBraceInsertion + "\n}\n");
            if (_outputUpdateMethod)
                sb.AppendLine("private void Update()" + OpeningBraceInsertion + "\n}\n");
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
