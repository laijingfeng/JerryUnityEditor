using UnityEditor;
using UnityEngine;

public class HierarchyNodeHelper : Editor
{
    [MenuItem("GameObject/Tr/PrintGoPath", false, 10)]
    public static void PrintGoPath()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Editable);
        if (selection == null || selection.Length <= 0)
        {
            return;
        }
        GameObject go = selection[0] as GameObject;
        Debug.LogWarning("find:" + EditorUtil.GetTransformHieraichyPath(go.transform), go);
    }
    
    [MenuItem("GameObject/Tr/PrintTransform", false, 11)]
    public static void PrintGoTransform()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Editable);
        if (selection == null || selection.Length <= 0)
        {
            return;
        }
        GameObject go = selection[0] as GameObject;
        Transform tf = go.transform;
        Debug.LogWarning("position:" + EditorUtil.Vector3String(tf.position));
        Debug.LogWarning("localPosition:" + EditorUtil.Vector3String(tf.localPosition));
        Debug.LogWarning("eulerAngles:" + EditorUtil.Vector3String(tf.rotation.eulerAngles));
        Debug.LogWarning("eulerAngles:" + EditorUtil.Vector3String(tf.localRotation.eulerAngles));
        Debug.LogWarning("localScale:" + EditorUtil.Vector3String(tf.localScale));
    }
}