using System.IO;
using UnityEditor;
using UnityEngine;

namespace YooTools.ScriptGenerator.Templates {
    /// <summary>
    /// Template generator for editor menu script.
    /// </summary>
    [ScriptTemplate("Editor Menu", 200)]
    public sealed class EditorMenuTemplate : ScriptTemplateGenerator {
        private string menuItem;

        /// <summary>
        /// Initialize new <see cref="EditorMenuTemplate"/> instance.
        /// </summary>
        public EditorMenuTemplate() {
            menuItem = EditorPrefs.GetString("ScriptTemplates.Shared.MenuItem", "");
        }

        private void UpdateEditorPrefs() {
            EditorPrefs.SetString("ScriptTemplates.Shared.MenuItem", menuItem);
        }

        /// <inheritdoc/>
        public override bool WillGenerateEditorScript => true;

        /// <inheritdoc/>
        public override void OnGUI() {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Output Options:", EditorStyles.boldLabel);

            EditorGUILayout.PrefixLabel("Menu Item (optional)");
            menuItem = EditorGUILayout.TextField(menuItem);

            EditorGUILayout.Space();

            OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
            OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);

            if (EditorGUI.EndChangeCheck()) {
                menuItem = menuItem.Trim();
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
            if (OutputInitializeOnLoad)
                sb.AppendLine("[InitializeOnLoad]");

            sb.BeginNamespace("public class " + scriptName + OpeningBraceInsertion);

            // Automatically initialize on load?
            if (OutputInitializeOnLoad || OutputStaticConstructor)
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            if (!string.IsNullOrEmpty(menuItem)) {
                string menuName = menuItem;
                if (!menuName.Contains("/"))
                    menuName = Application.productName + Path.DirectorySeparatorChar + menuName;

                sb.AppendLine("[MenuItem(\"" + menuName + "\")]");
                sb.AppendLine("private static void " + menuName.Replace("/", "_").Replace(" ", "_") + "()" + OpeningBraceInsertion);
                sb.AppendLine("}\n");
            }

            sb.EndNamespace("}");

            if (!string.IsNullOrEmpty(ns))
                sb.EndNamespace("}");

            return sb.ToString();
        }

    }
}
