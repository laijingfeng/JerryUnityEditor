using System.IO;
using System.Linq;
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

            Match mtNew = Regex.Match(File.ReadAllText(newObjectPath), @"--- !u!1 &(\d+)", RegexOptions.Singleline);
            if (mtNew != null && !string.IsNullOrEmpty(mtNew.Value))
            {
                newObjectFileId = mtNew.Value;
                newObjectFileId = newObjectFileId.Split('&')[1];
            }
        }

        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectFileId = "";
        Match mt = Regex.Match(File.ReadAllText(findObjectPath), @"--- !u!1 &(\d+)", RegexOptions.Singleline);
        if (mt != null && !string.IsNullOrEmpty(mt.Value))
        {
            findObjectFileId = mt.Value;
            findObjectFileId = findObjectFileId.Split('&')[1];
        }

        string findPathAbs = Application.dataPath + "/../" + findPath;
        string[] files = Directory.GetFiles(findPathAbs, "*.*", SearchOption.AllDirectories)
            .Where(s => IsMyCarrier(s)).ToArray();

        bool hasDoReplace = false;

        if (files != null && files.Length > 0)
        {
            int startIndex = 0;
            string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);

            EditorApplication.update = delegate()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
                if (Regex.IsMatch(File.ReadAllText(file), @"prefab: {fileID: " + findObjectFileId + ", guid: " + findObjectGuid + ", type: 2}"))
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
                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);

                    if (hasDoReplace)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            };
            SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
        }
    }
}