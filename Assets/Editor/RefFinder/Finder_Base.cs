using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class Finder_Base
{
    public abstract bool Match(System.Type type);
    
    #region PathFind

    public string pathFindTip = "设定对象和目录，进行引用查找";
    public MessageType pathFindTipType = MessageType.Info;
    protected List<Object> pathFindResults = new List<Object>();

    public abstract void PathFind(Object findObject, string findPath);
    public abstract void DrawPathFind();

    protected string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    #endregion PathFind

    #region ObjectFind

    public string objectFindTip = "设定对象和目标，进行引用查找";
    public MessageType objectFindTipType = MessageType.Info;
    protected List<Object> objectFindResults = new List<Object>();

    public abstract void ObjectFind(Object findObject, Object objectTarget);
    public abstract void DrawObjectFind();

    #endregion ObjectFind

    #region CurSceneFind

    public string curSceneFindTip = "在当前场景显示对象中，进行引用查找";
    public MessageType curSceneFindTipType = MessageType.Info;
    protected List<Object> curSceneFindResults = new List<Object>();

    public abstract void CurSceneFind(Object findObject);
    public abstract void DrawCurSceneFind();

    #endregion CurSceneFind

    protected void SetTip(string info, MessageType type, RefFinder.FindFromType fromType)
    {
        switch (fromType)
        {
            case RefFinder.FindFromType.FromPath:
                {
                    pathFindTip = info;
                    pathFindTipType = type;
                }
                break;
            case RefFinder.FindFromType.FromObject:
                {
                    objectFindTip = info;
                    objectFindTipType = type;
                }
                break;
            case RefFinder.FindFromType.FromCurScene:
                {
                    curSceneFindTip = info;
                    curSceneFindTipType = type;
                }
                break;
        }
    }

    protected virtual List<AssetType> MyCarrierList()
    {
        return new List<AssetType>();
    }

    protected string MyCarrierListStr()
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

    protected bool IsMyCarrier(Object obj)
    {
        return IsMyCarrier(Object2Type(obj));
    }

    protected bool IsMyCarrier(string path)
    {
        return IsMyCarrier(Path2Type(path));
    }

    protected bool IsMyCarrier(AssetType type)
    {
        if (MyCarrierList() == null || MyCarrierList().Count <= 0)
        {
            return false;
        }
        return MyCarrierList().Contains(type);
    }

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
        }
        return type;
    }

    public enum AssetType
    {
        Unknow = 0,
        Scene,
        GameObject,
        Sprite,
    }
}