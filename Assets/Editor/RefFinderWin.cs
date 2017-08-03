using UnityEditor;
using UnityEngine;

public class RefFinderWin : EditorWindow
{
    [MenuItem("Window/RefFinder")]
    private static void Open()
    {
        GetWindow<RefFinderWin>();
    }

    [MenuItem("Assets/FindRef", true)]
    private static bool FindValidate()
    {
        return Selection.objects.Length != 0;
    }

    [MenuItem("Assets/FindRef")]
    private static void Find()
    {
        //UnityEngine.Object[] selectedObjects = Selection.objects;
    }

    private string path = "Assets";
    private Rect PathRect;
    private Object findObject;

    void OnGUI()
    {
        EditorGUILayout.BeginVertical("box");
        PathRect = EditorGUILayout.GetControlRect();
        path = EditorGUI.TextField(PathRect, "查找目录", path);
        if ((Event.current.type == EventType.dragUpdated || Event.current.type == EventType.DragExited) &&
            PathRect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                path = DragAndDrop.paths[0];
            }
        }
        findObject = EditorGUILayout.ObjectField("查找对象", findObject, typeof(Object), true);
        if (GUILayout.Button("Find"))
        {
            Debug.LogWarning(" " + findObject.GetType());
        }
        EditorGUILayout.EndVertical();
    }
}