using System.Collections.Generic;

public class AnimatorControllerMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEditor.Animations.AnimatorController);
    }

    protected override FinderToolBase GetToolPath()
    {
        return new AnimatorControllerPath();
    }

    protected override FinderToolBase GetToolObject()
    {
        return new AnimatorControllerObject();
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
            AssetType.Fbx,
        };
    }
}