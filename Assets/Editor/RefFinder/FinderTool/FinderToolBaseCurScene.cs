using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 从当前场景查找
/// </summary>
public abstract class FinderToolBaseCurScene : FinderToolBase
{
    protected override string GetSupportInfoExt()
    {
        string ext = "将查找场景里的每一个GameObject";
        if (string.IsNullOrEmpty(base.GetSupportInfoExt()))
        {
            return ext;
        }
        return string.Format("{0},{1}", base.GetSupportInfoExt(), ext);
    }

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
    /// 当前打开场景的根对象，支持同时打开多个场景
    /// </summary>
    /// <returns></returns>
    protected List<GameObject> SceneRootGameObjects()
    {
        List<GameObject> ret = new List<GameObject>();
        for (int i = 0, imax = SceneManager.sceneCount; i < imax; i++)
        {
            UnityEngine.SceneManagement.Scene s = SceneManager.GetSceneAt(i);
            ret.AddRange(s.GetRootGameObjects());
        }
        return ret;
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