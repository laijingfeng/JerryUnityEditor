using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonoObject : FinderToolBaseObject
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