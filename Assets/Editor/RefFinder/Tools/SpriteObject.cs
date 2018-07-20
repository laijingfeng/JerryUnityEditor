using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpriteObject : FinderToolBaseObject
{
    protected override void WorkObject(Object findObject, Object targetObject)
    {
        FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(targetObject);
        if (!IsMyCarrier(type))
        {
            SetTip(string.Format("目标对象不是查找对象的载体({0})", MyCarrierListStr()), MessageType.Warning);
            return;
        }

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

        return ret;
    }
}