#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_2022_2_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace YooX.SceneReferenceAttribute {
    /// <summary>
    /// Custom property drawer for the reference attributes, making them read-only.
    /// 
    /// Note: Does not apply to the Anywhere attribute as that needs to remain editable. 
    /// </summary>
    [CustomPropertyDrawer(typeof(SelfAttribute))]
    [CustomPropertyDrawer(typeof(ChildAttribute))]
    [CustomPropertyDrawer(typeof(ParentAttribute))]
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneRefAttributePropertyDrawer : PropertyDrawer {

        private bool _isInitialized;
        private bool _canValidateType;
        private Type? _elementType;
        private string? _typeName;

        private SceneRefAttribute SceneRefAttribute => (SceneRefAttribute)attribute;
        private bool Editable => SceneRefAttribute.HasFlags(Flag.Editable);

        // unity 2022.2 makes UIToolkit the default for inspectors
#if UNITY_2022_2_OR_NEWER
        private const string SCENE_REF_CLASS = "sceneref";

        private PropertyField? _propertyField;
        private HelpBox? _helpBox;
        private InspectorElement? _inspectorElement;
        private SerializedProperty? _serializedProperty;

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            _serializedProperty = property;
            Initialize(property);

            var root = new VisualElement();
            root.AddToClassList(SCENE_REF_CLASS);

            _helpBox = new HelpBox("", HelpBoxMessageType.Error);
            _helpBox.style.display = DisplayStyle.None;
            root.Add(_helpBox);

            _propertyField = new PropertyField(property);
            _propertyField.SetEnabled(Editable);
            root.Add(_propertyField);

            if (_canValidateType) {
                UpdateHelpBox();
                _propertyField.RegisterCallback<AttachToPanelEvent>(OnAttach);
            }
            return root;
        }

        private void OnAttach(AttachToPanelEvent attachToPanelEvent) {
            _propertyField?.UnregisterCallback<AttachToPanelEvent>(OnAttach);
            _inspectorElement = _propertyField?.GetFirstAncestorOfType<InspectorElement>();
            if (_inspectorElement == null)
                // not in an inspector, invalid
                return;

            // subscribe to SerializedPropertyChangeEvent so we can update when the property changes
            _inspectorElement.RegisterCallback<SerializedPropertyChangeEvent>(OnSerializedPropertyChangeEvent);
            _propertyField?.RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        private void OnDetach(DetachFromPanelEvent detachFromPanelEvent) {
            // unregister from all callbacks
            _propertyField?.UnregisterCallback<DetachFromPanelEvent>(OnDetach);
            _inspectorElement?.UnregisterCallback<SerializedPropertyChangeEvent>(OnSerializedPropertyChangeEvent);
            _serializedProperty = null;
        }

        private void OnSerializedPropertyChangeEvent(SerializedPropertyChangeEvent changeEvent) {
            if (changeEvent.changedProperty != _serializedProperty)
                return;
            UpdateHelpBox();
        }

        private void UpdateHelpBox() {
            bool isSatisfied = IsSatisfied(_serializedProperty!);
            if (_helpBox != null) {
                _helpBox.style.display = isSatisfied ? DisplayStyle.None : DisplayStyle.Flex;
                string message = $"Missing {_serializedProperty?.propertyPath} ({_typeName}) reference on {SceneRefAttribute.Loc}!";
                _helpBox.text = message;
            }
        }
#endif

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (!_isInitialized)
                Initialize(property);

            if (!IsSatisfied(property)) {
                Rect helpBoxPos = position;
                helpBoxPos.height = EditorGUIUtility.singleLineHeight * 2;
                string message = $"Missing {property.propertyPath} ({_typeName}) reference on {SceneRefAttribute.Loc}!";
                EditorGUI.HelpBox(helpBoxPos, message, MessageType.Error);
                position.height = EditorGUI.GetPropertyHeight(property, label);
                position.y += helpBoxPos.height;
            }

            bool wasEnabled = GUI.enabled;
            GUI.enabled = Editable;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = wasEnabled;
        }

        private void Initialize(SerializedProperty property) {
            _isInitialized = true;

            // the type won't change, so we only need to initialize these values once
            _elementType = fieldInfo.FieldType;
            if (typeof(ISerializable).IsAssignableFrom(_elementType)) {
                Type interfaceType = _elementType.GetInterfaces().FirstOrDefault(type =>
                    type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ISerializable<>));
                if (interfaceType != null)
                    _elementType = interfaceType.GetGenericArguments()[0];
            }

            _canValidateType = typeof(Component).IsAssignableFrom(_elementType)
                                     && property.propertyType == SerializedPropertyType.ObjectReference;

            _typeName = fieldInfo.FieldType.Name;
            if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GenericTypeArguments.Length >= 1)
                _typeName = _typeName.Replace("`1", $"<{fieldInfo.FieldType.GenericTypeArguments[0].Name}>");
        }

        /// <summary>Is this field Satisfied with a value or optional</summary>
        private bool IsSatisfied(SerializedProperty property) {
            if (!_canValidateType || SceneRefAttribute.HasFlags(Flag.Optional))
                return true;
            return property.objectReferenceValue != null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float helpBoxHeight = 0;
            if (!IsSatisfied(property))
                helpBoxHeight = EditorGUIUtility.singleLineHeight * 2;
            return EditorGUI.GetPropertyHeight(property, label) + helpBoxHeight;
        }
    }
}
#endif

