using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonoCurScene : FinderToolBaseCurScene
{
    protected override void WorkCurScene(Object findObject)
    {
        List<GameObject> gos = SceneActiveRootGameObjects();
        foreach (GameObject go in gos)
        {
            results.AddRange(MonoObject.DoOneGameObject(findObject, go));
        }
        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }
}