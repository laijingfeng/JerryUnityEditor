using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

public abstract class FinderToolBaseObject : FinderToolBase
{
    public override void Work(params object[] param)
    {
        results.Clear();
        if (param == null || param.Length != 2)
        {
            SetTip("内部参数错误", MessageType.Error);
            return;
        }
        WorkObject((UnityEngine.Object)param[0], (UnityEngine.Object)param[1]);
    }

    protected abstract void WorkObject(UnityEngine.Object findObject, UnityEngine.Object targetObject);

    protected bool IsMyCarrier(UnityEngine.Object obj)
    {
        return IsMyCarrier(FinderToolMgrBase.Object2Type(obj));
    }

    protected List<UnityEngine.Object> DoOneObjectByGUID(UnityEngine.Object findObject, UnityEngine.Object targetObject)
    {
        List<UnityEngine.Object> ret = new List<UnityEngine.Object>();
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        string targetObjectPath = AssetDatabase.GetAssetPath(targetObject);
        if (Regex.IsMatch(File.ReadAllText(targetObjectPath), findObjectGuid))
        {
            ret.Add(targetObject);
        }
        return ret;
    }
}