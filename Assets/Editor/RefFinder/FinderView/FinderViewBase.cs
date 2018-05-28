﻿using UnityEditor;
using UnityEngine;

public abstract class FinderViewBase
{
    /// <summary>
    /// 构造的时候要设置
    /// </summary>
    protected FindFromType fromType;
    public FindFromType FromType
    {
        get
        {
            return fromType;
        }
    }
    protected FinderToolBase finder = null;
    protected string tip = "";
    protected MessageType tipType = MessageType.Info;
    protected Object findObject = null;

    public FinderViewBase()
    {
        finder = null;
        tipType = MessageType.Info;
    }

    protected abstract void ChildDraw();

    public void Draw(Object findObj)
    {
        findObject = findObj;
        
        ChildDraw();

        if (GUILayout.Button("点击开始查找"))
        {
            Work();
        }

        if (GUILayout.Button("导出结果文件"))
        {
            OutputFindContent();
        }
   
        EditorGUILayout.BeginVertical();
        RefreshTip();
        EditorGUILayout.HelpBox(tip, tipType, true);
        EditorGUILayout.EndVertical();
        if (finder != null)
        {
            finder.Draw();
        }
    }

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

    private void OutputFindContent()
    {
        if (finder != null)
        {
            finder.OutputFindContent();
        }
    }

    private void RefreshTip()
    {
        if (finder != null)
        {
            tip = finder.tip;
            tipType = finder.tipType;
        }
    }

    protected void SetTip(string info, MessageType type)
    {
        tip = info;
        tipType = type;
    }

    public enum FindFromType
    {
        FromPath = 0,
        FromObject,
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