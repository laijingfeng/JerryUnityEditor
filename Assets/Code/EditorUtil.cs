using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class EditorUtil
{
    public static string Vector3String(Vector3 v)
    {
        return string.Format("({0},{1},{2})", v.x, v.y, v.z);
    }

    public static string PathAbsolute2Assets(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    public static string GetHierarchyPath(Transform tf)
    {
        if (tf == null)
        {
            return string.Empty;
        }
        string path = tf.name;
        while (tf.parent != null)
        {
            tf = tf.parent;
            if (string.IsNullOrEmpty(path))
            {
                path = tf.name;
            }
            else
            {
                path = tf.name + "/" + path;
            }
        }
        return path;
    }

    public static List<string> GetFiles(List<string> paths, string searchPattern)
    {
        List<string> filePath = new List<string>();

        foreach (string path in paths)
        {
            //第一次传进来的路径可能直接是文件
            if (File.Exists(path))
            {
                if (Regex.IsMatch(path, searchPattern.Replace("*", ".*?")))
                {
                    filePath.Add(path);
                }
            }
            else
            {
                filePath.AddRange(Directory.GetFiles(path, searchPattern));

                foreach (string strDirectory in Directory.GetDirectories(path))
                {
                    filePath.AddRange(GetFiles(new List<string>() { strDirectory }, searchPattern));
                }
            }
        }

        return filePath;
    }
}