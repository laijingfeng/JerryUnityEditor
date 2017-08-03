using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class Finder_Base
{
    public abstract bool Match(System.Type type);
    public abstract void PathFind(Object findObject, string findPath);
    public abstract void ObjectFind(Object findObject, Object objectTarget);

    public string pathFindTip = "设定对象和目录，进行引用查找";
    public MessageType pathFindTipType = MessageType.Info;
    public List<Object> pathFindResults = new List<Object>();

    public string objectFindTip = "设定对象和目标，进行引用查找";
    public MessageType objectFindTipType = MessageType.Info;
    public List<Object> objectFindResults = new List<Object>();

    protected string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    protected void SetTip(string info, MessageType type, bool isPathFind)
    {
        if (isPathFind)
        {
            pathFindTip = info;
            pathFindTipType = type;
        }
        else
        {
            objectFindTip = info;
            objectFindTipType = type;
        }
    }
}