using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteCurScene : FinderToolBaseCurScene
{
    protected override string GetSupportInfoExt()
    {
        string ext = SpriteObject.GetSupportComponentsInfo();
        if (string.IsNullOrEmpty(base.GetSupportInfoExt()))
        {
            return ext;
        }
        return string.Format("{0},{1}", base.GetSupportInfoExt(), ext);
    }

    protected override void WorkCurScene(Object findObject)
    {
        List<GameObject> gos = SceneRootGameObjects();

        foreach (GameObject go in gos)
        {
            results.AddRange(SpriteObject.DoOneGameObject(findObject, go));
        }

        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }
}