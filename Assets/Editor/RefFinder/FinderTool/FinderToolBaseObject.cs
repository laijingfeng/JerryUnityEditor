using UnityEditor;
using UnityEngine;

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
        WorkObject((Object)param[0], (Object)param[1]);
    }

    protected abstract void WorkObject(Object findObject, Object objectTarget);

    protected bool IsMyCarrier(Object obj)
    {
        return IsMyCarrier(FinderToolMgrBase.Object2Type(obj));
    }
}