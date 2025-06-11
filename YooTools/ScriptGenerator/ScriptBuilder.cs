using System.Text;
using UnityEngine;

namespace YooTools.ScriptGenerator {
    /// <summary>
    /// Helps you to build script files with support for automatically indenting source code.
    /// </summary>
    public class ScriptBuilder {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        private int indentLevel = 0;
        private string indent = "";
        private string indentChars = "\t";
        private string indentCharsNewLine = "\n";

        /// <summary>
        /// Property for current indent level within script.
        /// </summary>
        public int IndentLevel {
            get => indentLevel;
            set {
                value = Mathf.Max(0, value);
                if (value != indentLevel) {
                    if (value < indentLevel) {
                        stringBuilder.Length -= indentChars.Length;
                    }

                    indentLevel = value;
                    indent = "";
                    for (int i = 0; i < value; ++i) {
                        indent += indentChars;
                    }

                    indentCharsNewLine = "\n" + indent;
                }
            }
        }

        /// <summary>
        /// Property for sequence of characters to use when indenting text.
        /// </summary>
        /// <remarks>
        /// <para>Changing this value will not affect text witch has already been append.</para>
        /// </remarks>
        public string IndentChars {
            get => indentChars;
            set {
                int restoreIndent = indentLevel;
                indentLevel = -1;
                indentChars = value;
                indentLevel = restoreIndent;
            }
        }

        /// <summary>
        /// Clear output and start over.
        /// </summary>
        /// <remarks>
        /// <para>This method also resets indention to 0.</para>
        /// </remarks>
        public void Clear() {
            stringBuilder.Length = 0;
            IndentLevel = 0;
        }

        /// <summary>
        /// Append text to script.
        /// </summary>
        /// <param name="text">Text.</param>
        public void Append(string text) {
            stringBuilder.Append(text.Replace("\n", indentCharsNewLine));
        }

        /// <summary>
        /// Append blank line to script.
        /// </summary>
        public void AppendLine() {
            stringBuilder.Append(indentCharsNewLine);
        }


        /// <summary>
        /// Append text to script and begin new line.
        /// </summary>
        /// <param name="text">Text.</param>
        public void AppendLine(string text) {
            Append(text);
            stringBuilder.Append(indentCharsNewLine);
        }

        /// <summary>
        /// Begin namespace scope and automatically indent.
        /// </summary>
        /// <param name="text">Text.</param>
        public void BeginNamespace(string text) {
            Append(text);
            stringBuilder.AppendLine();
            ++IndentLevel;
            stringBuilder.Append(indentCharsNewLine);
        }

        /// <summary>
        /// End namespace scope and unindented.
        /// </summary>
        /// <param name="text">Text.</param>
        public void EndNamespace(string text) {
            --IndentLevel;
            Append(text);
            stringBuilder.AppendLine();
            stringBuilder.Append(indent);
            AppendLine();
        }

        /// <summary>
        /// Get generate source code as string.
        /// </summary>
        /// <returns>
        /// Source code as string.
        /// </returns>
        public override string ToString() {
            return stringBuilder.ToString().Trim() + "\n";
        }

    }
}
