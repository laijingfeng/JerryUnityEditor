using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteCurScene : FinderToolBaseCurScene
{
    protected override string GetSupportInfoExt()
    {
        string ext = "检查组件:Image|SpriteRenderer。\n特别提醒自定义脚本里引用的无法查找";
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