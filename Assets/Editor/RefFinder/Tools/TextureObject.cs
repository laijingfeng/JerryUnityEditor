using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextureObject : FinderToolBaseObject
{
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
        string findObjecyName = findObject.name.ToString();
        RawImage[] rawImages = targetGo.GetComponentsInChildren<RawImage>(true);
        List<Object> ret = new List<Object>();

        foreach (RawImage img in rawImages)
        {
            if (img == null || img.texture == null || img.texture.name.Equals(findObjecyName) == false)
            {
                continue;
            }
            if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(img.texture)).Equals(findObjectGuid))
            {
                ret.Add(img);
            }
        }

        return ret;
    }
}