using System.Collections.Generic;

public class MonoMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEditor.MonoScript);
    }

    protected override FinderToolBase GetToolPath()
    {
        return new GuidPath();
    }

    protected override FinderToolBase GetToolObject()
    {
        return new MonoObject();
    }

    protected override FinderToolBase GetToolCurScene()
    {
        return new MonoCurScene();
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