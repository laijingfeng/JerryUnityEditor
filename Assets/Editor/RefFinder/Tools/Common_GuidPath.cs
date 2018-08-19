using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 通用，GUID查找
/// </summary>
public class Common_GuidPath : FinderToolBasePath
{
    protected override void WorkPath(Object findObject, string findPath, Object newObject)
    {
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findPathAbs = Application.dataPath + "/../" + findPath;
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);

        DoWorkPath(findPathAbs, (filePath) =>
        {
            if (Regex.IsMatch(File.ReadAllText(filePath), findObjectGuid))
            {
                results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(filePath)));
            }
        });
    }
}