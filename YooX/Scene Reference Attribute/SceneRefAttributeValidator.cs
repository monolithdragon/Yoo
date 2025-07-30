#if UNITY_EDITOR
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooX.SceneReferenceAttribute.Utils;
using Object = UnityEngine.Object;

namespace YooX.SceneReferenceAttribute {
	public static class SceneRefAttributeValidator {
		private static readonly IList<ReflectionUtil.AttributedField<SceneRefAttribute>> AttributedFieldsCache = new List<ReflectionUtil.AttributedField<SceneRefAttribute>>();

#if UNITY_EDITOR
        /// <summary>
        /// Validate all references for every script and every game object in the scene.
        /// </summary>
        [MenuItem("Yoo Tools/Validate All Refs")]
        public static bool ValidateAllRefs() {
            var validationSuccess = true;
            MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var runtimeMonoScript in scripts) {
                var scriptType = runtimeMonoScript.GetClass();
                if (scriptType == null) {
                    continue;
                }

                try {
                    ReflectionUtil.GetFieldsWithAttributeFromType(
                        scriptType,
                        AttributedFieldsCache,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                    );
                    if (AttributedFieldsCache.Count == 0) {
                        continue;
                    }


                    Object[] objects = Object.FindObjectsByType(scriptType, FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

                    if (objects.Length == 0) {
                        continue;
                    }

                    Debug.Log($"Validating {AttributedFieldsCache.Count} field(s) on {objects.Length} {objects[0].GetType().Name} instance(s)");
                    validationSuccess = objects.Aggregate(validationSuccess, (current, t) => current & Validate((MonoBehaviour)t, AttributedFieldsCache, false));
                } finally {
                    AttributedFieldsCache.Clear();
                }
            }
            return validationSuccess;
        }

        /// <summary>
        /// Validate a single components references, attempting to assign missing references
        /// and logging errors as necessary.
        /// </summary>
        [MenuItem("CONTEXT/Component/Validate Refs")]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used as menu item action")]
        private static void ValidateRefs(MenuCommand menuCommand)
            => Validate((Component)menuCommand.context);

        /// <summary>
        /// Clean and validate a single components references. Useful in instances where (for example) Unity has
        /// incorrectly serialized a scene reference within a prefab.
        /// </summary>
        [MenuItem("CONTEXT/Component/Clean and Validate Refs")]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used as menu item action")]
        private static void CleanValidateRefs(MenuCommand menuCommand)
            => CleanValidate((Component)menuCommand.context);

#endif

		/// <summary>
		/// Validate a single components references, attempting to assign missing references
		/// and logging errors as necessary.
		/// </summary>
		public static void ValidateRefs(this Component c, bool updateAtRuntime = false)
			=> Validate(c, updateAtRuntime);

		/// <summary>
		/// Validate a single components references, attempting to assign missing references
		/// and logging errors as necessary.
		/// </summary>
		public static void Validate(Component c, bool updateAtRuntime = false) {
			try {
				ReflectionUtil.GetFieldsWithAttributeFromType(
					c.GetType(),
					AttributedFieldsCache,
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
				);
				Validate(c, AttributedFieldsCache, updateAtRuntime);
			} finally {
				AttributedFieldsCache.Clear();
			}
		}

		/// <summary>
		/// Clean and validate a single components references. Useful in instances where (for example) Unity has
		/// incorrectly serialized a scene reference within a prefab.
		/// </summary>
		public static void CleanValidate(Component c, bool updateAtRuntime = false) {
			try {
				ReflectionUtil.GetFieldsWithAttributeFromType(
					c.GetType(),
					AttributedFieldsCache,
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
				);
				Clean(c, AttributedFieldsCache);
				Validate(c, AttributedFieldsCache, updateAtRuntime);
			} finally {
				AttributedFieldsCache.Clear();
			}
		}

		private static bool Validate(Component c,
		                             IList<ReflectionUtil.AttributedField<SceneRefAttribute>> requiredFields,
		                             bool updateAtRuntime) {
			if (requiredFields.Count == 0) {
				Debug.LogWarning($"{c.GetType().Name} has no required fields", c.gameObject);
				return true;
			}

			var validationSuccess = true;
			var isUninstantiatedPrefab = PrefabUtil.IsUninstantiatedPrefab(c.gameObject);
			foreach (var attributedField in requiredFields) {
				var attribute = attributedField.Attribute;
				var field = attributedField.FieldInfo;

				if (field.FieldType.IsInterface) {
					throw new Exception($"{c.GetType().Name} cannot serialize interface {field.Name} directly, use InterfaceRef instead");
				}

				var fieldValue = field.GetValue(c);
				if (updateAtRuntime || !Application.isPlaying) {
					fieldValue = UpdateRef(attribute, c, field, fieldValue);
				}

				if (isUninstantiatedPrefab) {
					continue;
				}

				validationSuccess &= ValidateRef(attribute, c, field, fieldValue!);
			}
			return validationSuccess;
		}

		private static void Clean(Component c,
		                          IList<ReflectionUtil.AttributedField<SceneRefAttribute>> requiredFields) {
			foreach (var attributedField in requiredFields) {
				var attribute = attributedField.Attribute;
				if (attribute.Loc == RefLoc.Anywhere) {
					continue;
				}

				var field = attributedField.FieldInfo;
				field.SetValue(c, null);
			}
		}

		private static object? UpdateRef(SceneRefAttribute attr,
		                                 Component component,
		                                 FieldInfo field,
		                                 object? existingValue// Change existingValue to nullable type
		) {
			var fieldType = field.FieldType;
			var excludeSelf = attr.HasFlags(Flag.ExcludeSelf);
			var isCollection = IsCollectionType(fieldType, out _, out var isList);
			var includeInactive = attr.HasFlags(Flag.IncludeInactive);

			ISerializable? iSerializable = null;
			if (typeof(ISerializable).IsAssignableFrom(fieldType)) {
				iSerializable = (ISerializable)(existingValue ?? Activator.CreateInstance(fieldType));
				fieldType = iSerializable.RefType;
				existingValue = iSerializable.SerializedObject;
			}

			if (attr.HasFlags(Flag.Editable)) {
				var isFilledArray = isCollection && ((IEnumerable)existingValue!).CountEnumerable() > 0;
				if (isFilledArray || existingValue is Object) {
					// If the field is editable and the value has already been set, keep it.
					return existingValue;
				}
			}

			var elementType = fieldType;
			if (isCollection) {
				elementType = GetElementType(fieldType);
				if (typeof(ISerializable).IsAssignableFrom(elementType)) {
					var interfaceType = elementType?
					                    .GetInterfaces()
					                    .FirstOrDefault(type =>
						                                    type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ISerializable<>));

					if (interfaceType != null) {
						elementType = interfaceType.GetGenericArguments()[0];
					}
				}
			}

			object? value = null;

			//INFO: when minimal unity version will be high enough, explicit casts to object will not be necessary.
			switch(attr.Loc) {
				case RefLoc.Anywhere:
					if (isCollection ? typeof(ISerializable).IsAssignableFrom(fieldType.GetElementType()) : iSerializable != null) {
						value = isCollection ? (object?)((ISerializable[])existingValue!).Select(existingRef => GetComponentIfWrongType(existingRef.SerializedObject, elementType!)).ToArray() : (object?)GetComponentIfWrongType(existingValue!, elementType!);
					}
					break;

				case RefLoc.Self:
					value = isCollection ? (object?)component.GetComponents(elementType) : (object?)component.GetComponent(elementType);
					break;

				case RefLoc.Parent:
					value = isCollection ? (object?)GetComponentsInParent(component, elementType!, includeInactive, excludeSelf) : (object?)GetComponentInParent(component, elementType!, includeInactive, excludeSelf);
					break;

				case RefLoc.Child:
					value = isCollection ? (object?)GetComponentsInChildren(component, elementType!, includeInactive, excludeSelf) : GetComponentInChildren(component, elementType!, includeInactive, excludeSelf);
					break;

				case RefLoc.Scene:
					value = GetComponentsInScene(elementType!, includeInactive, isCollection);
					break;
				default:
					throw new Exception($"Unhandled Loc={attr.Loc}");
			}

			if (value == null) {
				return existingValue;
			}

			var filter = attr.Filter;

			if (isCollection) {
				var realElementType = GetElementType(fieldType);

				var componentArray = (Array)value;
				if (filter != null) {
					componentArray = componentArray
					                 .Cast<object>()
					                 .Where(o => filter.IncludeSceneRef(o))
					                 .ToArray();

				}

				var typedArray = Array.CreateInstance(
					realElementType ?? throw new InvalidOperationException(),
					componentArray.Length
				);

				if (elementType == realElementType) {
					Array.Copy(componentArray, typedArray, typedArray.Length);
					value = typedArray;
				} else if (typeof(ISerializable).IsAssignableFrom(realElementType)) {
					for (var i = 0; i < typedArray.Length; i++) {
						var elementValue = (ISerializable)Activator.CreateInstance(realElementType);
						elementValue?.OnSerialize(componentArray.GetValue(i));
						typedArray.SetValue(elementValue, i);
					}
					value = typedArray;
				}
			} else if (filter?.IncludeSceneRef(value) == false) {
				iSerializable?.Clear();
#if UNITY_EDITOR
                if (existingValue != null)
                {
                    EditorUtility.SetDirty(component);
                }
#endif
				return null;
			}

			if (iSerializable == null) {
				var valueIsEqual = existingValue != null && isCollection ? ((IEnumerable<object>)value).SequenceEqual((IEnumerable<object>)existingValue) : value.Equals(existingValue);
				if (valueIsEqual) {
					return existingValue;
				}

				if (isList) {
					var listType = typeof(List<>);
					Type[] typeArgs = { fieldType.GenericTypeArguments[0] };
					var constructedType = listType.MakeGenericType(typeArgs);

					var newList = Activator.CreateInstance(constructedType);

					var addMethod = newList.GetType().GetMethod(nameof(List<object>.Add));

					foreach (var s in (IEnumerable)value) {
						addMethod!.Invoke(newList, [s!]);
					}

					field.SetValue(component, newList);
				} else {
					field.SetValue(component, value);
				}
			} else {
				if (!iSerializable.OnSerialize(value)) {
					return existingValue;
				}
			}

#if UNITY_EDITOR
            EditorUtility.SetDirty(component);
#endif
			return value;
		}

		private static IEnumerable<object> GetFilteredComponents(Array components, SceneRefFilter filter) {
			foreach (var o in components) {
				if (filter.IncludeSceneRef(o)) {
					yield return o;
				}
			}
		}

		private static Type? GetElementType(Type fieldType) => fieldType.IsArray ? fieldType.GetElementType() : fieldType.GenericTypeArguments[0];

		private static object GetComponentIfWrongType(object existingValue, Type elementType) {
			if (existingValue is Component existingComponent && existingComponent && !elementType.IsInstanceOfType(existingValue)) {
				return existingComponent.GetComponent(elementType);
			}

			return existingValue;
		}

		private static bool ValidateRef(SceneRefAttribute attr, Component c, FieldInfo field, object value) {
			var fieldType = field.FieldType;
			var isCollection = IsCollectionType(fieldType, out _, out _);
			var isOverridable = attr.HasFlags(Flag.EditableAnywhere);

			if (value is ISerializable ser) {
				value = ser.SerializedObject;
			}

			if (IsEmptyOrNull(value, isCollection)) {
				if (attr.HasFlags(Flag.Optional))
					return true;

				var elementType = isCollection ? fieldType.GetElementType() : fieldType;
				elementType = typeof(ISerializable).IsAssignableFrom(elementType) ? elementType!.GetGenericArguments()[0] : elementType;
				Debug.LogError($"{c.GetType().Name} missing required {elementType!.Name + (isCollection ? "[]" : "")} ref '{field.Name}'", c.gameObject);

				return false;
			}

			if (isCollection) {
				var validationSuccess = true;
				var a = (IEnumerable)value;
				var enumerator = a.GetEnumerator();

				var elementType = fieldType.GetElementType();

				while(enumerator.MoveNext()) {
					var o = enumerator.Current;
					if (o is ISerializable serObj) {
						o = serObj.SerializedObject;
					}

					if (o != null) {
						if (isOverridable)
							continue;

						if (attr.HasFlags(Flag.ExcludeSelf) && o is Component valueC && valueC.gameObject == c.gameObject)
							Debug.LogError($"{c.GetType().Name} {elementType?.Name}[] ref '{field.Name}' cannot contain component from the same GameObject", c.gameObject);

						validationSuccess &= ValidateRefLocation(attr.Loc, c, field, o);
					} else {
						Debug.LogError($"{c.GetType().Name} missing required element ref in array '{field.Name}'", c.gameObject);
						validationSuccess = false;
					}
				}
				return validationSuccess;
			} else {
				if (isOverridable)
					return true;

				if (attr.HasFlags(Flag.ExcludeSelf) && value is Component valueC && valueC.gameObject == c.gameObject)
					Debug.LogError($"{c.GetType().Name} {fieldType.Name} ref '{field.Name}' cannot be on the same GameObject", c.gameObject);

				return ValidateRefLocation(attr.Loc, c, field, value);
			}
		}

		private static bool ValidateRefLocation(RefLoc loc, Component c, FieldInfo field, object refObj) {
			return refObj switch {
				Component valueC => ValidateRefLocation(loc, c, field, valueC),
				ScriptableObject _ => ValidateRefLocationAnywhere(loc, c, field),
				GameObject _ => ValidateRefLocationAnywhere(loc, c, field),
				_ => throw new Exception($"{c.GetType().Name} has unexpected reference type {refObj?.GetType().Name}")
			};
		}

		private static bool ValidateRefLocation(RefLoc loc, Component c, FieldInfo field, Component refObj) {
			switch(loc) {
				case RefLoc.Anywhere:
					break;

				case RefLoc.Self:
					if (refObj.gameObject != c.gameObject) {
						Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be on Self", c.gameObject);
						return false;
					}

					break;

				case RefLoc.Parent:
					if (!c.transform.IsChildOf(refObj.transform)) {
						Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Parent", c.gameObject);
						return false;
					}

					break;

				case RefLoc.Child:
					if (!refObj.transform.IsChildOf(c.transform)) {
						Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Child", c.gameObject);
						return false;
					}

					break;

				case RefLoc.Scene:
					if (c == null) {
						Debug.LogError($"{c?.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be in the scene", c?.gameObject);
						return false;
					}
					break;

				default:
					throw new Exception($"Unhandled Loc={loc}");
			}
			return true;
		}

		private static bool ValidateRefLocationAnywhere(RefLoc loc, Component c, FieldInfo field) {
			switch(loc) {
				case RefLoc.Anywhere:
					return true;

				case RefLoc.Self:
				case RefLoc.Parent:
				case RefLoc.Child:
				case RefLoc.Scene:
					Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be Anywhere", c.gameObject);
					return false;

				default:
					throw new Exception($"Unhandled Loc={loc}");
			}
		}

		private static bool IsEmptyOrNull(object obj, bool isCollection) {
			if (obj is ISerializable ser) {
				return !ser.HasSerializedObject;
			}

			return obj.Equals(null) || isCollection && ((IEnumerable)obj).CountEnumerable() == 0;
		}

		private static bool IsCollectionType(Type t, out bool isArray, out bool isList) {
			isList = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
			isArray = t.IsArray;
			return isList || isArray;
		}

		private static Component[] GetComponentsInParent(Component c, Type elementType, bool includeInactive, bool excludeSelf) {
			var element = c;
			if (excludeSelf)
				element = c.transform.parent;

			return element == null ? Array.Empty<Component>() : element.GetComponentsInParent(elementType, includeInactive);
		}

		private static Component GetComponentInParent(Component c, Type elementType, bool includeInactive,
		                                              bool excludeSelf) {
			var element = c;
			if (excludeSelf)
				element = c.transform.parent;

			return element.GetComponentInParent(elementType, includeInactive);
		}

		private static Component[] GetComponentsInChildren(Component c, Type elementType, bool includeInactive, bool excludeSelf) {
			if (!excludeSelf)
				return c.GetComponentsInChildren(elementType, includeInactive);

			var components = new List<Component>();

			var transform = c.transform;
			var childCount = transform.childCount;

			for (var i = 0; i < childCount; ++i) {
				var child = transform.GetChild(i);
				components.AddRange(child.GetComponentsInChildren(elementType, includeInactive));
			}

			return components.ToArray();
		}

		private static Component? GetComponentInChildren(Component c, Type elementType, bool includeInactive,
		                                                 bool excludeSelf) {
			if (!excludeSelf)
				return c.GetComponentInChildren(elementType, includeInactive);

			var transform = c.transform;
			var childCount = transform.childCount;

			for (var i = 0; i < childCount; ++i) {
				var child = transform.GetChild(i);
				var component = child.GetComponentInChildren(elementType, includeInactive);

				if (component != null)
					return component;
			}

			return null;
		}

		private static object? GetComponentsInScene(Type elementType, bool includeInactive, bool isCollection) {
			var isUnityType = elementType.IsSubclassOf(typeof(Object));

			if (isUnityType)
				return isCollection ? (object?)Object.FindObjectsByType(elementType, includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID) : (object?)Object.FindFirstObjectByType(elementType, includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);

			var elements = Object.FindObjectsByType<MonoBehaviour>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

			elements = elements.Where(IsCorrectType).ToArray();
			if (isCollection)
				return (object?)elements;

			return elements.Length > 0 ? (object?)elements[0] : null;

			bool IsCorrectType(MonoBehaviour e) {
				return elementType.IsInterface ? e.GetType().GetInterfaces().Contains(elementType) : e.GetType().IsSubclassOf(elementType);
			}
		}
	}

}