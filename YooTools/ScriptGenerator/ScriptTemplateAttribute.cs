using System;

namespace YooTools.ScriptGenerator {
    /// <summary>
    /// Mark class as script template.  
    /// </summary>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ScriptTemplateAttribute : Attribute {
        public string Description { get; private set; }
        public int Priority { get; private set; }

        /// <summary>
        /// Initialize a new <see cref="ScriptTemplateAttribute"/> instance.
        /// </summary>
        /// <param name="description">Description of script template</param>
        /// <param name="priority">Priority some control over ordering in user interface</param>
        public ScriptTemplateAttribute(string description, int priority = 1000) {
            Description = description;
            Priority = priority;
        }
    }
}
