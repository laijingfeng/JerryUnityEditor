using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class SpritePath : FinderToolBasePath
{
    protected override void WorkPath(Object findObject, string findPath, Object newObject)
    {
        string newObjectPath = "";
        string newSpriteName = "";
        string newSpriteID = "";
        string newObjectGuid = "";

        if (newObject != null)
        {
            newObjectPath = AssetDatabase.GetAssetPath(newObject);
            newSpriteName = newObject.name;
            newSpriteID = "21300000";//单张图的时候，id是这个，但是没有记录
            newObjectGuid = AssetDatabase.AssetPathToGUID(newObjectPath);

            Match mtNew = Regex.Match(File.ReadAllText(newObjectPath + ".meta"), @"(\d+): " + newSpriteName, RegexOptions.Singleline);
            if (mtNew != null && !string.IsNullOrEmpty(mtNew.Value))
            {
                newSpriteID = mtNew.Value;
                newSpriteID = newSpriteID.Split(':')[0];
            }
        }

        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string spriteName = findObject.name;
        string spriteID = "21300000";//单张图的时候，id是这个，但是没有记录

        Match mt = Regex.Match(File.ReadAllText(findObjectPath + ".meta"), @"(\d+): " + spriteName, RegexOptions.Singleline);
        if (mt != null && !string.IsNullOrEmpty(mt.Value))
        {
            spriteID = mt.Value;
            spriteID = spriteID.Split(':')[0];
        }

        string findPathAbs = Application.dataPath + "/../" + findPath;
        bool hasDoReplace = false;
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);

        DoWorkPath(findPathAbs, (filePath) =>
        {
            if (Regex.IsMatch(File.ReadAllText(filePath), @"m_Sprite: {fileID: " + spriteID + ", guid: " + findObjectGuid + ", type: 3}"))
            {
                //要替换
                if (!string.IsNullOrEmpty(newObjectGuid)
                     && !string.IsNullOrEmpty(newSpriteID))
                {
                    string newFile = File.ReadAllText(filePath)
                        .Replace(@"m_Sprite: {fileID: " + spriteID + ", guid: " + findObjectGuid + ", type: 3}", @"m_Sprite: {fileID: " + newSpriteID + ", guid: " + newObjectGuid + ", type: 3}");
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