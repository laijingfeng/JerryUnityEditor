using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class PrefabPath : FinderToolBasePath
{
    protected override void WorkPath(Object findObject, string findPath, Object newObject)
    {
        string newObjectPath = "";
        string newObjectGuid = "";
        string newObjectFileId = "";

        if (newObject != null)
        {
            newObjectPath = AssetDatabase.GetAssetPath(newObject);
            newObjectGuid = AssetDatabase.AssetPathToGUID(newObjectPath);
            newObjectFileId = GetFileID(newObject);
        }

        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectFileId = GetFileID(findObject);

        string findPathAbs = Application.dataPath + "/../" + findPath;
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);

        bool hasDoReplace = false;

        DoWorkPath(findPathAbs, (file) =>
        {
            //查找无需太严，要支持Hierarchy
            if (Regex.IsMatch(File.ReadAllText(file), @", guid: " + findObjectGuid + ", type: 2}"))
            {
                //要替换
                if (!string.IsNullOrEmpty(newObjectGuid))
                {
                    string newFile = File.ReadAllText(file)
                        .Replace(@"prefab: {fileID: " + findObjectFileId + ", guid: " + findObjectGuid + ", type: 2}", @"prefab: {fileID: " + newObjectFileId + ", guid: " + newObjectGuid + ", type: 2}");
                    File.WriteAllText(file, newFile);
                    hasDoReplace = true;
                }

                results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(file)));
            }
        }
        ,
        () =>
        {
            if (hasDoReplace)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        });
    }
}