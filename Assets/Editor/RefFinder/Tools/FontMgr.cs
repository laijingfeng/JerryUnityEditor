using System;
using System.Collections.Generic;

public class FontMgr : FinderToolMgrBase
{
    /// <summary>
    /// 资源是否匹配
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public override bool Match(Type type)
    {
        return type == typeof(UnityEngine.Font);
    }

    protected override FinderToolBase GetToolPath()
    {
        return new Common_GuidPath();
    }

    protected override FinderToolBase GetToolObject()
    {
        return new FontObject();
    }

    protected override FinderToolBase GetToolCurScene()
    {
        return new FontCurScene();
    }

    /// <summary>
    /// 资源可能的载体
    /// </summary>
    /// <returns></returns>
    protected override List<AssetType> MyCarrierList()
    {
        return new List<AssetType>()
        {
            AssetType.Scene,
            AssetType.GameObject,
        };
    }
}