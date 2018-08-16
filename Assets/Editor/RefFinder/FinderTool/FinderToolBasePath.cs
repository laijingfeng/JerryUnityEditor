using System.Reflection;
using UnityEditor;
using UnityEngine;

public abstract class FinderToolBasePath : FinderToolBase
{
    protected override string GetSupportInfoExt()
    {
        return "使用GUID匹配查找";
    }

    public override void Work(params object[] param)
    {
        results.Clear();
        if (param == null || param.Length != 3)
        {
            SetTip("内部参数错误", MessageType.Error);
            return;
        }
        WorkPath((Object)param[0], (string)param[1], (Object)param[2]);
    }

    protected abstract void WorkPath(Object findObject, string findPath, Object newObject);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">绝对路径</param>
    /// <returns></returns>
    protected bool IsMyCarrier(string path)
    {
        return IsMyCarrier(FinderToolMgrBase.Path2Type(path));
    }

    /// <summary>
    /// <para>获取对象的FileID</para>
    /// <para>Hierarchy的对象无效</para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected string GetFileID(Object obj)
    {
        PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
        SerializedObject srlzedObject = new SerializedObject(obj);
        inspectorModeInfo.SetValue(srlzedObject, InspectorMode.Debug, null);
        SerializedProperty localIdProp = srlzedObject.FindProperty("m_LocalIdentfierInFile");
        return localIdProp.intValue.ToString();
    }
}