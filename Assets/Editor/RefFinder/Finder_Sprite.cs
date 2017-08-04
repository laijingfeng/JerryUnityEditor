using System.Collections.Generic;
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

    protected override List<AssetType> MyCarrierList()
    {
        return new List<AssetType>()
        {
            AssetType.Scene,
            AssetType.GameObject,
        };
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
            SetTip("查找异常", MessageType.Error, RefFinder.FindFromType.FromPath);
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
                    pathFindResults.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(f)));
                }
            }    
        }

        SetTip(string.Format("查找结果如下({0}):", pathFindResults.Count), MessageType.Info, RefFinder.FindFromType.FromPath);
    }

    public override void DrawPathFind()
    {
        if (pathFindResults.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            foreach (Object obj in pathFindResults)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
            EditorGUILayout.EndVertical();
        }
    }

    public override void ObjectFind(Object findObject, Object objectTarget)
    {
        objectFindResults.Clear();
        AssetType type = Object2Type(objectTarget);
        if (!IsMyCarrier(type))
        {
            SetTip(string.Format("目标对象不是查找对象的载体({0})", MyCarrierListStr()), MessageType.Warning, RefFinder.FindFromType.FromObject);
            return;
        }

        switch (type)
        {
            case AssetType.GameObject:
                {
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

                    SetTip(string.Format("查找结果如下({0}):", objectFindResults.Count), MessageType.Info, RefFinder.FindFromType.FromObject);
                }
                break;
            case AssetType.Scene:
                {
                    SetTip("Scene不支持查找详情，可以打开，再用FromCurScene查找", MessageType.Info, RefFinder.FindFromType.FromObject);
                }
                break;
        }
    }

    public override void DrawObjectFind()
    {
        if (objectFindResults.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            foreach (Object obj in objectFindResults)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
            EditorGUILayout.EndVertical();
        }
    }

    public override void CurSceneFind(Object findObject)
    {
        curSceneFindResults.Clear();
        
        GameObject[] gos = Editor.FindObjectsOfType<GameObject>();

        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        string spriteName = findObject.name;

        foreach (GameObject go in gos)
        {
            //只要跟结点
            if (go.transform.parent != null)
            {
                continue;
            }
            Image[] imgs = go.GetComponentsInChildren<Image>(true);

            foreach (Image im in imgs)
            {
                if (im == null || im.sprite == null || im.sprite.name.Equals(spriteName) == false)
                {
                    continue;
                }
                if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(im.sprite)).Equals(findObjectGuid))
                {
                    curSceneFindResults.Add(im);
                }
            }
        }

        SetTip(string.Format("查找结果如下({0}):", curSceneFindResults.Count), MessageType.Info, RefFinder.FindFromType.FromCurScene);
    }

    public override void DrawCurSceneFind()
    {
        if (curSceneFindResults.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            foreach (Object obj in curSceneFindResults)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
            EditorGUILayout.EndVertical();
        }
    }
}