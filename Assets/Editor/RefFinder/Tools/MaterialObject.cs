using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialObject : FinderToolBaseObject
{
    protected override string GetSupportInfoExt()
    {
        string ext = "查找组件:Renderer";
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
        
        Renderer[] rs = targetGo.GetComponentsInChildren<Renderer>(true);
        List<Object> ret = new List<Object>();
        
        foreach (Renderer r in rs)
        {
            foreach (Material mt in r.sharedMaterials)
            {
                if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mt)).Equals(findObjectGuid))
                {
                    ret.Add(r);
                    break;
                }
            }
        }
        return ret;
    }
}