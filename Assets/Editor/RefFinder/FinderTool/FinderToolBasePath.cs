using UnityEditor;
using UnityEngine;

public abstract class FinderToolBasePath : FinderToolBase
{
    public override void Work(params object[] param)
    {
        results.Clear();
        if (param == null || param.Length != 2)
        {
            SetTip("内部参数错误", MessageType.Error);
            return;
        }
        WorkPath((Object)param[0], (string)param[1]);
    }

    protected abstract void WorkPath(Object findObject, string findPath);

    protected bool IsMyCarrier(string path)
    {
        return IsMyCarrier(FinderToolMgrBase.Path2Type(path));
    }
}