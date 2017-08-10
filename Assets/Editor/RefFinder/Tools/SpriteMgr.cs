using System.Collections.Generic;

public class SpriteMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEngine.Sprite);
    }

    protected override FinderToolBase GetToolPath()
    {
        return new SpritePath();
    }

    protected override FinderToolBase GetToolObject()
    {
        return new SpriteObject();
    }

    protected override FinderToolBase GetToolCurScene()
    {
        return new SpriteCurScene();
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