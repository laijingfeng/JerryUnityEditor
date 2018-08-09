using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class FinderToolMgrBase
{
    protected Dictionary<FinderViewBase.FindFromType, FinderToolBase> dicTools = new Dictionary<FinderViewBase.FindFromType, FinderToolBase>();

    /// <summary>
    /// 资源是否匹配
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public abstract bool Match(System.Type type);

    /// <summary>
    /// 资源可能的载体
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 路径(后缀)转类型
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
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
                    case ".fbx":
                        {
                            type = AssetType.Fbx;
                        }
                        break;
                }
            }
        }
        return type;
    }

    /// <summary>
    /// 可能不准确，UnityEngine.GameObject可能是预设或模型
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static AssetType Object2Type(Object obj)
    {
        AssetType type = AssetType.Unknow;
        if (obj != null)
        {
            if (obj.GetType() == typeof(UnityEngine.GameObject))
            {
                type = AssetType.GameObject;
            }
#if UNITY_5//_OR_NEWER //在Unity5.6.1f1中UNITY_5_OR_NEWER没有起作用 2018-07-14 17:22:13
            else if (obj.GetType() == typeof(UnityEditor.SceneAsset))
            {
                type = AssetType.Scene;
            }
#else
            else if (obj.GetType() == typeof(UnityEngine.Object))
            {
                type = AssetType.Scene;
            }
#endif
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

    /// <summary>
    /// 载体资源类型
    /// </summary>
    public enum AssetType
    {
        Unknow = 0,
        /// <summary>
        /// 场景
        /// </summary>
        Scene,
        /// <summary>
        /// 预设
        /// </summary>
        GameObject,
        Sprite,
        /// <summary>
        /// 材质
        /// </summary>
        Material,
        /// <summary>
        /// 模型
        /// </summary>
        Fbx,
    }
}