using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class FindChinese : MonoBehaviour
{
    [MenuItem("Assets/FindChinese/OneLine_NotCheck", false)]
    private static void FindChinese_OneLine_NotCheck()
    {
        TryFind(false, false);
    }

    [MenuItem("Assets/FindChinese/OneLine_Check", false)]
    private static void FindChinese_OneLine_Check()
    {
        TryFind(false, true);
    }

    [MenuItem("Assets/FindChinese/AllLine_NotCheck", false)]
    private static void FindChinese_AllLine_NotCheck()
    {
        TryFind(true, false);
    }

    [MenuItem("Assets/FindChinese/AllLine_Check", false)]
    private static void FindChinese_AllLine_Check()
    {
        TryFind(true, true);
    }

    private static void TryFind(bool showAllLine = false, bool checkMark = false)
    {
        List<string> selectPaths = GetSelectPaths();
        if (selectPaths.Count > 0)
        {
            DoFindChinese(selectPaths, showAllLine, checkMark);
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

    private static void DoFindChinese(List<string> paths, bool showAllLine = false, bool checkMark = false)
    {
        Debug.Log("Finding");

        List<string> files = EditorUtil.GetFiles(paths, "*.cs");

        int cnt = 0;
        Regex r = new Regex(@"^((?!/).)+"".*?[\u4e00-\u9fa5]+.*?""");
        Regex r2 = new Regex(@"""(.*?[\u4e00-\u9fa5]+.*?)""");
        for (int i = 0; i < files.Count; i++)
        {
            string[] contents = File.ReadAllLines(files[i]);
            for (int j = 0; j < contents.Length; j++)
            {
                if (r.IsMatch(contents[j]))
                {
                    bool isContainMark = contents[j].Trim().Contains("UIHelper.TranslateTest");
                    if (checkMark && isContainMark)
                    {
                        continue;
                    }
                    Match m = r2.Match(contents[j]);
                    Debug.LogWarningFormat("<color={0}>{1}</color>\n{2}\t{3}",
                        isContainMark ? "yellow" : "red", contents[j].Trim(),
                        m.Groups[1].Value, Path.GetFileName(files[i]));
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