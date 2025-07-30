using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace YooTools.ScriptGenerator {
	/// <summary>
	/// Describes script template generator
	/// </summary>
	public class ScriptGeneratorDescriptor {
		private static readonly List<ScriptGeneratorDescriptor> GeneratorDescriptors = new();
		private static readonly ReadOnlyCollection<ScriptGeneratorDescriptor> DescriptorsReadOnly;

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
		public static IList<ScriptGeneratorDescriptor> Descriptors => DescriptorsReadOnly;

		/// <summary>
		/// Create new instance of script generator.
		/// </summary>
		/// <return>
		/// The new <see cref="ScriptTemplateGenerator"/> instance.
		/// </return>
		public ScriptTemplateGenerator CreateInstance() => Activator.CreateInstance(ScriptType) as ScriptTemplateGenerator;

		static ScriptGeneratorDescriptor() {
			// Gather script template generator types.
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				foreach (var type in assembly.GetTypes()) {
					if (type.IsDefined(typeof(ScriptTemplateAttribute), true)) {
						GeneratorDescriptors.Add(new ScriptGeneratorDescriptor {
							ScriptType = type,
							TemplateAttribute = (ScriptTemplateAttribute)type.GetCustomAttributes(typeof(ScriptTemplateAttribute), true).First()
						});
					}
				}
			}

			// Sort descriptor by priority
			GeneratorDescriptors.Sort((a, b) => a.TemplateAttribute.Priority - b.TemplateAttribute.Priority);

			// We want to expose read only access to the collection
			DescriptorsReadOnly = new ReadOnlyCollection<ScriptGeneratorDescriptor>(GeneratorDescriptors);
		}

	}
}