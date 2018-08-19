using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimatorControllerObject : FinderToolBaseObject
{
    protected override string GetSupportInfoExt()
    {
        string ext = "查找组件:Animator";
        if (string.IsNullOrEmpty(base.GetSupportInfoExt()))
        {
            return ext;
        }
        return string.Format("{0},{1}", base.GetSupportInfoExt(), ext);
    }

    protected override void WorkObject(Object findObject, Object targetObject)
    {
        FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(targetObject);

        switch (type)
        {
            case FinderToolMgrBase.AssetType.GameObject:
            case FinderToolMgrBase.AssetType.Fbx:
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

        Animator[] compnents = targetGo.GetComponentsInChildren<Animator>(true);
        List<Object> ret = new List<Object>();

        foreach (Animator c in compnents)
        {
            if(c.runtimeAnimatorController == null)
            {
                continue;
            }
            if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(c.runtimeAnimatorController)).Equals(findObjectGuid))
            {
                ret.Add(c);
            }
        }
        return ret;
    }
}