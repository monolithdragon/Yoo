using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace YooTools {
	/// <summary>
	/// Extends how ScriptableObject object references are displayed in the inspector
	/// Shows you all values under the object reference
	/// Also provides a button to create a new ScriptableObject if property is null.
	/// </summary>
	[CustomPropertyDrawer(typeof(ScriptableObject), true)]
	public class ExtendScriptableObjectDrawer : PropertyDrawer {
		private VisualElement _childContainer;
		private static readonly List<string> IgnoreClassFullNames = new() {
			"TMPro.TMP_FontAsset"
		};

		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var container = new VisualElement();

			var type = GetFieldType();

			if (type == null || IgnoreClassFullNames.Contains(type.FullName)) {
				container.Add(new PropertyField(property));

				return container;
			}

			var foldout = new Foldout {
				text = property.displayName
			};

			container.Add(foldout);

			// vízszintes sor az ObjectField + gomb számára
			var headerRow = new VisualElement {
				style = {
					flexDirection = FlexDirection.Row
				}
			};

			foldout.Add(headerRow);

			// ObjectField a ScriptableObject referenciahoz
			var objectField = new ObjectField {
				bindingPath = property.propertyPath,
				objectType = type,
				allowSceneObjects = false,
				style = {
					flexGrow = 1
				}
			};

			headerRow.Add(objectField);

			var button = CreateButton(property, type);
			button.style.width = 70;

			// Külön container a gyermek property-knek
			_childContainer = new VisualElement();
			foldout.Add(_childContainer);

			// első betöltésnél is lefusson
			if (property.objectReferenceValue != null) {
				RefreshChildProperties(_childContainer, (ScriptableObject)property.objectReferenceValue);
			} else {
				headerRow.Add(button);
			}

			// Gyermek propertyk megjelenítése
			objectField.RegisterValueChangedCallback(evt => {
					_childContainer.Clear();

					if (evt.newValue != null) {
						headerRow.Remove(button);
						RefreshChildProperties(_childContainer, (ScriptableObject)evt.newValue);
					} else {
						headerRow.Add(button);
					}
				}
			);

			return container;
		}

		private static void RefreshChildProperties(VisualElement container, ScriptableObject soTarget) {
			container.Clear();

			if (soTarget == null) {
				return;
			}

			var so = new SerializedObject(soTarget);
			var iterator = so.GetIterator();

			if (iterator.NextVisible(true)) {
				do {
					if (iterator.name == "m_Script") {
						continue;
					}

					var prop = so.FindProperty(iterator.name);

					if (prop != null) {
						var field = new PropertyField(prop);
						field.Bind(so);
						container.Add(field);
					}
				} while (iterator.NextVisible(false));
			}
		}

		private VisualElement CreateButton(SerializedProperty property, Type type) {
			var createButton = new Button(() => {
					var selectedAssetPath = "Assets";

					if (property.serializedObject.targetObject is MonoBehaviour behaviour) {
						var ms = MonoScript.FromMonoBehaviour(behaviour);
						selectedAssetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
					}

					var newAsset = CreateAssetWithSavePrompt(type, selectedAssetPath);

					if (newAsset != null) {
						property.objectReferenceValue = newAsset;
						property.serializedObject.ApplyModifiedProperties();
						RefreshChildProperties(_childContainer, newAsset);
					}
				}
			) {
				text = "Create"
			};

			return createButton;
		}

		// Creates a new ScriptableObject via the default Save File panel
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