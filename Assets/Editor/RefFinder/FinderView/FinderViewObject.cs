using UnityEditor;
using UnityEngine;

public class FinderViewObject : FinderViewBase
{
    private Object objectTarget = null;

    public FinderViewObject()
        : base()
    {
        fromType = FindFromType.FromObject;
        tip = "设定对象和目标，进行引用查找";
    }

    protected override void ChildDraw()
    {
        objectTarget = EditorGUILayout.ObjectField(new GUIContent("目标对象", "拖拽或选择目标对象到这里"), objectTarget, typeof(Object), true);
    }

    protected override bool Work()
    {
        if (base.Work() == false)
        {
            return false;
        }

        if (objectTarget == null)
        {
            SetTip("目标对象不能为空", MessageType.Warning);
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
                    finder.Work(findObject, objectTarget);
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