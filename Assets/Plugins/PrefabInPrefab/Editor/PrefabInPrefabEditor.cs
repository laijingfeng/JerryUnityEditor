using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PrefabInPrefabAsset
{
    [CustomEditor(typeof(PrefabInPrefab))]
    public class PrefabInPrefabEditor : Editor
    {
        private SerializedProperty prefab;

        void OnEnable()
        {
            prefab = serializedObject.FindProperty("prefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            prefab.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab.objectReferenceValue, typeof(GameObject), false);
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                var targetComponent = target as PrefabInPrefab;
                targetComponent.ForceDrawDontEditablePrefab();
            }
        }
    }
}