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
        string spriteID = "21300000";//单张图的时候，id是这个，但是没有记录

        Match mt = Regex.Match(File.ReadAllText(findObjectPath + ".meta"), @"(\d+): " + spriteName, RegexOptions.Singleline);
        if (mt != null && !string.IsNullOrEmpty(mt.Value))
        {
            spriteID = mt.Value;
            spriteID = spriteID.Split(':')[0];
        }

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
                if (Regex.IsMatch(File.ReadAllText(file), @"m_Sprite: {fileID: " + spriteID + ", guid: " + findObjectGuid + ", type: 3}"))
                {
                    results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(file)));
                }
                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
                }
            };
            SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
        }
    }
}