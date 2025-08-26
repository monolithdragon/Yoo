using UnityEditor;
using UnityEngine;

namespace YooTools.HierarchyDrawer {
	[CustomPropertyDrawer(typeof(RequiredAttribute))]
	public class RequiredDrawer : PropertyDrawer {
		private readonly Texture2D _requiredIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.animation/Editor/Assets/EditorIcons/Dark/d_Warning@2x.png");

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			EditorGUI.BeginChangeCheck();

			var requiredFieldRect = new Rect(position.x, position.y, position.width - 20f, position.height);
			EditorGUI.PropertyField(requiredFieldRect, property, label);

			// If the field is required but unassigned, show the icon
			if (IsFieldUnassigned(property)) {
				var iconRect = new Rect(position.xMax - 18f, requiredFieldRect.y, 16f, 16f);
				GUI.Label(iconRect, new GUIContent(_requiredIcon, "This field is required and is either missing or empty!"));
			}

			if (EditorGUI.EndChangeCheck()) {
				property.serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(property.serializedObject.targetObject);

				// Force a repaint of the hierarchy
				EditorApplication.RepaintHierarchyWindow();
			}

			EditorGUI.EndProperty();
		}

		private bool IsFieldUnassigned(SerializedProperty property) {
			switch (property.propertyType) {
				case SerializedPropertyType.ObjectReference when property.objectReferenceValue:
				case SerializedPropertyType.ExposedReference when property.exposedReferenceValue:
				case SerializedPropertyType.AnimationCurve when property.animationCurveValue is {
					length: > 0
				}:
				case SerializedPropertyType.String when !string.IsNullOrEmpty(property.stringValue):
					return false;
				default:
					return true;
			}
		}
	}
}