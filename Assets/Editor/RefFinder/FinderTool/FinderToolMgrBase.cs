using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class FinderToolMgrBase
{
    protected Dictionary<FinderViewBase.FindFromType, FinderToolBase> dicTools = new Dictionary<FinderViewBase.FindFromType, FinderToolBase>();

    public abstract bool Match(System.Type type);
    protected abstract List<AssetType> MyCarrierList();

    public string MyCarrierListStr()
    {
        string ret = "";
        if (MyCarrierList() == null || MyCarrierList().Count <= 0)
        {
            return ret;
        }
        foreach (AssetType type in MyCarrierList())
        {
            if (!string.IsNullOrEmpty(ret))
            {
                ret += "|";
            }
            ret += type.ToString();
        }
        return ret;
    }

    public bool IsMyCarrier(FinderToolMgrBase.AssetType type)
    {
        if (MyCarrierList() == null || MyCarrierList().Count <= 0)
        {
            return false;
        }
        return MyCarrierList().Contains(type);
    }

    public FinderToolBase GetTool(FinderViewBase.FindFromType type)
    {
        FinderToolBase tool = null;
        if (dicTools.ContainsKey(type))
        {
            tool = dicTools[type];
        }
        if (tool == null)
        {
            switch (type)
            {
                case FinderViewBase.FindFromType.FromPath:
                    {
                        tool = GetToolPath();
                    }
                    break;
                case FinderViewBase.FindFromType.FromObject:
                    {
                        tool = GetToolObject();
                    }
                    break;
                case FinderViewBase.FindFromType.FromCurScene:
                    {
                        tool = GetToolCurScene();
                    }
                    break;
            }

            if (tool != null)
            {
                tool.mgr = this;

                if (!dicTools.ContainsKey(type))
                {
                    dicTools.Add(type, tool);
                }
                else
                {
                    dicTools[type] = tool;
                }
            }
        }
        return tool;
    }

    protected abstract FinderToolBase GetToolPath();
    protected abstract FinderToolBase GetToolObject();
    protected abstract FinderToolBase GetToolCurScene();

    public static AssetType Path2Type(string path)
    {
        AssetType type = AssetType.Unknow;
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            string extension = Path.GetExtension(path).ToLower();
            if (!string.IsNullOrEmpty(extension))
            {
                switch (extension)
                {
                    case ".prefab":
                        {
                            type = AssetType.GameObject;
                        }
                        break;
                    case ".unity":
                        {
                            type = AssetType.Scene;
                        }
                        break;
                    case ".mat":
                        {
                            type = AssetType.Material;
                        }
                        break;
                }
            }
        }
        return type;
    }

    public static AssetType Object2Type(Object obj)
    {
        AssetType type = AssetType.Unknow;
        if (obj != null)
        {
            if (obj.GetType() == typeof(UnityEngine.GameObject))
            {
                type = AssetType.GameObject;
            }
            else if (obj.GetType() == typeof(UnityEditor.SceneAsset))
            {
                type = AssetType.Scene;
            }
            else if (obj.GetType() == typeof(UnityEngine.Sprite))
            {
                type = AssetType.Sprite;
            }
            else if (obj.GetType() == typeof(UnityEngine.Material))
            {
                type = AssetType.Material;
            }
        }
        return type;
    }

    public enum AssetType
    {
        Unknow = 0,
        Scene,
        GameObject,
        Sprite,
        Material,
    }
}