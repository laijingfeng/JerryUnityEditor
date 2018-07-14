using System.Collections.Generic;

public class MaterialMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEngine.Material);
    }

    protected override FinderToolBase GetToolPath()
    {
        return new Common_GuidPath();
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
            //AssetType.Fbx,//暂时屏蔽模型
        };
    }
}