using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public partial class SpriteObject : FinderToolBaseObject
{
    protected override string GetSupportInfoExt()
    {
        string ext = "目标对象是场景时，预设里引用的将无法查找，建议用从当前场景查找"
            + GetSupportComponentsInfo();
        if (string.IsNullOrEmpty(base.GetSupportInfoExt()))
        {
            return ext;
        }
        return string.Format("{0},{1}", base.GetSupportInfoExt(), ext);
    }

    public static string GetSupportComponentsInfo()
    {
        List<string> des = new List<string>()
        {
            "Image",
            "SpriteRenderer",
        };
        DoMoreComponentsDes(ref des);
        string ret = "";
        foreach (string r in des)
        {
            if (!string.IsNullOrEmpty(ret))
            {
                ret += "|";
            }
            ret += r;
        }
        ret = string.Format("\n检查组件:{0}。\n提醒自定义脚本可在SpriteObjectExt实现", ret);
        return ret;
    }

    protected override void WorkObject(Object findObject, Object targetObject)
    {
        FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(targetObject);

        switch (type)
        {
            case FinderToolMgrBase.AssetType.GameObject:
                {
                    results.AddRange(DoOneGameObject(findObject, targetObject as GameObject));
                }
                break;
            default:
                {
                    results.AddRange(DoOneObjectByGUID(findObject, targetObject));
                }
                break;
        }
        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }

    public static List<Object> DoOneGameObject(Object findObject, GameObject targetGo)
    {
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        string spriteName = findObject.name;
        Image[] imgs = targetGo.GetComponentsInChildren<Image>(true);
        SpriteRenderer[] srs = targetGo.GetComponentsInChildren<SpriteRenderer>(true);
        List<Object> ret = new List<Object>();

        foreach (Image im in imgs)
        {
            if (im == null || im.sprite == null || im.sprite.name.Equals(spriteName) == false)
            {
                continue;
            }
            if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(im.sprite)).Equals(findObjectGuid))
            {
                ret.Add(im);
            }
        }

        foreach (SpriteRenderer sr in srs)
        {
            if (sr == null || sr.sprite == null || sr.sprite.name.Equals(spriteName) == false)
            {
                continue;
            }
            if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sr.sprite)).Equals(findObjectGuid))
            {
                ret.Add(sr);
            }
        }

        DoMoreComponents(findObject, targetGo, ref ret);

        return ret;
    }

    /// <summary>
    /// 扩展脚本
    /// </summary>
    /// <param name="findObject"></param>
    /// <param name="targetGo"></param>
    /// <param name="ret"></param>
    static partial void DoMoreComponents(Object findObject, GameObject targetGo, ref List<Object> ret);

    /// <summary>
    /// 扩展脚本描述
    /// </summary>
    /// <param name="des"></param>
    static partial void DoMoreComponentsDes(ref List<string> des);
}