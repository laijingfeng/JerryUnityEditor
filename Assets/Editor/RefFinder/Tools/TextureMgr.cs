using System.Collections.Generic;

public class TextureMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEngine.Texture2D);
    }

    protected override FinderToolBase GetToolPath()
    {
        return new Common_GuidPath();
    }

    protected override FinderToolBase GetToolObject()
    {
        return new TextureObject();
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
            AssetType.Material,
        };
    }
}