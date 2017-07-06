using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class FindChinese : MonoBehaviour
{
    [MenuItem("Assets/FindChinese/ShowOneLine", false)]
    private static void FindChinese_ShowOneLine()
    {
        List<string> selectPaths = GetSelectPaths();
        if (selectPaths.Count > 0)
        {
            DoFindChinese(selectPaths, false);
        }
        else
        {
            Debug.Log("Select Nothing");
        }
    }

    [MenuItem("Assets/FindChinese/ShowAllLine", false)]
    private static void FindChinese_ShowAllLine()
    {
        List<string> selectPaths = GetSelectPaths();
        if (selectPaths.Count > 0)
        {
            DoFindChinese(selectPaths, true);
        }
        else
        {
            Debug.Log("Select Nothing");
        }
    }

    private static List<string> GetSelectPaths()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        string str;
        List<string> selectPaths = new List<string>();
        foreach (UnityEngine.Object obj in selection)
        {
            str = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(str))
            {
                selectPaths.Add(Path.GetFullPath(str));
            }
        }
        return selectPaths;
    }

    private static void DoFindChinese(List<string> paths, bool showAllLine)
    {
        Debug.Log("Finding");

        List<string> files = EditorUtil.GetFiles(paths, "*.cs");

        int cnt = 0;
        Regex r = new Regex(@"^((?!/).)+"".*?[\u4e00-\u9fa5]+.*?""");
        for (int i = 0; i < files.Count; i++)
        {
            string[] contents = File.ReadAllLines(files[i]);
            for (int j = 0; j < contents.Length; j++)
            {
                if (r.IsMatch(contents[j]))
                {
                    cnt++;
                    Debug.LogFormat(AssetDatabase.LoadMainAssetAtPath(EditorUtil.PathAbsolute2Assets(files[i])), "<color=yellow>{0}</color> Line.{1}", Path.GetFileName(files[i]), j + 1);
                    if (!showAllLine)
                    {
                        break;
                    }
                }
            }
        }
        Debug.Log("Do FindChinese InfoCnt=" + cnt);
    }
}