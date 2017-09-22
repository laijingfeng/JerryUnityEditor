using UnityEditor;
using UnityEngine;

public class FinderViewCurScene : FinderViewBase
{
    public FinderViewCurScene()
        : base()
    {
        fromType = FindFromType.FromCurScene;
        tip = "在当前场景显示对象中，进行引用查找";
    }

    protected override void ChildDraw()
    {
    }

    protected override bool Work()
    {
        if (base.Work() == false)
        {
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
                    finder.Work(findObject);
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
        }
        return true;
    }
}