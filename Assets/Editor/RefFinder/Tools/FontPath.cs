using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class FontPath : FinderToolBasePath
{
    protected override string GetSupportInfoExt()
    {
        string ext = "支持替换";
        if (string.IsNullOrEmpty(base.GetSupportInfoExt()))
        {
            return ext;
        }
        return string.Format("{0},{1}", base.GetSupportInfoExt(), ext);
    }

    protected override void WorkPath(Object findObject, string findPath, Object newObject)
    {
        string newObjectPath = "";
        string newFileID = "12800000";//看着两个字体是一样的
        string newObjectGuid = "";

        if (newObject != null)
        {
            newObjectPath = AssetDatabase.GetAssetPath(newObject);
            newObjectGuid = AssetDatabase.AssetPathToGUID(newObjectPath);
        }

        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string fileID = "12800000";
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        
        string findPathAbs = Application.dataPath + "/../" + findPath;
        bool hasDoReplace = false;
        
        DoWorkPath(findPathAbs, (filePath) =>
        {
            if (Regex.IsMatch(File.ReadAllText(filePath), @"m_Font: {fileID: " + fileID + ", guid: " + findObjectGuid + ", type: 3}"))
            {
                //要替换
                if (!string.IsNullOrEmpty(newObjectGuid)
                     && !string.IsNullOrEmpty(newFileID))
                {
                    string newFile = File.ReadAllText(filePath)
                        .Replace(@"m_Font: {fileID: " + fileID + ", guid: " + findObjectGuid + ", type: 3}", @"m_Font: {fileID: " + newFileID + ", guid: " + newObjectGuid + ", type: 3}");
                    File.WriteAllText(filePath, newFile);
                    hasDoReplace = true;
                }

                results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(filePath)));
            }
        },
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