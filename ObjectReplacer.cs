// Copyright (c) 2021 Hiroyuki Kako
// This software is released under the MIT License, see LICENSE.

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ObjectReplacer
{
    public class ObjectReplacer : EditorWindow
    {
        [MenuItem("KEditorExtensions/ObjectReplacer")]
        private static void Open()
        {
            GetWindow<ObjectReplacer>("Replace object in scene.");
        }

        static GameObject expectedObject;

        public GameObject[] targetObjectsInEditor;
        public GameObject expectedObjectInEditor;

        private static List<GameObject> _cachedObjects = new List<GameObject>();

        [MenuItem("GameObject/KEditorExtensions/ObjectReplacer/DO_REPLACE", false, 20)]
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
            expectedObjectInEditor =
                (GameObject) EditorGUILayout.ObjectField(expectedObjectInEditor, typeof(GameObject), true);
            expectedObject = expectedObjectInEditor;
            EditorGUILayout.EndHorizontal();

            ScriptableObject target = this;

            SerializedObject so = new SerializedObject(target);

            SerializedProperty stringsProperty = so.FindProperty("targetObjectsInEditor");

            EditorGUILayout.PropertyField(stringsProperty, true);

            so.ApplyModifiedProperties();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Right click on hierarchy and select DO REPLACE or push button.");
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("DO REPLACE"))
            {
                _cachedObjects.Clear();
                _cachedObjects = new List<GameObject>(targetObjectsInEditor);
                expectedObject = expectedObjectInEditor;
                ReplaceFunction();
                targetObjectsInEditor = new GameObject[0];
            }
        }

        private static void ReplaceFunction()
        {
            if (expectedObject != null)
            {
                foreach (var tmp in _cachedObjects)
                {
                    GameObject newObj =
                        PrefabUtility.InstantiatePrefab(expectedObject) as GameObject;
                    if (!(newObj is null))
                    {
                        newObj.transform.position = tmp.transform.position;
                        newObj.transform.eulerAngles = tmp.transform.eulerAngles;
                        newObj.transform.SetParent(tmp.transform.parent);
                    }

                    DestroyImmediate(tmp);
                }
            }
            else
            {
                Debug.Log("No target objects are set.");
            }
        }
    }
}
#endif
