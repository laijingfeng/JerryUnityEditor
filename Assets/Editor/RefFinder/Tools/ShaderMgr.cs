using System.Collections.Generic;

public class ShaderMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEngine.Shader);
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
            //AssetType.Scene,//场景只会有材质的引用，不会有Shader
            //AssetType.GameObject,//Prefab只会有材质的引用，不会有Shader
            AssetType.Material,
            //AssetType.Fbx,//模型不是文本保存会报错，暂时屏蔽
        };
    }
}