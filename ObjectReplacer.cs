using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ObjectReplacer
{
    public class ObjectReplacer : EditorWindow
    {
        [MenuItem("ObjectReplacer/Window")]
        private static void Open()
        {
            GetWindow<ObjectReplacer>("Replace object in scene.");
        }


        public GameObject[] _targetObjectsInEditor;
        public GameObject _expectedObjectInEditor;

        private static GameObject _expectedObject;
        private static List<GameObject> _cachedObjects = new List<GameObject>();

        [MenuItem("GameObject/DO REPLACE", false, 20)]
        public static void DO_REPLACE()
        {
            var tmp = Selection.gameObjects;

            foreach (var obj in tmp)
            {
                if (!_cachedObjects.Contains(obj))
                {
                    _cachedObjects.Clear();
                    _cachedObjects = new List<GameObject>(tmp);

                    ReplaceFunction();

                    break;
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object of destinations to convert.");
            _expectedObjectInEditor =
                (GameObject) EditorGUILayout.ObjectField(_expectedObjectInEditor, typeof(GameObject), true);
            _expectedObject = _expectedObjectInEditor;
            EditorGUILayout.EndHorizontal();

            ScriptableObject target = this;

            SerializedObject so = new SerializedObject(target);

            SerializedProperty stringsProperty = so.FindProperty("_targetObjectsInEditor");

            EditorGUILayout.PropertyField(stringsProperty, true);

            so.ApplyModifiedProperties();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Right click on hierarchy and select DO REPLACE or d&d and push button.");
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("DO REPLACE"))
            {
                _cachedObjects.Clear();
                _cachedObjects = new List<GameObject>(_targetObjectsInEditor);
                _expectedObject = _expectedObjectInEditor;
                ReplaceFunction();
                _targetObjectsInEditor = new GameObject[0];
            }
        }

        private static void ReplaceFunction()
        {
            if (_expectedObject != null)
            {
                foreach (var cachedObject in _cachedObjects)
                {
                    GameObject newObj =
                        PrefabUtility.InstantiatePrefab(_expectedObject) as GameObject;
                    if (!(newObj is null))
                    {
                        newObj.transform.position = cachedObject.transform.position;
                        newObj.transform.eulerAngles = cachedObject.transform.eulerAngles;
                        newObj.transform.SetParent(cachedObject.transform.parent);
                    }

                    DestroyImmediate(cachedObject);
                }
            }
            else
            {
                Debug.Log("No target objects are set.");
            }
        }
    }
}