using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FontObject : FinderToolBaseObject
{
    protected override void WorkObject(UnityEngine.Object findObject, UnityEngine.Object targetObject)
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

    /// <summary>
    /// 在一个GameObject上查找字体
    /// </summary>
    /// <param name="findObject"></param>
    /// <param name="targetGo"></param>
    /// <returns></returns>
    public static List<UnityEngine.Object> DoOneGameObject(UnityEngine.Object findObject, GameObject targetGo)
    {
        string findObjectType = findObject.name.ToString();
        Text[] texts = targetGo.GetComponentsInChildren<Text>(true);
        List<UnityEngine.Object> ret = new List<UnityEngine.Object>();

        foreach (Text text in texts)
        {
            if (text == null)
            {
                continue;
            }
            if(text.font == findObject)
            {
                ret.Add(text);
            }
        }

        return ret;
    }
}
