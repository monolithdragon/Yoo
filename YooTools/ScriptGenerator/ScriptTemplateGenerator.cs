using UnityEditor;

namespace YooTools.ScriptGenerator {
    /// <summary>
    /// Indicates visibility of a type.
    /// </summary>
    public enum TypeVisibility {
        Public,
        Private,
        Internal
    }


    /// <summary>
    /// Base class for a script template generator.
    /// </summary>
    /// <remarks>
    /// <para>Custom template generators must be marked with the 
    /// <see cref="ScriptTemplateAttribute"/> so that they can be used within the
    /// "Create script" window.</para>
    /// </remarks>
    [InitializeOnLoad]
    public abstract class ScriptTemplateGenerator {
        static ScriptTemplateGenerator() {
            LoadSharedPreferences();
        }

        #region Shared References

        private static bool isBracesOnNewLine;
        private static bool isEditorScript;
        private static bool isInitializeOnLoad;
        private static bool isStaticConstructor;

        private static TypeVisibility typeVisibility;
        private static bool isStaticClass;
        private static bool isPartialClass;
        private static bool isAbstractClass;

        /// <summary>
        /// Gets or sets whether opening brace character '{' should begin on a new line.
        /// </summary>
        protected static bool BracesOnNewLine {
            get { return isBracesOnNewLine; }
            set {
                if (value != isBracesOnNewLine) {
                    EditorPrefs.SetBool("ScriptTemplates.Shared.BracesOnNewLine", isBracesOnNewLine = value);
                }
            }
        }

        /// <summary>
		/// When applicable indicates whether script should be for editor usage.
		/// </summary>
		protected static bool IsEditorScript {
            get { return isEditorScript; }
            set {
                if (value != isEditorScript) {
                    EditorPrefs.SetBool("ScriptTemplates.Shared.EditorScript", isEditorScript = value);
                    if (!value)
                        OutputInitializeOnLoad = false;
                }
            }
        }

        /// <summary>
		/// Indicates that output class should be marked with <c>InitializeOnLoad</c> attribute.
		/// </summary>
		protected static bool OutputInitializeOnLoad {
            get { return isInitializeOnLoad; }
            set {
                if (value != isInitializeOnLoad)
                    EditorPrefs.SetBool("ScriptTemplates.Shared.InitializeOnLoad", isInitializeOnLoad = value);
            }
        }

        /// <summary>
		/// Indicates if static constructor should be added to output class.
		/// </summary>
		protected static bool OutputStaticConstructor {
            get { return isStaticConstructor; }
            set {
                if (value != isStaticConstructor) {
                    EditorPrefs.SetBool("ScriptTemplates.Shared.StaticConstructor", isStaticConstructor = value);
                    if (!value)
                        OutputInitializeOnLoad = false;
                }
            }
        }

        /// <summary>
		/// Visibility for output type.
		/// </summary>
		protected static TypeVisibility ScriptTypeVisibility {
            get { return typeVisibility; }
            set {
                if (value != typeVisibility)
                    EditorPrefs.SetInt("ScriptTemplates.Shared.TypeVisibility", (int)(typeVisibility = value));
            }
        }

        /// <summary>
		/// Indicates whether output class should be marked as abstract.
		/// </summary>
		protected static bool AbstractClass {
            get { return isAbstractClass; }
            set {
                if (value != isAbstractClass) {
                    EditorPrefs.SetBool("ScriptTemplates.Shared.AbstractClass", isAbstractClass = value);
                    if (value) {
                        StaticClass = false;
                        PartialClass = false;
                    }
                }
            }
        }

        /// <summary>
		/// Indicates whether output class should be marked as static.
		/// </summary>
		protected static bool StaticClass {
            get { return isStaticClass; }
            set {
                if (value != isStaticClass) {
                    EditorPrefs.SetBool("ScriptTemplates.Shared.StaticClass", isStaticClass = value);
                    if (value) {
                        AbstractClass = false;
                        PartialClass = false;
                    }
                }
            }
        }

        /// <summary>
		/// Indicates whether output class should be marked as partial.
		/// </summary>
		protected static bool PartialClass {
            get { return isPartialClass; }
            set {
                if (value != isPartialClass) {
                    EditorPrefs.SetBool("ScriptTemplates.Shared.PartialClass", isPartialClass = value);
                    if (value) {
                        AbstractClass = false;
                        StaticClass = false;
                    }
                }
            }
        }

        /// <summary>
		/// Gets characters for opening brace insertion.
		/// </summary>
		protected static string OpeningBraceInsertion {
            get { return BracesOnNewLine ? "\n{" : " {"; }
        }

        private static void LoadSharedPreferences() {
            isBracesOnNewLine = EditorPrefs.GetBool("ScriptTemplates.Shared.BraceOnNewLine", true);
            isEditorScript = EditorPrefs.GetBool("ScriptTemplates.Shared.EditorScript", false);
            isStaticConstructor = EditorPrefs.GetBool("ScriptTemplates.Shared.StaticConstructor", false);
            typeVisibility = (TypeVisibility)EditorPrefs.GetInt("ScriptTemplates.Shared.TypeVisibility", (int)TypeVisibility.Public);
            isAbstractClass = EditorPrefs.GetBool("ScriptTemplates.Shared.AbstractClass", false);
            isStaticClass = EditorPrefs.GetBool("ScriptTemplates.Shared.StaticClass", false);
            isPartialClass = EditorPrefs.GetBool("ScriptTemplates.Shared.PartialClass", false);
        }

        #endregion

        /// <summary>
		/// Gets a value indicating whether an editor script will be generated.
		/// </summary>
		public abstract bool WillGenerateEditorScript { get; }

        /// <summary>
		/// Draws interface and handles GUI events allowing user to provide additional inputs.
		/// </summary>
		public virtual void OnGUI() { }

        /// <summary>
		/// Draws interface and handles GUI events for standard option inputs.
		/// </summary>
		public void OnStandardGUI() {
            BracesOnNewLine = EditorGUILayout.ToggleLeft("Places braces on new lines", BracesOnNewLine);
        }

        /// <summary>
		/// Create new <see cref="ScriptBuilder"/> instance to build script.
		/// </summary>
		/// <returns>
		/// The <see cref="ScriptBuilder"/> instance.
		/// </returns>
		public ScriptBuilder CreateScriptBuilder() {
            return new ScriptBuilder();
        }

        /// <summary>
		/// Generate C# source code for template.
		/// </summary>
		/// <param name="scriptName">Name of script.</param>
		/// <param name="ns">Namespace for new script.</param>
		/// <returns>
		/// The generated source code.
		/// </returns>
		/// <seealso cref="CreateScriptBuilder"/>
		public abstract string GenerateScript(string scriptName, string ns);

    }
}
