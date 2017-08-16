using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextureObject : FinderToolBaseObject
{
    protected override void WorkObject(Object findObject, Object targetObject)
    {
        FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(targetObject);
        if (!IsMyCarrier(type))
        {
            SetTip(string.Format("目标对象不是查找对象的载体({0})", MyCarrierListStr()), MessageType.Warning);
            return;
        }

        results.AddRange(DoOneObjectGuid(findObject, targetObject));
        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }

    public static List<Object> DoOneObjectGuid(Object findObject, Object targetObject)
    {
        if (targetObject is GameObject)
        {
            return DoOneGameObject(findObject, targetObject as GameObject);
        }

        List<Object> ret = new List<Object>();
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        string targetObjectPath = AssetDatabase.GetAssetPath(targetObject);
        if (Regex.IsMatch(File.ReadAllText(targetObjectPath), findObjectGuid))
        {
            ret.Add(targetObject);
        }
        return ret;
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