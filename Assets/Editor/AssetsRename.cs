using System.IO;
using UnityEditor;

/// <summary>
/// 重命名，2017-09-23-00
/// </summary>
public class AssetsRename : Editor
{
    [MenuItem("Assets/RenameSelectAssets", false)]
    public static void DoSelectPrefabs()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        if (selection != null && selection.Length >= 1)
        {
            foreach (UnityEngine.Object obj in selection)
            {
                DoSelectPrefabOne(obj);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
        string fileDir = Path.GetDirectoryName(assetsPath);//eg:Assets/xxx/xxx
        string fileName = Path.GetFileNameWithoutExtension(assetsPath);//eg:xxx
        string extension = Path.GetExtension(assetsPath);//eg:.prefab
        //UnityEngine.Debug.LogWarning(assetsPath + "\n" + fileDir + "\n" + fileName + "\n" + extension);
        //===========条件

        string newFileName = fileName;// + "_Test";

        AssetDatabase.RenameAsset(assetsPath, newFileName);
    }
}