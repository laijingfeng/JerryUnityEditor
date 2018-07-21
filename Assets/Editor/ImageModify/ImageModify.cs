using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ImageModify : EditorWindow
{
    [MenuItem("JerryWins/ImageModify")]
    private static void Open()
    {
        GetWindow<ImageModify>("图片修改");
    }

    private Texture2D workObject = null;
    private int toolbarIdx = 0;
    private List<string> workerViewTypeList = new List<string>()
    {
        "增",
        "删",
        "改",
    };
    public static List<ImageModifyViewBase> workerViewList = new List<ImageModifyViewBase>()
    {
        new ImageModifyViewAdd(),
        new ImageModifyViewDelete(),
        new ImageModifyViewModify(),
    };

    void OnGUI()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(300));
        workObject = EditorGUILayout.ObjectField(new GUIContent("修改对象", "拖拽或选择修改图像对象到这里"), workObject, typeof(Texture2D), true) as Texture2D;
        List<GUIContent> viewNames = new List<GUIContent>();
        foreach (string worker in workerViewTypeList)
        {
            viewNames.Add(new GUIContent(worker, worker));
        }
        toolbarIdx = GUILayout.Toolbar(toolbarIdx, viewNames.ToArray());
        workerViewList[toolbarIdx].Draw(AssetDatabase.GetAssetPath(workObject));
        EditorGUILayout.EndVertical();
    }
}