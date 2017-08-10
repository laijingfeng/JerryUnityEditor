using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpriteObject : FinderToolBaseObject
{
    protected override void WorkObject(Object findObject, Object objectTarget)
    {
        FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(objectTarget);
        if (!IsMyCarrier(type))
        {
            SetTip(string.Format("目标对象不是查找对象的载体({0})", MyCarrierListStr()), MessageType.Warning);
            return;
        }

        switch (type)
        {
            case FinderToolMgrBase.AssetType.GameObject:
                {
                    GameObject objectTargetGo = objectTarget as GameObject;
                    Image[] imgs = objectTargetGo.GetComponentsInChildren<Image>(true);

                    string findObjectPath = AssetDatabase.GetAssetPath(findObject);
                    string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
                    string spriteName = findObject.name;

                    foreach (Image im in imgs)
                    {
                        if (im == null || im.sprite == null || im.sprite.name.Equals(spriteName) == false)
                        {
                            continue;
                        }
                        if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(im.sprite)).Equals(findObjectGuid))
                        {
                            results.Add(im);
                        }
                    }

                    SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
                }
                break;
            case FinderToolMgrBase.AssetType.Scene:
                {
                    SetTip("Scene不支持查找详情，可以打开，再用FromCurScene查找", MessageType.Info);
                }
                break;
        }
    }
}