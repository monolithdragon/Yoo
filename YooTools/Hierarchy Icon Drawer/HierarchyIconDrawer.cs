using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace YooTools.HierarchyIconDrawer {
	[InitializeOnLoad]
	public static class HierarchyIconDrawer {
		private static readonly Texture2D RequiredIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Icons/RequiredIcon.png");
		private static bool hasFocusWindow = false;
		private static EditorWindow hierarchyEditorWindow;
		private static readonly Dictionary<Type, FieldInfo[]> CacheFieldInfos = new();

		static HierarchyIconDrawer() {
			EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
			EditorApplication.update += OnEditorUpdate;
		}

		private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect) {
			if (EditorUtility.InstanceIDToObject(instanceID) is not GameObject instanceObject) return;

			if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(instanceObject))
				return;

			var components = instanceObject.GetComponents<Component>();
			foreach (var component in components) {
				if (!component) continue;

				var type = component.GetType();

				var fields = GetCachedFieldsWithRequiredAttribute(type);
				if (fields == null) continue;
				if (fields.Any(field => IsFieldUnassigned(field.GetValue(component)))) {
					var iconRect = new Rect(selectionRect.xMax - 20f, selectionRect.y, 16f, 16f);
					GUI.Label(iconRect, new GUIContent(RequiredIcon, "One or more required fields missing or empty!"));
				}

				var content = EditorGUIUtility.ObjectContent(component, type);
				content.text = null;
				content.tooltip = type.Name;

				if (!content.image)
					return;

				EditorGUI.LabelField(selectionRect, content);
			}

			var isSelected = Selection.instanceIDs.Contains(instanceID);
			var isHovering = selectionRect.Contains(Event.current.mousePosition);

			var color = UnityEditorBackgroundColor.GetColor(isSelected, isHovering, hasFocusWindow);
			var backgroundRect = selectionRect;
			backgroundRect.width = 18.5f;
			EditorGUI.DrawRect(backgroundRect, color);

		}

		private static void OnEditorUpdate() {
			if (hierarchyEditorWindow == null)
				hierarchyEditorWindow = EditorWindow.GetWindow(Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));

			hasFocusWindow = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == hierarchyEditorWindow;
		}

		private static bool IsFieldUnassigned(object fieldValue) {
			switch(fieldValue) {
				case null:
				case string stringValue when string.IsNullOrEmpty(stringValue):
					return true;
				case System.Collections.IEnumerable enumerableValue: {
					return enumerableValue.Cast<object>().Any(item => item == null);

				}
			}
			return false;

		}

		private static FieldInfo[] GetCachedFieldsWithRequiredAttribute(Type componentType) {
			if (!CacheFieldInfos.TryGetValue(componentType, out var fields)) {
				fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var requiredFields = new List<FieldInfo>();

				foreach (var field in fields) {
					var isSerialized = field.IsPublic || field.IsDefined(typeof(SerializeField), false);
					var isRequired = field.IsDefined(typeof(RequiredAttribute), false);

					if (isRequired && isSerialized) requiredFields.Add(field);

					fields = requiredFields.ToArray();
					CacheFieldInfos[componentType] = fields;
				}
			}
			return fields;
		}

	}
}