using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Finder_Sprite : Finder_Base
{
    public override bool Match(System.Type type)
    {
        return type == typeof(UnityEngine.Sprite);
    }

    public override void PathFind(UnityEngine.Object findObject, string findPath)
    {
        pathFindResults.Clear();

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
            SetTip("查找异常", MessageType.Error, true);
            return;
        }

        string findPathAbs = Application.dataPath + "/../" + findPath;
        string[] files = Directory.GetFiles(findPathAbs, "*.*", SearchOption.AllDirectories)
            .Where(s => Path.GetExtension(s).ToLower().Equals(".prefab")).ToArray();

        if (files == null || files.Length <= 0)
        {
            SetTip("查找目录没有对象的载体(Prefab)", MessageType.Warning, true);
            return;
        }

        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        foreach (string f in files)
        {
            if (Regex.IsMatch(File.ReadAllText(f), @"m_Sprite: {fileID: " + spriteID + ", guid: " + findObjectGuid + ", type: 3}"))
            {
                pathFindResults.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(f)));
            }
        }

        SetTip(string.Format("查找结果如下({0}):", pathFindResults.Count), MessageType.Info, true);
    }

    public override void ObjectFind(Object findObject, Object objectTarget)
    {
        objectFindResults.Clear();

        if (objectTarget.GetType() != typeof(UnityEngine.GameObject))
        {
            SetTip("目标对象不是查找对象的载体(GameObject)", MessageType.Warning,false);
            return;
        }

        GameObject objectTargetGo = objectTarget as GameObject;
        Image[] imgs = objectTargetGo.GetComponentsInChildren<Image>(true);

        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        string spriteName = findObject.name;

        foreach (Image im in imgs)
        {
            if (im == null || im.sprite == null || im.sprite.name.Equals(spriteName) == false)
            {
                continue;
            }
            if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(im.sprite)).Equals(findObjectGuid))
            {
                objectFindResults.Add(im);
            }
        }

        SetTip(string.Format("查找结果如下({0}):", objectFindResults.Count), MessageType.Info, false);
    }
}