﻿using System.Collections.Generic;

public class MaterialMgr : FinderToolMgrBase
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEngine.Material);
    }

    protected override FinderToolBase GetToolPath()
    {
        return new MaterialPath();
    }

    protected override FinderToolBase GetToolObject()
    {
        return new MaterialObject();
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