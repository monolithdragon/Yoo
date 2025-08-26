using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace YooTools.HierarchyDrawer {
	[InitializeOnLoad]
	static public class HierarchyIconDrawer {
		private static readonly Texture2D RequiredIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.animation/Editor/Assets/EditorIcons/Dark/d_Warning@2x.png");
		private static readonly Dictionary<Type, FieldInfo[]> CachedFieldInfo = new();

		static HierarchyIconDrawer() => EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;

		private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect) {
			if (EditorUtility.InstanceIDToObject(instanceID) is not GameObject gameObject) {
				return;
			}

			foreach (var component in gameObject.GetComponents<Component>()) {
				if (component == null) {
					continue;
				}

				var fields = GetCachedFieldsWithRequireAttribute(component.GetType());

				if (fields == null) {
					continue;
				}

				if (fields.Any(field => IsFieldUnassigned(field.GetValue(component)))) {
					var iconRect = new Rect(selectionRect.xMax - 20f, selectionRect.y, 16f, 16f);
					GUI.Label(iconRect, new GUIContent(RequiredIcon, "One or more required fields are missing or empty!"));

					break;
				}
			}
		}

		private static bool IsFieldUnassigned(object fieldValue) {
			switch (fieldValue) {
				case null:
				case string stringValue when string.IsNullOrEmpty(stringValue):
					return true;
				case IEnumerable enumerable: {
					if (enumerable.Cast<object>().Any(item => item == null)) {
						return true;
					}

					break;
				}
			}

			return false;
		}

		private static FieldInfo[] GetCachedFieldsWithRequireAttribute(Type componentType) {
			if (!CachedFieldInfo.TryGetValue(componentType, out var fields)) {
				fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var requiredFields = new List<FieldInfo>();

				foreach (var field in fields) {
					bool isSerialized = field.IsPublic || field.IsDefined(typeof(SerializeField), false);
					bool isRequired = field.IsDefined(typeof(RequiredAttribute), false);

					if (isSerialized && isRequired) {
						requiredFields.Add(field);
					}
				}

				fields = requiredFields.ToArray();
				CachedFieldInfo[componentType] = fields;
			}

			return fields;
		}
	}
}