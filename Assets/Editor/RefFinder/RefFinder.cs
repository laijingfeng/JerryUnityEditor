using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RefFinder : EditorWindow
{
    [MenuItem("JerryWins/RefFinder")]
    private static void Open()
    {
        GetWindow<RefFinder>();
    }

    private Object findObject = null;
    public const string OUTPUT_CONTENT_FILE = "RefFinderOutput.txt";

    private int toolbarIdx = 0;
    private List<FinderViewBase> finderViewList = new List<FinderViewBase>()
    {
        new FinderViewPath(),
        new FinderViewObject(),
        new FinderViewCurScene(),
    };
    public static List<FinderToolMgrBase> finderMgrList = new List<FinderToolMgrBase>()
    {
        new SpriteMgr(),
        new MonoMgr(),
        new TextureMgr(),
        new PrefabMgr(),
    };

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(300));
        findObject = EditorGUILayout.ObjectField(new GUIContent("查找对象", "拖拽或选择查找对象到这里"), findObject, typeof(Object), true);
        List<GUIContent> viewNames = new List<GUIContent>();
        foreach (FinderViewBase view in finderViewList)
        {
            viewNames.Add(new GUIContent(view.FromType.ToString(), FinderViewBase.FindFromType2Tip(view.FromType)));
        }
        toolbarIdx = GUILayout.Toolbar(toolbarIdx, viewNames.ToArray());
        foreach (FinderViewBase view in finderViewList)
        {
            if (view.FromType.GetHashCode() == toolbarIdx)
            {
                view.Draw(findObject);
                break;
            }
        }
        EditorGUILayout.EndVertical();

        //EditorGUILayout.BeginVertical("box");
        //if (GUILayout.Button(new GUIContent() { text = "Reset", tooltip = "如果异常了，可尝试重置" }))
        //{
        //    DoReset();
        //}
        //EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }
}