using UnityEditor;
using UnityEngine;

namespace YooTools {
	public class Group : EditorWindow {
		private static GameObject _groupParent;

		[MenuItem("GameObject/Group %g", false, -999)]
		private static void GroupMenu() => GroupSelectedTransforms();

		private static void GroupSelectedTransforms(string name = "Group", string tag = null, Vector3 position = default, bool calcCenter = true, bool bottomY = false, bool roundToInt = false) {
			var currentSelection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable);

			if (currentSelection.Length == 0) {
				return;
			}

			_groupParent = new GameObject(name);
			Undo.RegisterCreatedObjectUndo(_groupParent, "Created group"); // V 4.3+

			if (Selection.activeTransform != null) {
				_groupParent.transform.SetParent(Selection.activeTransform.parent);
				_groupParent.transform.SetSiblingIndex(Selection.activeTransform.GetSiblingIndex());

				if (Selection.activeTransform.parent != null && Selection.activeTransform.parent.GetComponent<RectTransform>() != null) {
					var rectTransform = Undo.AddComponent<RectTransform>(_groupParent);
					rectTransform.localScale = Vector3.one;
				}
			}

			if (calcCenter) {
				position = Vector3.zero;

				foreach (var groupMember in currentSelection) {
					position += groupMember.transform.position;
				}

				if (currentSelection.Length > 0) {
					position /= currentSelection.Length;
				}

				if (bottomY) {
					foreach (var groupMember in currentSelection) {
						if (groupMember.transform.position.y < position.y) {
							position.y = groupMember.transform.position.y;
						}

						if (groupMember.GetComponent<Collider>()) {
							if (groupMember.GetComponent<Collider>().bounds.min.y < position.y) {
								position.y = groupMember.GetComponent<Collider>().bounds.min.y;
							}
						}

						if (groupMember.GetComponent<Renderer>()) {
							if (groupMember.GetComponent<Renderer>().bounds.min.y < position.y) {
								position.y = groupMember.GetComponent<Renderer>().bounds.min.y;
							}
						}
					}
				}

				if (roundToInt) {
					position.x = Mathf.RoundToInt(position.x);
					position.y = Mathf.RoundToInt(position.y);
					position.z = Mathf.RoundToInt(position.z);
				}
			}

			_groupParent.transform.position = position;

			if (tag != null) {
				try {
					_groupParent.tag = tag;
				} catch {
					Debug.Log("The tag '" + tag + "' is not defined, please add the tag in the tag manager!");
				}
			}

			foreach (var groupMember in currentSelection) {
				Undo.SetTransformParent(groupMember.transform, _groupParent.transform, "Group");
			}

			var newSel = new GameObject[1];
			newSel[0] = _groupParent;
			Selection.objects = newSel;
		}

		[MenuItem("GameObject/Group %g", true, -999), MenuItem("GameObject/Group...", true, -998)]
		private static bool ValidateSelection() => Selection.activeTransform != null;

		public Vector3 parentPosition = Vector3.zero;
		public bool centerPosition = false;
		public bool centerBottomY = false;
		public string groupName = "Group";
		public string gTag;

		[MenuItem("GameObject/Group...", false, -998)]
		static public void Init() {
			var win = GetWindow(typeof(Group)) as Group;

			if (win != null) {
				win.autoRepaintOnSceneChange = true;
			}
		}

		private void OnGUI() {
			var helpString = "Groups the selected objects into an empty parent.\nYou can specify the position for the root object \nand it's name.\nYou can also select a tag for the group.\nCheck the center option to position the group\nat the center of the selected objects.";

			GUILayout.BeginArea(new Rect(20f, 20f, 280f, 400f));

			GUILayout.BeginVertical();
			GUILayout.Label(helpString);
			GUILayout.EndVertical();

			GUILayout.Space(20);

			GUILayout.BeginVertical();
			GUILayout.Label("Group Name: ");
			groupName = GUILayout.TextField(groupName);
			GUILayout.EndVertical();

			GUILayout.Space(20);

			GUILayout.BeginHorizontal();
			gTag = EditorGUILayout.TagField("Tag group with:", gTag);
			GUILayout.EndHorizontal();

			GUILayout.Space(20);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Position parent in center of selected objects: ");
			centerPosition = EditorGUILayout.Toggle(centerPosition);
			GUILayout.EndHorizontal();

			if (!centerPosition) {
				GUI.enabled = false;
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("... and at the bottom of the group (min Y): ");
			centerBottomY = EditorGUILayout.Toggle(centerBottomY);
			GUILayout.EndHorizontal();

			GUILayout.Space(20);

			GUI.enabled = true;

			GUILayout.BeginHorizontal();

			if (centerPosition) {
				GUI.enabled = false;
			}

			parentPosition = EditorGUILayout.Vector3Field("Parent Position:", parentPosition);
			GUILayout.EndHorizontal();

			GUILayout.Space(20);

			GUI.enabled = true;

			GUILayout.Space(20);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Close")) {
				Close();
			}

			if (GUILayout.Button("Group")) {
				GroupSelectedTransforms(groupName, gTag, parentPosition, centerPosition, centerBottomY);
			}

			if (GUILayout.Button("Group & Close")) {
				GroupSelectedTransforms(groupName, gTag, parentPosition, centerPosition, centerBottomY);
				Close();
			}

			GUILayout.EndHorizontal();

			GUILayout.EndArea();
		}

		[MenuItem("GameObject/Ungroup %#g", false, -997)]
		private static void UngroupMenu() {
			var currentSelection = Selection.GetTransforms(SelectionMode.Editable);
			Transform grandfatherObject;
			GameObject parentObject;

			if (currentSelection.Length == 0) {
				return;
			}

			foreach (var groupMember in currentSelection) {
				parentObject = groupMember.parent.gameObject;
				grandfatherObject = parentObject.transform.parent;
				Undo.SetTransformParent(groupMember.transform, grandfatherObject, "Ungroup");
				groupMember.transform.SetSiblingIndex(grandfatherObject.GetSiblingIndex());

				if (parentObject.GetComponents<Component>().Length < 2) {
					if (parentObject.transform.childCount == 0) {
						Debug.Log("Empty group object \"" + parentObject.name + "\" deleted!");
						Undo.DestroyObjectImmediate(parentObject);
					}
				}
			}
		}

		private static bool CheckGroupSelection() {
			if (Selection.activeTransform == null) {
				return false;
			}

			if (Selection.activeTransform.parent == null) {
				return false;
			}

			return true;
		}

		[MenuItem("GameObject/Ungroup %#g", true, -997)]
		private static bool ValidateGroupSelection() => CheckGroupSelection();
	}
}