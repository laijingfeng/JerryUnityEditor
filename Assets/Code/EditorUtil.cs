using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

//version: 2018-08-13 13:41:57

public class EditorUtil
{
    public static string Vector3String(Vector3 v)
    {
        return string.Format("({0},{1},{2})", v.x, v.y, v.z);
    }

    public static string PathAbsolute2Assets(string absPath)
    {
        return "Assets" + Path.GetFullPath(absPath).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    public static string PathAssets2Absolute(string assetPath)
    {
        return Application.dataPath + "/../" + assetPath;
    }

    public static string PathAssets2Absolute2(string assetPath)
    {
        return Application.dataPath.Replace("/Assets", "") + "/" + assetPath;
    }

    /// <summary>
    /// 获取Transform的Hieraichy路径
    /// </summary>
    /// <param name="tf"></param>
    /// <returns></returns>
    public static string GetTransformHieraichyPath(Transform tf)
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

    /// <summary>
    /// 递归获得目录下的所有文件
    /// </summary>
    /// <param name="paths">目录（支持多个目录合并）</param>
    /// <param name="searchPattern">文件过滤器，例如:*.cs</param>
    /// <returns></returns>
    public static List<string> GetFiles(List<string> paths, string searchPattern = "*.*")
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

    /// <summary>
    /// 拷贝目录
    /// </summary>
    /// <param name="pathFrom">来源目录</param>
    /// <param name="pathTo">目标目录</param>
    /// <param name="clean">是否清理已经存在的目标目录，不清理同名文件也一定会覆盖</param>
    /// <param name="fileNameCheck">只要这个列表里包含的文件名</param>
    /// <param name="fileNameFilter">文件名需要包含这个列表的所有特征</param>
    /// <param name="fileNameNotCheck">这个列表里的不要</param>
    /// <param name="fileNameNotFilter">文件名不能包含这个列表的任意特征</param>
    public static void CopyDirectory(string pathFrom, string pathTo, bool clean = false, List<string> fileNameCheck = null,
        List<string> fileNameFilter = null, List<string> fileNameNotCheck = null, List<string> fileNameNotFilter = null)
    {
        if (string.IsNullOrEmpty(pathFrom) ||
            string.IsNullOrEmpty(pathTo))
        {
            return;
        }

        if (Directory.Exists(pathFrom) == false)
        {
            return;
        }

        if (clean)
        {
            if (Directory.Exists(pathTo))
            {
                Directory.Delete(pathTo, true);
            }
            Directory.CreateDirectory(pathTo);
        }
        else
        {
            if (!Directory.Exists(pathTo))
            {
                Directory.CreateDirectory(pathTo);
            }
        }

        string[] files = Directory.GetFiles(pathFrom);
        string fileName;
        foreach (string file in files)
        {
            fileName = Path.GetFileName(file);
            if (FileNameFilter(fileName, fileNameFilter, true) == false
                || FileNameFilter(fileName, fileNameNotFilter, false) == false
                || (fileNameCheck != null && !fileNameCheck.Contains(fileName))
                || (fileNameNotCheck != null && fileNameNotCheck.Contains(fileName)))
            {
                continue;
            }
            //不做删除的话，同名文件，只是大小写不一样，文件名不会替换
            if (File.Exists(pathTo + "/" + fileName))
            {
                File.Delete(pathTo + "/" + fileName);
            }
            File.Copy(pathFrom + "/" + fileName, pathTo + "/" + fileName, true);
        }

        string[] directs = Directory.GetDirectories(pathFrom);
        string directName;
        foreach (string direct in directs)
        {
            directName = Path.GetFileName(direct);
            CopyDirectory(direct, pathTo + "/" + directName, clean, fileNameCheck, fileNameFilter, fileNameNotCheck, fileNameNotFilter);
        }
    }

    /// <summary>
    /// 文件名过滤
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="fileNameFilter">检查特征组</param>
    /// <param name="include">true:包含所有特征才通过;false:不包含所有特征才通过</param>
    /// <returns>是否通过</returns>
    private static bool FileNameFilter(string fileName, List<string> fileNameFilter = null, bool include = true)
    {
        if (fileNameFilter == null
            || fileNameFilter.Count <= 0
            || string.IsNullOrEmpty(fileName))
        {
            return true;
        }
        foreach (string filter in fileNameFilter)
        {
            if (include)
            {
                if (!fileName.Contains(filter))
                {
                    return false;
                }
            }
            else
            {
                if (fileName.Contains(filter))
                {
                    return false;
                }
            }
        }
        return true;
    }
}