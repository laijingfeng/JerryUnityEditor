using System.IO;
using UnityEditor;
using UnityEngine;

public class FinderViewPath : FinderViewBase
{
    /// <summary>
    /// 替换成新的对象
    /// </summary>
    protected Object newObject = null;

    /// <summary>
    /// 查找路径
    /// </summary>
    private string findPath = "Assets";
    /// <summary>
    /// 路径区域
    /// </summary>
    private Rect pathRect;

    public FinderViewPath()
        : base()
    {
        fromType = FindFromType.FromPath;
        tip = "设定对象和目录，进行引用查找，新的对象不为空的时候，将进行替换";
    }

    protected override void ChildDraw()
    {
        newObject = EditorGUILayout.ObjectField(new GUIContent("新的对象", "拖拽或选择新的对象到这里，用来替换查找对象"), newObject, typeof(Object), true);

        pathRect = EditorGUILayout.GetControlRect();
        findPath = EditorGUI.TextField(pathRect, new GUIContent("查找目录", "拖拽需要的目录到这里即可"), findPath);
        if ((Event.current.type == EventType.dragUpdated || Event.current.type == EventType.DragExited) &&
            pathRect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null
                && DragAndDrop.paths.Length > 0
                && Directory.Exists(DragAndDrop.paths[0]))
            {
                findPath = DragAndDrop.paths[0];
            }
        }
    }

    protected override bool Work()
    {
        if (base.Work() == false)
        {
            return false;
        }

        string findPathAbs = Application.dataPath + "/../" + findPath;
        if (Directory.Exists(findPathAbs) == false)
        {
            SetTip("查找目录不存在", MessageType.Warning);
            return false;
        }

        bool match = false;
        foreach (FinderToolMgrBase mgr in RefFinder.finderMgrList)
        {
            if (mgr.Match(findObject.GetType()))
            {
                finder = mgr.GetTool(fromType);
                match = true;
                if (finder != null)
                {
                    finder.Work(findObject, findPath, newObject);
                }
                else
                {
                    SetTip(string.Format("{0}的{1}查找方式还未实现", findObject.GetType(), fromType), MessageType.Warning);
                    return false;
                }
                break;
            }
        }

        if (!match)
        {
            SetTip(string.Format("查找对象{0}是不支持的类型", findObject.GetType()), MessageType.Warning);
            return false;
        }

        return true;
    }
}