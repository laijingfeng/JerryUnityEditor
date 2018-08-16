using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RefFinder : EditorWindow
{
    [MenuItem("Tools/引用查找")]
    private static void Open()
    {
        GetWindow<RefFinder>("引用查找");
    }

    /// <summary>
    /// 查找对象
    /// </summary>
    private Object findObject = null;
    /// <summary>
    /// 输出文件名
    /// </summary>
    public const string OUTPUT_CONTENT_FILE = "RefFinderOutput.txt";

    /// <summary>
    /// 当前选中的查找显示界面
    /// </summary>
    private int toolbarIdx = 0;
    /// <summary>
    /// 查找显示界面列表
    /// </summary>
    private List<FinderViewBase> finderViewList = new List<FinderViewBase>()
    {
        new FinderViewPath(),
        new FinderViewObject(),
        new FinderViewCurScene(),
    };

    /// <summary>
    /// 查找管理器列表
    /// </summary>
    public static List<FinderToolMgrBase> finderMgrList = new List<FinderToolMgrBase>()
    {
        new SpriteMgr(),
        new MonoMgr(),
        new TextureMgr(),
        new PrefabMgr(),
        new MaterialMgr(),
        new FontMgr(),
        new ShaderMgr(),
    };

    void OnGUI()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(300));
        findObject = EditorGUILayout.ObjectField(new GUIContent("查找对象", "拖拽或选择查找对象到这里"), findObject, typeof(Object), true);
        List<GUIContent> viewNames = new List<GUIContent>();
        foreach (FinderViewBase view in finderViewList)
        {
            viewNames.Add(new GUIContent(FinderViewBase.FindFromType2ShowName(view.FromType), FinderViewBase.FindFromType2Tip(view.FromType)));
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
    }
}