using System.IO;
using UnityEditor;
using UnityEngine;

public class FinderViewPath : FinderViewBase
{
    private string findPath = "Assets";
    private Rect pathRect;

    public FinderViewPath()
        : base()
    {
        fromType = FindFromType.FromPath;
        tip = "设定对象和目录，进行引用查找";
    }

    protected override void ChildDraw()
    {
        pathRect = EditorGUILayout.GetControlRect();
        findPath = EditorGUI.TextField(pathRect, "查找目录", findPath);
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
                    finder.Work(findObject, findPath);
                }
                else
                {
                    SetTip(string.Format("{0}查找方式还未实现", fromType), MessageType.Warning);
                    return false;
                }
                break;
            }
        }

        if (!match)
        {
            SetTip(string.Format("查找对象{0}是不支持的类型", findObject.GetType()), MessageType.Warning);
        }

        return true;
    }
}