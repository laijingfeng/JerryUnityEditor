using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonoObject : FinderToolBaseObject
{
    protected override void WorkObject(Object findObject, Object objectTarget)
    {
        FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(objectTarget);
        if (!IsMyCarrier(type))
        {
            SetTip(string.Format("目标对象不是查找对象的载体({0})", MyCarrierListStr()), MessageType.Warning);
            return;
        }

        switch (type)
        {
            case FinderToolMgrBase.AssetType.GameObject:
                {
                    results.AddRange(DoOneGameObject(findObject, objectTarget as GameObject));
                    SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
                }
                break;
            case FinderToolMgrBase.AssetType.Scene:
                {
                    SetTip("Scene不支持查找详情，可以打开，再用FromCurScene查找", MessageType.Info);
                }
                break;
        }
    }

    public static List<Object> DoOneGameObject(Object findObject, GameObject targetGo)
    {
        string findObjectType = findObject.name.ToString();
        Component[] coms = targetGo.GetComponentsInChildren<Component>(true);
        List<Object> ret = new List<Object>();

        foreach (Component com in coms)
        {
            if (com.GetType().ToString().Equals(findObjectType))
            {
                ret.Add(com);
            }
        }

        return ret;
    }
}