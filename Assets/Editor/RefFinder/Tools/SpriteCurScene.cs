using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCurScene : FinderToolBaseCurScene
{
    protected override void WorkCurScene(Object findObject)
    {
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        string spriteName = findObject.name;
        List<GameObject> gos = SceneActiveRootGameObjects();

        foreach (GameObject go in gos)
        {
            Image[] imgs = go.GetComponentsInChildren<Image>(true);

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
        }

        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }
}