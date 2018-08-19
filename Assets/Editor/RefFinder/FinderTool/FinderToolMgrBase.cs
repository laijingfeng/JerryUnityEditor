using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

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

    /// <summary>
    /// 我的载体列表
    /// </summary>
    /// <returns></returns>
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
                ret += "\n";
            }
            ret += TypeDes(type);
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
    /// 对象转类型描述
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns></returns>
    public static string Object2TypeDes(Object obj)
    {
        AssetType type = FinderToolMgrBase.Object2Type(obj);
        if (type == AssetType.Unknow)
        {
            return string.Format("{0}[{1}]", obj.GetType().ToString(), "未知");
        }
        return TypeDes(type);
    }

    /// <summary>
    /// 类型的描述
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public static string TypeDes(AssetType type)
    {
        AssetTypeDes des = assetTypeList.Find((x) => x.type == type);
        if (des == null)
        {
            return "未知";
        }
        return string.Format("{0}[{1}/{2}]", des.sysType, des.chineseShortName, des.extension);
    }

    /// <summary>
    /// 路径(后缀)转类型
    /// </summary>
    /// <param name="path">绝对路径</param>
    /// <returns></returns>
    public static AssetType Path2Type(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return AssetType.Unknow;
        }
        string assetPath = EditorUtil.PathAbsolute2Assets(path);

        Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        if (obj == null)//Meta文件会为空
        {
            return AssetType.Unknow;
        }

        string extension = Path.GetExtension(assetPath);
        System.Type sysType = obj.GetType();

        List<AssetTypeDes> rets = assetTypeList.FindAll((x) => x.extension.Equals(extension));
        if (rets.Count > 0)
        {
            AssetTypeDes des = rets.Find((x) => x.sysType == sysType);
            if (des != null)
            {
                return des.type;
            }
            else
            {
                return rets[0].type;
            }
        }
        return AssetType.Unknow;
    }

    /// <summary>
    /// <para>资源转类型</para>
    /// <para>Hierarchy里拖拽的无法获取路径，可能不准确</para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static AssetType Object2Type(Object obj)
    {
        if (obj == null)
        {
            return AssetType.Unknow;
        }

        string extension = Path.GetExtension(AssetDatabase.GetAssetPath(obj));
        System.Type sysType = obj.GetType();

        List<AssetTypeDes> rets = assetTypeList.FindAll((x) => x.sysType == sysType);
        if (rets.Count > 0)
        {
            AssetTypeDes des = rets.Find((x) => x.extension.Equals(extension));
            if (des != null)
            {
                return des.type;
            }
            else
            {
                return rets[0].type;
            }
        }
        return AssetType.Unknow;
    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum AssetType
    {
        Unknow = 0,
        /// <summary>
        /// 预设(UnityEngine.GameObject/.prefab)
        /// </summary>
        GameObject,
        /// <summary>
        /// 模型(UnityEngine.GameObject/.fbx|.FBX)
        /// </summary>
        Fbx,
        /// <summary>
        /// 场景(UnityEditor.SceneAsset/.unity)
        /// </summary>
        Scene,
        /// <summary>
        /// 精灵(UnityEngine.Sprite/.png)
        /// </summary>
        Sprite,
        /// <summary>
        /// 贴图(UnityEngine.Texture2D/.png)
        /// </summary>
        Texture,
        /// <summary>
        /// 材质(UnityEngine.Material/.mat)
        /// </summary>
        Material,
        /// <summary>
        /// 脚本(UnityEditor.MonoScript/.cs)
        /// </summary>
        MonoScript,
        /// <summary>
        /// 字体(UnityEngine.Font/.TTF|.ttf)
        /// </summary>
        Font,
        /// <summary>
        /// 动画控制器(UnityEditor.Animations.AnimatorController/.controller)
        /// </summary>
        AnimatorController,
    }

    /// <summary>
    /// 资源类型列表
    /// </summary>
    public static List<AssetTypeDes> assetTypeList = new List<AssetTypeDes>()
    {
        new AssetTypeDes()
        {
            type = AssetType.GameObject,
            sysType = typeof(UnityEngine.GameObject),
            extension = ".prefab",
            chineseShortName = "预设"
        },
        new AssetTypeDes()
        {
            type = AssetType.Fbx,
            sysType = typeof(UnityEngine.GameObject),
            extension = ".fbx",
            chineseShortName = "模型"
        },
        new AssetTypeDes()
        {
            type = AssetType.Fbx,
            sysType = typeof(UnityEngine.GameObject),
            extension = ".FBX",
            chineseShortName = "模型"
        },
        new AssetTypeDes()
        {
            type = AssetType.Scene,
#if UNITY_5//_OR_NEWER //在Unity5.6.1f1中UNITY_5_OR_NEWER没有起作用 2018-07-14 17:22:13
            sysType = typeof(UnityEditor.SceneAsset),
#else
            sysType = typeof(UnityEngine.Object),
#endif
            extension = ".unity",
            chineseShortName = "场景"
        },
        new AssetTypeDes()
        {
            type = AssetType.Sprite,
            sysType = typeof(UnityEngine.Sprite),
            extension = ".png",
            chineseShortName = "精灵"
        },
        new AssetTypeDes()
        {
            type = AssetType.Texture,
            sysType = typeof(UnityEngine.Texture2D),
            extension=".png",
            chineseShortName="贴图"
        },
        new AssetTypeDes()
        {
            type = AssetType.Material,
            sysType = typeof(UnityEngine.Material),
            extension = ".mat",
            chineseShortName = "材质"
        },
        new AssetTypeDes()
        {
            type = AssetType.MonoScript,
            sysType = typeof(UnityEditor.MonoScript),
            extension = ".cs",
            chineseShortName = "脚本"
        },
        new AssetTypeDes()
        {
            type = AssetType.Font,
            sysType = typeof(UnityEngine.Font),
            extension = ".ttf",
            chineseShortName = "字体"
        },
        new AssetTypeDes()
        {
            type = AssetType.Font,
            sysType = typeof(UnityEngine.Font),
            extension = ".TTF",
            chineseShortName = "字体"
        },
        new AssetTypeDes()
        {
            type = AssetType.AnimatorController,
            sysType = typeof(UnityEditor.Animations.AnimatorController),
            extension = ".controller",
            chineseShortName = "动画控制器"
        },
    };

    /// <summary>
    /// 资源类型描述
    /// </summary>
    public class AssetTypeDes
    {
        /// <summary>
        /// 类型
        /// </summary>
        public AssetType type;
        /// <summary>
        /// 系统类型
        /// </summary>
        public System.Type sysType;
        /// <summary>
        /// 资源后缀
        /// </summary>
        public string extension;
        /// <summary>
        /// 中文简称
        /// </summary>
        public string chineseShortName;
    }
}