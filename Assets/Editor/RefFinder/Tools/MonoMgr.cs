using System.Collections.Generic;

public class MonoMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEditor.MonoScript);
    }

    protected override FinderToolBase GetToolPath()
    {
        return null;
    }

    protected override FinderToolBase GetToolObject()
    {
        return null;
    }

    protected override FinderToolBase GetToolCurScene()
    {
        return null;
    }

    protected override List<AssetType> MyCarrierList()
    {
        return new List<AssetType>()
        {
            AssetType.Scene,
            AssetType.GameObject,
        };
    }
}