using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace YooTools.HierarchyDrawer {
	[InitializeOnLoad]
	static public class HierarchyIconDrawer {
		private static readonly GUIContent RequiredIcon = EditorGUIUtility.IconContent("console.erroricon.sml", "One or more required fields are missing or empty!");
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
					// Jobb oldali ikon a Hierarchy mező végén
					var iconRect = new Rect(selectionRect.xMax - 18f, selectionRect.y + 1f, 16f, 16f);
					GUI.Label(iconRect, RequiredIcon);

					break;
				}
			}

			InternalEditorUtility.RepaintAllViews();
		}

		private static bool IsFieldUnassigned(object fieldValue) {
			switch (fieldValue) {
				case null:
				case string stringValue when string.IsNullOrEmpty(stringValue):
					return true;
				case IEnumerable enumerable: {
					return enumerable.Cast<object>().Any(item => item == null);
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
				CachedFieldInfo.Add(componentType, fields);
			}

			return fields;
		}
	}
}