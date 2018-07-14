using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonoObject : FinderToolBaseObject
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

    /// <summary>
    /// 在一个GameObject上查找脚本
    /// </summary>
    /// <param name="findObject"></param>
    /// <param name="targetGo"></param>
    /// <returns></returns>
    public static List<Object> DoOneGameObject(Object findObject, GameObject targetGo)
    {
        MissingMonoCheck(targetGo);

        string findObjectType = findObject.name.ToString();
        Component[] coms = targetGo.GetComponentsInChildren<Component>(true);
        List<Object> ret = new List<Object>();

        foreach (Component com in coms)
        {
            //空引用
            if (com == null)
            {
                continue;
            }
            if (com.GetType().ToString().Equals(findObjectType))
            {
                ret.Add(com);
            }
        }

        return ret;
    }

    /// <summary>
    /// 查找丢失或者无法加载的脚本
    /// </summary>
    static private void MissingMonoCheck(GameObject targetGo)
    {
        if(targetGo == null)
        {
            return;
        }

        Component[] coms = targetGo.GetComponents<Component>();
        foreach (Component com in coms)
        {
            if (com == null)
            {
                UnityEngine.Debug.LogError("发现一个丢失或者无法加载的脚本", targetGo);
            }
        }

        for (int i = 0, imax = targetGo.transform.childCount; i < imax; i++)
        {
            Transform child = targetGo.transform.GetChild(i);
            if (child != null)
            {
                MissingMonoCheck(child.gameObject);
            }
        }
    }
}