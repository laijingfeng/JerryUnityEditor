using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FontCurScene : FinderToolBaseCurScene
{
    protected override void WorkCurScene(UnityEngine.Object findObject)
    {
        List<GameObject> gos = SceneRootGameObjects();
        foreach (GameObject go in gos)
        {
            results.AddRange(FontObject.DoOneGameObject(findObject, go));
        }
        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }
}