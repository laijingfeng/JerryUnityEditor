using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 对Prefab进行再次批量编辑
/// </summary>
public class CtrPrefabs : Editor 
{
    [MenuItem("Assets/DoSelectPrefabs", false)]
    public static void DoSelectPrefabs()
    {
        //========类型筛选
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), SelectionMode.Assets);

        if (selection != null && selection.Length >= 1)
        {
            foreach (UnityEngine.Object obj in selection)
            {
                DoSelectPrefabOne(obj);
            }
        }
    }

    /// <summary>
    /// 对一个Prefab处理
    /// </summary>
    /// <param name="obj"></param>
    private static void DoSelectPrefabOne(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            return;
        }
        string assetsPath = AssetDatabase.GetAssetPath(obj);

        //========路径筛选
        Debug.LogWarning("path:" + assetsPath);
        if (assetsPath.Contains("/TestCtrPrefabs/") == false)
        {
            return;
        }

        GameObject go = GetPrefabByPath(assetsPath);
        if (DoSelectGameObjectOne(go))
        {
            SavePrefab(go, assetsPath);
        }

        UnityEngine.Object.DestroyImmediate(go);
    }

    /// <summary>
    /// <para>对一个GameObject处理</para>
    /// <para>========具体操作</para>
    /// </summary>
    /// <param name="go"></param>
    private static bool DoSelectGameObjectOne(GameObject go)
    {
        if (go == null)
        {
            return false;
        }
        bool modify = false;

        //举例是删除Collider
        Collider[] meshCols = go.GetComponentsInChildren<Collider>(true);
        for (int i = 0, imax = meshCols.Length; i < imax; i++)
        {
            if (meshCols[i] != null)
            {
                modify = true;
                DestroyImmediate(meshCols[i]);
                meshCols[i] = null;
            }
        }
        
        return modify;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="go"></param>
    /// <param name="originAssetsPath"></param>
    private static void SavePrefab(GameObject go, string originAssetsPath)
    {
        if (go == null)
        {
            return;
        }

        string bundle = "";
        AssetImporter importer = AssetImporter.GetAtPath(originAssetsPath);
        if (importer != null)
        {
            bundle = importer.assetBundleName;
        }

        Object objPrefab = PrefabUtility.CreateEmptyPrefab(originAssetsPath);
        PrefabUtility.ReplacePrefab(go, objPrefab, ReplacePrefabOptions.ConnectToPrefab);

        importer = AssetImporter.GetAtPath(originAssetsPath);
        if (importer != null)
        {
            importer.assetBundleName = bundle;
        }

        Debug.Log(string.Format("save {0} success", Path.GetFileName(originAssetsPath)));
    }

    /// <summary>
    /// 通过路径获取Prefab
    /// </summary>
    /// <param name="assetsPath"></param>
    /// <returns></returns>
    private static GameObject GetPrefabByPath(string assetsPath)
    {
        Object obj = AssetDatabase.LoadMainAssetAtPath(assetsPath);
        GameObject go = null;
        if (obj == null)
        {
            return null;
        }

        go = UnityEngine.Object.Instantiate(obj) as GameObject;
        go.name = go.name.Replace("(Clone)", "");
        go.transform.position = Vector3.zero;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
        go.transform.localScale = Vector3.one;

        return go;
    }
}