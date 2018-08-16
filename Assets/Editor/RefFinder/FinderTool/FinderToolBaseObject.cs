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
        PreWorkObject((UnityEngine.Object)param[0], (UnityEngine.Object)param[1]);
    }

    /// <summary>
    /// 在特定对象查找_预处理
    /// </summary>
    /// <param name="findObject">查找对象</param>
    /// <param name="targetObject">目标对象</param>
    private void PreWorkObject(UnityEngine.Object findObject, UnityEngine.Object targetObject)
    {
        FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(targetObject);
        if (!IsMyCarrier(type))
        {
            SetTip(string.Format("目标对象({0})\n不是\n查找对象({1})\n的载体({2})", 
                findObject.GetType(), 
                targetObject.GetType(), 
                MyCarrierListStr()),
                MessageType.Warning);
            return;
        }
        WorkObject(findObject, targetObject);
    }

    /// <summary>
    /// <para>在特定对象查找</para>
    /// <para>已经检查过了目标对象是否符合载体</para>
    /// </summary>
    /// <param name="findObject">查找对象</param>
    /// <param name="targetObject">目标对象</param>
    protected abstract void WorkObject(UnityEngine.Object findObject, UnityEngine.Object targetObject);

    protected bool IsMyCarrier(UnityEngine.Object obj)
    {
        return IsMyCarrier(FinderToolMgrBase.Object2Type(obj));
    }

    /// <summary>
    /// 根据GUID判断findObject是否被targetObject引用
    /// </summary>
    /// <param name="findObject"></param>
    /// <param name="targetObject"></param>
    /// <returns></returns>
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