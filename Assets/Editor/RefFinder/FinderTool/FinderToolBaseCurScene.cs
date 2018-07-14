using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class FinderToolBaseCurScene : FinderToolBase
{
    public override void Work(params object[] param)
    {
        results.Clear();
        if (param == null || param.Length != 1)
        {
            SetTip("内部参数错误", MessageType.Error);
            return;
        }
        WorkCurScene((Object)param[0]);
    }

    protected abstract void WorkCurScene(Object findObject);

    /// <summary>
    /// 当前场景的根对象
    /// </summary>
    /// <returns></returns>
    protected List<GameObject> SceneRootGameObjects()
    {
        UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
        return new List<GameObject>(s.GetRootGameObjects());
    }

    /// <summary>
    /// 当前场景激活的根对象
    /// </summary>
    /// <returns></returns>
    [System.Obsolete("请使用SceneRootGameObjects")]
    protected List<GameObject> SceneActiveRootGameObjects()
    {
        GameObject[] gos = Editor.FindObjectsOfType<GameObject>();
        List<GameObject> ret = new List<GameObject>();
        foreach (GameObject go in gos)
        {
            //只要根结点
            if (go.transform.parent != null)
            {
                continue;
            }
            ret.Add(go);
        }
        return ret;
    }
}