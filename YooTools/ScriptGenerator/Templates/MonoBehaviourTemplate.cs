using UnityEditor;

namespace YooTools.ScriptGenerator.Templates {
    /// <summary>
    /// Template generator for MonoBehaviour script.
    /// </summary>
    [ScriptTemplate("MonoBehaviour", 0)]
    public sealed class MonoBehaviourTemplate : ScriptTemplateGenerator {
        private bool outputAwakeMethod;
        private bool outputStartMethod;
        private bool outputUpdateMethod;
        private bool outputOnEnableMethod;
        private bool outputOnDisableMethod;
        private bool outputOnDestroyMethod;

        /// <inheritdoc/>
        public override bool WillGenerateEditorScript => false;

        /// <summary>
        /// Initialize new <see cref="MonoBehaviourTemplate"/> instance.
        /// </summary>
        public MonoBehaviourTemplate() {
            outputAwakeMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Awake", false);
            outputStartMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Start", false);
            outputUpdateMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Update", false);
            outputOnEnableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnEnable", false);
            outputOnDisableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDisable", false);
            outputOnDestroyMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDestroy", false);
        }

        private void UpdateEditorPrefs() {
            EditorPrefs.SetBool("ScriptTemplates.Message.Awake", outputAwakeMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.Start", outputStartMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.Update", outputUpdateMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnEnable", outputOnEnableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDisable", outputOnDisableMethod);
            EditorPrefs.SetBool("ScriptTemplates.Message.OnDestroy", outputOnDestroyMethod);
        }

        /// <inheritdoc/>
        public override void OnGUI() {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Output Options:", EditorStyles.boldLabel);

            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor);

            EditorGUILayout.Space();

            outputAwakeMethod = EditorGUILayout.ToggleLeft("Awake Method", outputAwakeMethod);
            outputStartMethod = EditorGUILayout.ToggleLeft("Start Method", outputStartMethod);
            outputUpdateMethod = EditorGUILayout.ToggleLeft("Update Method", outputUpdateMethod);
            outputOnEnableMethod = EditorGUILayout.ToggleLeft("OnEnable Method", outputOnEnableMethod);
            outputOnDisableMethod = EditorGUILayout.ToggleLeft("OnDisable Method", outputOnDisableMethod);
            outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", outputOnDestroyMethod);

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

            if (outputAwakeMethod)
                sb.AppendLine("private void Awake()" + OpeningBraceInsertion + "\n}\n");
            if (outputStartMethod)
                sb.AppendLine("private void Start()" + OpeningBraceInsertion + "\n}\n");
            if (outputUpdateMethod)
                sb.AppendLine("private void Update()" + OpeningBraceInsertion + "\n}\n");
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
