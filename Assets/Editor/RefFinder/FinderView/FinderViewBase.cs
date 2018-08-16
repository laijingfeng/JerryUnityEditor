using UnityEditor;
using UnityEngine;

public abstract class FinderViewBase
{
    /// <summary>
    /// 查找类型，构造的时候要设置
    /// </summary>
    protected FindFromType fromType;
    /// <summary>
    /// 查找类型，构造的时候要设置
    /// </summary>
    public FindFromType FromType
    {
        get
        {
            return fromType;
        }
    }
    /// <summary>
    /// 查找器
    /// </summary>
    protected FinderToolBase finder = null;
    /// <summary>
    /// 提示信息
    /// </summary>
    protected string tip = "";
    /// <summary>
    /// 提示信息类型
    /// </summary>
    protected MessageType tipType = MessageType.Info;
    /// <summary>
    /// 查找对象
    /// </summary>
    protected Object findObject = null;
    /// <summary>
    /// 支持信息
    /// </summary>
    protected string supportInfo = "这里是支持信息";

    public FinderViewBase()
    {
        finder = null;
        tipType = MessageType.Info;
    }

    /// <summary>
    /// 子类绘制
    /// </summary>
    protected abstract void ChildDraw();

    /// <summary>
    /// 绘制显示界面
    /// </summary>
    /// <param name="findObj">查找对象</param>
    public void Draw(Object findObj)
    {
        findObject = findObj;

        ChildDraw();

        GUILayout.Space(10);

        if (GUILayout.Button("刷新支持信息"))
        {
            RefreshSupportInfo();
        }
        EditorGUILayout.HelpBox(supportInfo, MessageType.Info, true);

        GUILayout.Space(10);

        GUI.color = Color.green;
        if (GUILayout.Button("点击开始查找"))
        {
            Work();
        }
        GUI.color = Color.white;

        if (GUILayout.Button("导出结果文件"))
        {
            OutputFindContent();
        }

        GUILayout.Space(10);

        RefreshTip();
        EditorGUILayout.HelpBox(tip, tipType, true);

        GUILayout.Space(10);

        if (finder != null)
        {
            finder.Draw();
        }
    }

    /// <summary>
    /// 刷新支持信息
    /// </summary>
    protected virtual void RefreshSupportInfo()
    {
        if (findObject == null)
        {
            supportInfo = "设置查找对象后才能获取支持信息";
            return;
        }

        supportInfo = string.Format("查找对象类型:{0}\n", FinderToolMgrBase.Object2TypeDes(findObject));

        bool match = false;//是否找到管理器
        foreach (FinderToolMgrBase mgr in RefFinder.finderMgrList)
        {
            if (mgr.Match(findObject.GetType()))
            {
                //不要覆盖属性变量，重新定义
                FinderToolBase finder = mgr.GetTool(fromType);
                match = true;
                if (finder != null)
                {
                    supportInfo += finder.GetSupportInfo();
                }
                else
                {
                    supportInfo += string.Format("查找对象的{0}查找方式还未实现", fromType);
                    return;
                }
                return;
            }
        }

        if (!match)
        {
            supportInfo += "该查找对象是不支持的类型";
        }
    }

    /// <summary>
    /// 点击开始查找
    /// </summary>
    /// <returns></returns>
    protected virtual bool Work()
    {
        finder = null;
        if (findObject == null)
        {
            SetTip("查找对象不能为空", MessageType.Warning);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 导出结果文件
    /// </summary>
    private void OutputFindContent()
    {
        if (finder != null)
        {
            finder.OutputFindContent();
        }
    }

    /// <summary>
    /// 从查找器刷新提示信息
    /// </summary>
    private void RefreshTip()
    {
        if (finder != null)
        {
            tip = finder.tip;
            tipType = finder.tipType;
        }
    }

    /// <summary>
    /// 设置提示信息
    /// </summary>
    /// <param name="info"></param>
    /// <param name="type"></param>
    protected void SetTip(string info, MessageType type)
    {
        tip = info;
        tipType = type;
    }

    /// <summary>
    /// 查找类型
    /// </summary>
    public enum FindFromType
    {
        /// <summary>
        /// 从指定路径
        /// </summary>
        FromPath = 0,
        /// <summary>
        /// 从特定对象
        /// </summary>
        FromObject,
        /// <summary>
        /// 从当前场景
        /// </summary>
        FromCurScene,
    }

    /// <summary>
    /// 查找方式->显示名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static public string FindFromType2ShowName(FindFromType type)
    {
        switch (type)
        {
            case FindFromType.FromPath:
                {
                    return "从指定路径";
                }
            case FindFromType.FromObject:
                {
                    return "从特定对象";
                }
            case FindFromType.FromCurScene:
                {
                    return "从当前场景";
                }
        }
        return "";
    }

    static public string FindFromType2Tip(FindFromType type)
    {
        switch (type)
        {
            case FindFromType.FromPath:
                {
                    return "在指定的目录中查找";
                }
            case FindFromType.FromObject:
                {
                    return "在指定的对象中查找";
                }
            case FindFromType.FromCurScene:
                {
                    return "在当前场景中查找";
                }
        }
        return "";
    }
}