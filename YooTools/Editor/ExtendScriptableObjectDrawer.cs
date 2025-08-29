using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace YooTools {
	[CustomPropertyDrawer(typeof(ScriptableObject), true)]
	public class ExtendScriptableObjectDrawer : PropertyDrawer {
		private readonly VisualElement _childrenProperties = new() {
			style = {
				marginLeft = 16, marginTop = 2
			}
		};
		private bool _expanded;

		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var container = new VisualElement();

			property.serializedObject.Update();

			var header = new VisualElement {
				style = {
					flexDirection = FlexDirection.Row, flexGrow = 1, justifyContent = Justify.SpaceBetween, alignItems = Align.Center
				}
			};

			container.Add(header);

			var triangle = new Label("▶") {
				style = {
					width = 16,
					fontSize = 10,
					unityTextAlign = TextAnchor.MiddleCenter,
					color = new StyleColor(new Color(0.5f, 0.5f, 0.5f)),
					display = property.objectReferenceValue == null
						? DisplayStyle.Flex
						: DisplayStyle.None
				}
			};

			header.Add(triangle);

			var objectField = new PropertyField(property) {
				style = {
					flexGrow = 1
				}
			};

			header.Add(objectField);

			var button = CreateButton(property);

			button.style.display = property.objectReferenceValue == null
				? DisplayStyle.Flex
				: DisplayStyle.None;

			header.Add(button);

			triangle.RegisterCallback<MouseUpEvent>(evt => {
					_expanded = !_expanded;

					triangle.text = _expanded
						? "▼"
						: "▶";

					_childrenProperties.style.display = _expanded
						? DisplayStyle.Flex
						: DisplayStyle.None;

					if (_expanded) {
						RefreshChildrenProperties(property, container);
					}
				}
			);

			objectField.RegisterValueChangeCallback(evt => {
					if (evt.changedProperty.objectReferenceValue != null) {
						button.style.display = DisplayStyle.None;
						triangle.style.display = DisplayStyle.Flex;
					} else {
						button.style.display = DisplayStyle.Flex;
						triangle.style.display = DisplayStyle.None;
					}
				}
			);

			return container;
		}

		private void RefreshChildrenProperties(SerializedProperty property, VisualElement parent) {
			_childrenProperties.Clear();

			if (property.objectReferenceValue == null) {
				return;
			}

			var so = new SerializedObject(property.objectReferenceValue);
			var iterator = so.GetIterator();
			iterator.NextVisible(true);

			while (iterator.NextVisible(false)) {
				if (iterator.name == "m_Script") {
					continue;
				}

				var propertyField = new PropertyField(iterator.Copy());
				propertyField.Bind(so);
				_childrenProperties.Add(propertyField);
			}

			parent.Add(_childrenProperties);
			InternalEditorUtility.RepaintAllViews();
		}

		private VisualElement CreateButton(SerializedProperty property) {
			var type = GetFieldType();

			var createButton = new Button(() => {
					var selectedAssetPath = "Assets";

					if (property.serializedObject.targetObject is MonoBehaviour behaviour) {
						var ms = MonoScript.FromMonoBehaviour(behaviour);
						selectedAssetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
					}

					property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
					property.serializedObject.ApplyModifiedProperties();

					EditorUtility.SetDirty(property.serializedObject.targetObject);
					InternalEditorUtility.RepaintAllViews();
				}
			) {
				text = "Create New"
			};

			return createButton;
		}

		private static ScriptableObject CreateAssetWithSavePrompt(Type type, string path) {
			path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", type.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);

			if (string.IsNullOrEmpty(path)) {
				return null;
			}

			var asset = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			EditorGUIUtility.PingObject(asset);

			return asset;
		}

		private Type GetFieldType() {
			if (fieldInfo == null) {
				return null;
			}

			var type = fieldInfo.FieldType;

			if (type.IsArray) {
				type = type.GetElementType();
			} else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
				type = type.GetGenericArguments()[0];
			}

			return type;
		}
	}
}