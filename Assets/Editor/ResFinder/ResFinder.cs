using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ResFinder : EditorWindow
{
    /// <summary>
    /// 路径
    /// </summary>
    private string findPath = "Assets";
    private Rect pathRect;
    /// <summary>
    /// 名字
    /// </summary>
    private string withName = "";
    /// <summary>
    /// Bundle
    /// </summary>
    private string withBundle = "";
    /// <summary>
    /// 后缀
    /// </summary>
    private string withPostfix = "";
    /// <summary>
    /// 查找结果
    /// </summary>
    protected List<Object> results = new List<Object>();

    [MenuItem("Window/ResFinder")]
    private static void Open()
    {
        GetWindow<ResFinder>();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.MinHeight(300));
        EditorGUILayout.BeginVertical("box");

        pathRect = EditorGUILayout.GetControlRect();
        findPath = EditorGUI.TextField(pathRect, new GUIContent("查找目录", "拖拽需要的目录到这里即可"), findPath);
        if ((Event.current.type == EventType.dragUpdated || Event.current.type == EventType.DragExited) &&
            pathRect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null
                && DragAndDrop.paths.Length > 0
                && Directory.Exists(DragAndDrop.paths[0]))
            {
                findPath = DragAndDrop.paths[0];
            }
        }

        withName = EditorGUILayout.TextField(new GUIContent("过滤名字", "忽略大小写，支持逻辑运算符"), withName);
        EditorGUILayout.HelpBox("&:与\n|:或\n!:非\n_name0&(!hi|cc)\n(名称含_name0)且((名称不含hi)或(名称含cc))", MessageType.Info, true);
        withBundle = EditorGUILayout.TextField(new GUIContent("过滤Bundle", "忽略大小写"), withBundle);
        withPostfix = EditorGUILayout.TextField(new GUIContent("过滤后缀", "忽略大小写"), withPostfix);

        if (GUILayout.Button(new GUIContent("查找", "点击进行查找")))
        {
            Work();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(string.Format("查找结果如下({0}):", results.Count));

        if (results.Count > 0)
        {
            if (GUILayout.Button(new GUIContent("选中查找结果", "点击选中查找的结果")))
            {
                Selection.objects = results.ToArray();
            }
            foreach (Object obj in results)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private void Work()
    {
        results.Clear();
        string findPathAbs = Application.dataPath + "/../" + findPath;
        string[] files = Directory.GetFiles(findPathAbs, "*.*", SearchOption.AllDirectories)
            .Where(s => FilterAsset(s)).ToArray();

        if (files != null && files.Length > 0)
        {
            foreach (string file in files)
            {
                results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(file)));
            }
        }
    }

    protected string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    private bool FilterAsset(string path)
    {
        if (path.EndsWith(".meta"))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(withPostfix))
        {
            string extenName = Path.GetExtension(path).ToLower();
            if (!extenName.Equals("." + withPostfix.ToLower()))
            {
                return false;
            }
        }
        if (!string.IsNullOrEmpty(withBundle))
        {
            string assetPath = GetRelativeAssetsPath(path);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (!withBundle.ToLower().Equals(importer.assetBundleName))
            {
                return false;
            }
        }
        if (!string.IsNullOrEmpty(withName))
        {
            string fileName = Path.GetFileNameWithoutExtension(path).ToLower();
            return CheckName(fileName, withName.ToLower());
        }
        return true;
    }

    #region CheckPath

    private string m_CheckName;

    /// <summary>
    /// 名字匹配
    /// </summary>
    /// <param name="checkName"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    private bool CheckName(string checkName, string filter)
    {
        m_CheckName = checkName;

        if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(m_CheckName))
        {
            return true;
        }

        //换行符号去掉
        //空格不能去掉，空格可能是资源命名的空格
        filter = filter.Replace("\n", "");

        if (GrammarPass(filter) == false)
        {
            return false;
        }

        return DoSearch(filter);
    }

    private static List<char> m_oks = new List<char>() 
    {
        '&','|','_','(',')',' ','!'
    };

    /// <summary>
    /// 语法检测
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    private bool GrammarPass(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return true;
        }
        char[] chs = filter.ToCharArray();
        for (int i = 0; i < chs.Length; i++)
        {
            if (chs[i] >= '0' && chs[i] <= '9')
            {
                continue;
            }
            if (chs[i] >= 'a' && chs[i] <= 'z')
            {
                continue;
            }
            if (chs[i] >= 'A' && chs[i] <= 'Z')
            {
                continue;
            }
            if (m_oks.Contains(chs[i]) == false)
            {
                return false;
            }
        }
        return true;
    }

    private bool DoSearch(string filter)
    {
        List<string> ret = GetOneLogicPiece(filter);

        //一个逻辑语句，下面两种情况
        //1.带符号:a&b
        //2.不带符号:a
        if (ret == null || (ret.Count != 1 && ret.Count != 3))
        {
            UnityEngine.Debug.LogError("filter error:" + filter);
            return false;
        }

        //debug
        //foreach (string s in ret)
        //{
        //    Debug.LogWarning(filter + ":" + s);
        //}

        if (ret.Count == 1)
        {
            return JudgeOne(ret[0]);
        }

        if (ret[1] == "&")
        {
            return DoSearch(ret[0]) && DoSearch(ret[2]);
        }
        else
        {
            return DoSearch(ret[0]) || DoSearch(ret[2]);
        }
    }

    private bool JudgeOne(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            UnityEngine.Debug.LogError("filter error:" + s);
            return false;
        }

        s = s.Replace("(", "").Replace(")", "");
        if (s.Length <= 1)
        {
            return false;
        }

        if (s[0] == '!')
        {
            return !m_CheckName.Contains(s.Substring(1));
        }
        else
        {
            return m_CheckName.Contains(s);
        }
    }

    /// <summary>
    /// 获取一个逻辑语句
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    private List<string> GetOneLogicPiece(string filter)
    {
        List<string> ret = new List<string>();
        if (string.IsNullOrEmpty(filter))
        {
            return ret;
        }
        char[] chs = filter.ToCharArray();
        int cnt = 0;
        int s = 0;//开始下标
        int e = chs.Length - 1;//结束下标

        //去除最外层的括号
        while (true)
        {
            if (s >= e)
            {
                break;
            }

            if (chs[s] == '(' && chs[e] == ')')
            {
                s++;
                e--;
            }
            else
            {
                break;
            }
        }

        int idx = s;

        for (int i = s; i <= e; i++)
        {
            if (chs[i] == '(')
            {
                cnt++;
            }
            else if (chs[i] == ')')
            {
                cnt--;
                if (cnt < 0)
                {
                    UnityEngine.Debug.LogError("filter error");
                    return null;
                }
            }
            else if (chs[i] == '&')
            {
                if (cnt == 0)
                {
                    ret.Add(filter.Substring(idx, i - idx));
                    idx = i + 1;
                    ret.Add("&");
                    break;
                }
            }
            else if (chs[i] == '|')
            {
                if (cnt == 0)
                {
                    ret.Add(filter.Substring(idx, i - idx));
                    idx = i + 1;
                    ret.Add("|");
                    break;
                }
            }
        }

        ret.Add(filter.Substring(idx, e - idx + 1));

        return ret;
    }

    #endregion CheckPath
}