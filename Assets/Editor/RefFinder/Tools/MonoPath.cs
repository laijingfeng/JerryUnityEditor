using UnityEditor;
using UnityEngine;

public class MonoPath : FinderToolBasePath
{
    protected override void WorkPath(Object findObject, string findPath)
    {
        SetTip("暂未实现", MessageType.Info);
    }
}