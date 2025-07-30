using System.Text;
using UnityEngine;

namespace YooTools.ScriptGenerator {
    /// <summary>
    /// Helps you to build script files with support for automatically indenting source code.
    /// </summary>
    public class ScriptBuilder {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        private int _indentLevel = 0;
        private string _indent = "";
        private string _indentChars = "\t";
        private string _indentCharsNewLine = "\n";

        /// <summary>
        /// Property for current indent level within script.
        /// </summary>
        public int IndentLevel {
            get => _indentLevel;
            set {
                value = Mathf.Max(0, value);
                if (value != _indentLevel) {
                    if (value < _indentLevel) {
                        _stringBuilder.Length -= _indentChars.Length;
                    }

                    _indentLevel = value;
                    _indent = "";
                    for (int i = 0; i < value; ++i) {
                        _indent += _indentChars;
                    }

                    _indentCharsNewLine = "\n" + _indent;
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
            get => _indentChars;
            set {
                var restoreIndent = _indentLevel;
                _indentLevel = -1;
                _indentChars = value;
                _indentLevel = restoreIndent;
            }
        }

        /// <summary>
        /// Clear output and start over.
        /// </summary>
        /// <remarks>
        /// <para>This method also resets indention to 0.</para>
        /// </remarks>
        public void Clear() {
            _stringBuilder.Length = 0;
            IndentLevel = 0;
        }

        /// <summary>
        /// Append text to script.
        /// </summary>
        /// <param name="text">Text.</param>
        public void Append(string text) {
            _stringBuilder.Append(text.Replace("\n", _indentCharsNewLine));
        }

        /// <summary>
        /// Append blank line to script.
        /// </summary>
        public void AppendLine() {
            _stringBuilder.Append(_indentCharsNewLine);
        }


        /// <summary>
        /// Append text to script and begin new line.
        /// </summary>
        /// <param name="text">Text.</param>
        public void AppendLine(string text) {
            Append(text);
            _stringBuilder.Append(_indentCharsNewLine);
        }

        /// <summary>
        /// Begin namespace scope and automatically indent.
        /// </summary>
        /// <param name="text">Text.</param>
        public void BeginNamespace(string text) {
            Append(text);
            _stringBuilder.AppendLine();
            ++IndentLevel;
            _stringBuilder.Append(_indentCharsNewLine);
        }

        /// <summary>
        /// End namespace scope and unindented.
        /// </summary>
        /// <param name="text">Text.</param>
        public void EndNamespace(string text) {
            --IndentLevel;
            Append(text);
            _stringBuilder.AppendLine();
            _stringBuilder.Append(_indent);
            AppendLine();
        }

        /// <summary>
        /// Get generate source code as string.
        /// </summary>
        /// <returns>
        /// Source code as string.
        /// </returns>
        public override string ToString() {
            return _stringBuilder.ToString().Trim() + "\n";
        }

    }
}
