using System.Collections.Generic;
using UnityEditor;

namespace YooTools.ScriptGenerator.Templates {
    /// <summary>
    /// Base class for basic C# source files.
    /// </summary>
    public abstract class BaseTemplate : ScriptTemplateGenerator {
        private readonly string typeKeyword;

        /// <summary>
        /// Initialize a new <see cref="BaseTemplate"/> instance.
        /// </summary>
        /// <param name="typeKeyword">>Keyword of type; for instance, 'class'</param>
        public BaseTemplate(string typeKeyword) {
            this.typeKeyword = typeKeyword;
        }

        /// <inheritdoc/>
		public override bool WillGenerateEditorScript => IsEditorScript;

        /// <inheritdoc/>
        public override void OnGUI() {
            EditorGUILayout.LabelField("Output Options:", EditorStyles.boldLabel);

            IsEditorScript = EditorGUILayout.ToggleLeft("Editor Script", IsEditorScript);
            if (!IsEditorScript)
                OutputInitializeOnLoad = false;

            if (typeKeyword == "class") {
                EditorGUI.BeginDisabledGroup(!IsEditorScript);
                OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
                EditorGUI.EndDisabledGroup();

                OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PrefixLabel("Visibility");
            ScriptTypeVisibility = (TypeVisibility)EditorGUILayout.EnumPopup(ScriptTypeVisibility);

            if (typeKeyword == "class") {
                AbstractClass = EditorGUILayout.ToggleLeft("Abstract", AbstractClass);
                StaticClass = EditorGUILayout.ToggleLeft("Static", StaticClass);
                PartialClass = EditorGUILayout.ToggleLeft("Partial", PartialClass);
            }
        }

        /// <inheritdoc/>
		public override string GenerateScript(string scriptName, string ns) {
            ScriptBuilder sb = CreateScriptBuilder();

            sb.AppendLine("using UnityEngine;");
            if (IsEditorScript)
                sb.AppendLine("using UnityEditor;");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(ns))
                sb.BeginNamespace("namespace " + ns + OpeningBraceInsertion);

            // Automatically initialize on load?
            if (typeKeyword == "class" && (IsEditorScript && OutputInitializeOnLoad))
                sb.AppendLine("[InitializeOnLoad]");

            // Build type declaration string.
            var declaration = new List<string>();

            if (ScriptTypeVisibility == TypeVisibility.Public)
                declaration.Add("public");
            else if (ScriptTypeVisibility == TypeVisibility.Internal)
                declaration.Add("internal");

            if (typeKeyword == "class") {
                if (AbstractClass)
                    declaration.Add("abstract");
                else if (StaticClass)
                    declaration.Add("static");
                else if (PartialClass)
                    declaration.Add("partial");
            }

            declaration.Add(typeKeyword);
            declaration.Add(scriptName);

            sb.BeginNamespace(string.Join(" ", declaration.ToArray()) + OpeningBraceInsertion);

            // Automatically initialize on load?
            if (typeKeyword == "class" && (IsEditorScript && OutputInitializeOnLoad || OutputStaticConstructor))
                sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

            sb.EndNamespace("}");

            if (!string.IsNullOrEmpty(ns))
                sb.EndNamespace("}");

            return sb.ToString();
        }

    }
}
