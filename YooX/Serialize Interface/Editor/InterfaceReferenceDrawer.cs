using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YooX.SerializeInterface {
	[CustomPropertyDrawer(typeof(InterfaceReference<>)), CustomPropertyDrawer(typeof(InterfaceReference<,>))]
	public class InterfaceReferenceDrawer : PropertyDrawer {
		private const string UnderlyingValueFieldName = "underlyingValue";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var underlyingProperty = property.FindPropertyRelative(UnderlyingValueFieldName);
			var args = GetArguments(fieldInfo);

			EditorGUI.BeginProperty(position, label, property);

			var assignedObject = EditorGUI.ObjectField(position, label, underlyingProperty.objectReferenceValue, args.ObjectType, true);

			if (assignedObject != null) {
				Object component = null;

				if (assignedObject is GameObject gameObject) {
					component = gameObject.GetComponent(args.InterfaceType);
				} else if (args.InterfaceType.IsAssignableFrom(assignedObject.GetType())) {
					component = assignedObject;
				}

				if (component != null) {
					ValidateAndAssignObject(underlyingProperty, component, component.name, args.InterfaceType.Name);
				} else {
					Debug.LogWarning($"Assigned object does not implement required interface '{args.InterfaceType.Name}'.");
					underlyingProperty.objectReferenceValue = null;
				}
			} else {
				underlyingProperty.objectReferenceValue = null;
			}

			EditorGUI.EndProperty();
			InterfaceReferenceUtil.OnGUI(position, underlyingProperty, label, args);
		}

		private static InterfaceArgs GetArguments(FieldInfo fieldInfo) {
			var fieldType = fieldInfo.FieldType;

			bool TryGetTypesFromInterfaceReference(Type type, out Type objType, out Type intfType) {
				objType = null;
				intfType = null;

				if (type?.IsGenericType != true) {
					return false;
				}

				var genericType = type.GetGenericTypeDefinition();

				if (genericType == typeof(InterfaceReference<>)) {
					type = type.BaseType;
				}

				if (type?.GetGenericTypeDefinition() == typeof(InterfaceReference<,>)) {
					var types = type.GetGenericArguments();
					intfType = types[0];
					objType = types[1];

					return true;
				}

				return false;
			}

			void GetTypesFromList(Type type, out Type objType, out Type intType) {
				objType = null;
				intType = null;

				var listInterface = type.GetInterfaces()
										.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));

				if (listInterface != null) {
					var elementType = listInterface.GetGenericArguments()[0];
					TryGetTypesFromInterfaceReference(elementType, out objType, out intType);
				}
			}

			if (!TryGetTypesFromInterfaceReference(fieldType, out var objectType, out var interfaceType)) {
				GetTypesFromList(fieldType, out objectType, out interfaceType);
			}

			return new InterfaceArgs(objectType, interfaceType);
		}

		private static void ValidateAndAssignObject(SerializedProperty property, Object targetObject, string componentNameOrType, string interfaceName = null) {
			if (targetObject != null) {
				property.objectReferenceValue = targetObject;
			} else {
				string message = interfaceName != null
					? $"GameObject '{componentNameOrType}'"
					: "assigned object";

				Debug.LogWarning(
					$"The {message} does not have a component that implements '{interfaceName}'."
				);

				property.objectReferenceValue = null;
			}
		}
	}

	public readonly struct InterfaceArgs {
		public readonly Type ObjectType;
		public readonly Type InterfaceType;

		public InterfaceArgs(Type objectType, Type interfaceType) {
			Debug.Assert(typeof(Object).IsAssignableFrom(objectType), $"{nameof(objectType)} needs to be of Type {typeof(Object)}.");
			Debug.Assert(interfaceType.IsInterface, $"{nameof(interfaceType)} needs to be an interface.");

			ObjectType = objectType;
			InterfaceType = interfaceType;
		}
	}
}