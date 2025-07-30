using System;
using System.Collections.Generic;
using System.Reflection;

namespace YooX.SceneReferenceAttribute.Utils {
	public static class ReflectionUtil {
		public struct AttributedField<T> where T : Attribute {
			public T Attribute;
			public FieldInfo FieldInfo;
		}

		/// <summary>
		/// Finds all fields in the specified type (and its base types) that are decorated with the given attribute type <typeparamref name="T"/>.
		/// Adds each matching field and its attribute instance to the provided output list.
		/// </summary>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <param name="classToInspect">The type to inspect for attributed fields.</param>
		/// <param name="output">A list to which the found attributed fields will be added.</param>
		/// <param name="reflectionFlags">Binding flags to control field visibility and scope during reflection.</param>
		public static void GetFieldsWithAttributeFromType<T>(Type? classToInspect, IList<AttributedField<T>> output, BindingFlags reflectionFlags = BindingFlags.Default) where T : Attribute {
			var type = typeof(T);
			do {
				var allFields = classToInspect!.GetFields(reflectionFlags);
				foreach (var fieldInfo in allFields) {
					Attribute[] attributes = Attribute.GetCustomAttributes(fieldInfo);

					foreach (var attribute in attributes) {
						if (!type.IsInstanceOfType(attribute))
							continue;

						output.Add(new AttributedField<T> {
							Attribute = (T)attribute,
							FieldInfo = fieldInfo
						});
						break;
					}
				}

				classToInspect = classToInspect.BaseType;
			} while(classToInspect != null);
		}
	}

}