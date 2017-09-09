using System.Collections.Generic;

public class PrefabMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEngine.GameObject);
    }

    protected override FinderToolBase GetToolPath()
    {
        return new PrefabPath();
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