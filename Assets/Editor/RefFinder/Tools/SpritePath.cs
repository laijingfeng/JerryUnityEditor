using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class SpritePath : FinderToolBasePath
{
    protected override void WorkPath(Object findObject, string findPath)
    {
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string spriteName = findObject.name;
        string spriteID = "";

        Match mt = Regex.Match(File.ReadAllText(findObjectPath + ".meta"), @"(\d+): " + spriteName, RegexOptions.Singleline);
        if (mt != null)
        {
            spriteID = mt.Value;
            spriteID = spriteID.Split(':')[0];
        }
        else
        {
            SetTip("查找异常", MessageType.Error);
            return;
        }

        string findPathAbs = Application.dataPath + "/../" + findPath;
        string[] files = Directory.GetFiles(findPathAbs, "*.*", SearchOption.AllDirectories)
            .Where(s => IsMyCarrier(s)).ToArray();

        if (files != null && files.Length > 0)
        {
            string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
            foreach (string f in files)
            {
                if (Regex.IsMatch(File.ReadAllText(f), @"m_Sprite: {fileID: " + spriteID + ", guid: " + findObjectGuid + ", type: 3}"))
                {
                    results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(f)));
                }
            }
        }

        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }
}