using UnityEditor;
using UnityEngine;

namespace YooTools.HierarchyIconDrawer {
    public static class UnityEditorBackgroundColor {
        private static readonly Color defaultColor = new Color(0.7843f, 0.7843f, 0.7843f);
        private static readonly Color defaultProColor = new Color(0.2196f, 0.2196f, 0.2196f);
        private static readonly Color selectedColor = new Color(0.22745f, 0.447f, 0.6902f);
        private static readonly Color selectedProColor = new Color(0.1725f, 0.3647f, 0.5294f);
        private static readonly Color selectedUnFocusColor = new Color(0.68f, 0.68f, 0.68f);
        private static readonly Color selectedUnFocusProColor = new Color(0.3f, 0.3f, 0.3f);
        private static readonly Color hoveredColor = new Color(0.698f, 0.698f, 0.698f);
        private static readonly Color hoveredProColor = new Color(0.2706f, 0.2706f, 0.2706f);

        public static Color GetColor(bool isSelected, bool isHovered, bool isWindowFocused) {
            if (isSelected) {
                if (isWindowFocused) {
                    return EditorGUIUtility.isProSkin ? selectedProColor : selectedColor;
                } else {
                    return EditorGUIUtility.isProSkin ? selectedUnFocusProColor : selectedUnFocusColor;
                }
            } else if (isHovered) {
                return EditorGUIUtility.isProSkin ? hoveredProColor : hoveredColor;
            }

            return EditorGUIUtility.isProSkin ? defaultProColor : defaultColor;
        }
    }
}
