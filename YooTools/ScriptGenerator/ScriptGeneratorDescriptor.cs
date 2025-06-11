using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace YooTools.ScriptGenerator {
    /// <summary>
    /// Describes script template generator
    /// </summary>
    public class ScriptGeneratorDescriptor {
        private static readonly List<ScriptGeneratorDescriptor> descriptors;
        private static readonly ReadOnlyCollection<ScriptGeneratorDescriptor> descriptorsReadOnly;

        /// <summary>
        /// Type of script template generator.
        /// </summary>
        public Type ScriptType { get; private set; }

        /// <summary>
        /// Associated attribute witch include description of template generator.
        /// </summary>
        public ScriptTemplateAttribute TemplateAttribute { get; private set; }

        /// <summary>
        /// Gets read only collections of script template generator descriptors.
        /// </summary>
        public static IList<ScriptGeneratorDescriptor> Descriptors => descriptorsReadOnly;

        /// <summary>
        /// Create new instance of script generator.
        /// </summary>
        /// <return>
        /// The new <see cref="ScriptTemplateGenerator"/> instance.
        /// </return>
        public ScriptTemplateGenerator CreateInstance() {
            return Activator.CreateInstance(ScriptType) as ScriptTemplateGenerator;
        }

        static ScriptGeneratorDescriptor() {
            // Gather script template generator types.
            descriptors = new List<ScriptGeneratorDescriptor>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in assembly.GetTypes()) {
                    if (type.IsDefined(typeof(ScriptTemplateAttribute), true)) {
                        descriptors.Add(new ScriptGeneratorDescriptor() {
                            ScriptType = type,
                            TemplateAttribute = (ScriptTemplateAttribute)type.GetCustomAttributes(typeof(ScriptTemplateAttribute), true).First()
                        });
                    }
                }
            }

            // Sort descriptor by priority
            descriptors.Sort((a, b) => a.TemplateAttribute.Priority - b.TemplateAttribute.Priority);

            // We want to expose read only access to the collection
            descriptorsReadOnly = new ReadOnlyCollection<ScriptGeneratorDescriptor>(descriptors);
        }

    }
}
