using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YooTools.HierarchyIconDisplay {
    [InitializeOnLoad]

    public static class HierarchyIconDisplay {
        private static bool hasFocusWindow = false;
        private static EditorWindow hierarchyEditorWindow;

        static HierarchyIconDisplay() {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect) {
            var instanceObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (instanceObject == null)
                return;

            if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(instanceObject) != null)
                return;

            var components = instanceObject.GetComponents<Component>();
            if (components == null || components.Length == 0)
                return;

            var component = components.Length > 1 ? components[1] : components[0];
            var type = component.GetType();
            var content = EditorGUIUtility.ObjectContent(component, type);
            content.text = null;
            content.tooltip = type.Name;

            if (content.image == null)
                return;

            var isSelected = Selection.instanceIDs.Contains(instanceID);
            var isHovering = selectionRect.Contains(Event.current.mousePosition);

            var color = UnityEditorBackgroundColor.GetColor(isSelected, isHovering, hasFocusWindow);
            var backgroundRect = selectionRect;
            backgroundRect.width = 18.5f;
            EditorGUI.DrawRect(backgroundRect, color);

            EditorGUI.LabelField(selectionRect, content);
        }

        private static void OnEditorUpdate() {
            if (hierarchyEditorWindow == null)
                hierarchyEditorWindow = EditorWindow.GetWindow(Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));

            hasFocusWindow = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == hierarchyEditorWindow;
        }

    }
}
