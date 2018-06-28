using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// 场景中拖拽上去的Monster在打包的时候转换成配置脚本
/// </summary>
public class SceneHandle : Editor
{
    /// <summary>
    /// 怪物根节点名称
    /// </summary>
    private const string MonstersNodeName = "EditorMonsters";
    /// <summary>
    /// 配置根节点名称
    /// </summary>
    private const string ConfigsNodeName = "MonsterConfigs";
    /// <summary>
    /// 场景资源根目录
    /// </summary>
    private const string SceneAssetsRootPath = "/TestScene/";

    [MenuItem("Tools/场景怪物手动处理/怪物转配置", false, 0)]
    static public void HandleScenes()
    {
        UnityEngine.Debug.Log("【怪物转配置】");

        List<string> findPaths = FindScenesInPaths(new List<string>()
        {
            Application.dataPath + SceneAssetsRootPath,
        });
        if (findPaths.Count > 0)
        {
            int startIndex = 0;
            int total = findPaths.Count;
            EditorApplication.update = delegate ()
            {
                string file = findPaths[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("怪物转配置", file, (float)startIndex / (float)total);
                PlayerPrefs.SetInt("IS_NOW_HANDLE_SCENE", 1);
                HandleOneScene(file);
                PlayerPrefs.SetInt("IS_NOW_HANDLE_SCENE", 0);
                startIndex++;
                if (isCancel || startIndex >= total)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                }
            };
        }
    }

    [MenuItem("Tools/场景怪物手动处理/转回原场景", false, 1)]
    static public void HandleScenesBack()
    {
        UnityEngine.Debug.Log("【转回原场景】");

        List<string> findPaths = FindScenesInPaths(new List<string>()
        {
            Application.dataPath + SceneAssetsRootPath,
        });
        if (findPaths.Count > 0)
        {
            int startIndex = 0;
            int total = findPaths.Count;
            EditorApplication.update = delegate ()
            {
                string file = findPaths[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("转回原场景", file, (float)startIndex / (float)total);
                PlayerPrefs.SetInt("IS_NOW_HANDLE_SCENE", 1);
                HandleOneSceneBack(file);
                PlayerPrefs.SetInt("IS_NOW_HANDLE_SCENE", 0);
                startIndex++;
                if (isCancel || startIndex >= total)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                }
            };
        }
    }

    [MenuItem("Tools/场景怪物手动处理/单开场景标志归位", false, 2)]
    static public void FlagBack()
    {
        PlayerPrefs.SetInt("IS_NOW_HANDLE_SCENE", 0);
    }

    static private void HandleOneSceneBack(string assetsPath)
    {
        UnityEngine.SceneManagement.Scene s = EditorSceneManager.OpenScene(assetsPath);

        GameObject goFrom = null;
        GameObject goTo = null;
        GameObject[] gos = s.GetRootGameObjects();

        foreach (GameObject go in gos)
        {
            if (go.name == MonstersNodeName)
            {
                goFrom = go;
            }
            else if (go.name == ConfigsNodeName)
            {
                goTo = go;
            }
        }

        if (goFrom != null)
        {
            BackGo(goFrom, goTo);
            EditorSceneManager.SaveScene(s);
        }
    }

    static private void HandleOneScene(string assetsPath)
    {
        UnityEngine.SceneManagement.Scene s = EditorSceneManager.OpenScene(assetsPath);

        GameObject goFrom = null;
        GameObject goTo = null;
        GameObject[] gos = s.GetRootGameObjects();

        foreach (GameObject go in gos)
        {
            if (go.name == MonstersNodeName)
            {
                goFrom = go;
            }
            else if (go.name == ConfigsNodeName)
            {
                goTo = go;
            }
        }

        if (goFrom != null)
        {
            if (goTo != null)
            {
                GameObject.DestroyImmediate(goTo);
            }
            goTo = new GameObject(ConfigsNodeName);
            goTo.transform.localScale = Vector3.one;
            goTo.transform.localPosition = Vector3.zero;
            goTo.transform.localEulerAngles = Vector3.zero;
            TransferGo(goFrom, goTo);
            EditorSceneManager.SaveScene(s);
        }
    }

    private static void BackGo(GameObject goFrom, GameObject goTo)
    {
        if (goTo != null)
        {
            GameObject.DestroyImmediate(goTo);
        }
        goFrom.tag = "Untagged";
    }

    private static void TransferGo(GameObject goFrom, GameObject goTo)
    {
        goFrom.tag = "EditorOnly";

        for (int i = 0, imax = goFrom.transform.childCount; i < imax; i++)
        {
            Transform child = goFrom.transform.GetChild(i);
            if (child == null)
            {
                continue;
            }

            GameObject childConfig = new GameObject(child.name);
            childConfig.transform.parent = goTo.transform;
            childConfig.transform.localScale = Vector3.one;
            childConfig.transform.localPosition = Vector3.zero;
            childConfig.transform.localEulerAngles = Vector3.zero;

            //TODO:TestConfig替换成实际脚本
            //TestConfig test = childConfig.AddComponent<TestConfig>();
            //test.FillConfig(child.gameObject);//该函数用UNITY_EDITOR和Rabbit_Tools包起来
        }
    }

    static private List<string> FindScenesInPaths(List<string> paths)
    {
        List<string> ret = new List<string>();
        foreach (string path in paths)
        {
            if (!Directory.Exists(path))
            {
                Log("配置了一个不存在的目录 " + path);
                continue;
            }
            string[] files = Directory.GetFiles(path, "*.unity");
            if (files != null && files.Length > 0)
            {
                foreach (string file in files)
                {
                    //可以添加或改变条件
                    string fileName = Path.GetFileName(file);
                    if (fileName.StartsWith("HA"))
                    {
                        ret.Add(PathFullToAssets(file));
                    }
                }
            }

            foreach (string strDirectory in Directory.GetDirectories(path))
            {
                ret.AddRange(FindScenesInPaths(new List<string>() { strDirectory }));
            }
        }
        return ret;
    }

    static private string PathFullToAssets(string fullPath)
    {
        return fullPath.Substring(fullPath.LastIndexOf("Assets/"));
    }

    static private void Log(object msg)
    {
        UnityEngine.Debug.LogWarning(msg.ToString());
    }
}