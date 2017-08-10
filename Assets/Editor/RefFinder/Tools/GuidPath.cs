using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 通用，GUID查找
/// </summary>
public class GuidPath : FinderToolBasePath
{
    protected override void WorkPath(Object findObject, string findPath)
    {
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findPathAbs = Application.dataPath + "/../" + findPath;
        string[] files = Directory.GetFiles(findPathAbs, "*.*", SearchOption.AllDirectories)
            .Where(s => IsMyCarrier(s)).ToArray();

        if (files != null && files.Length > 0)
        {
            int startIndex = 0;
            string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);

            EditorApplication.update = delegate()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
                if (Regex.IsMatch(File.ReadAllText(file), findObjectGuid))
                {
                    results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(file)));
                }
                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                }
            };
        }

        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }
}