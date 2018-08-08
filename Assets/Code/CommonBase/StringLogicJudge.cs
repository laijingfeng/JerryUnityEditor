using System.Collections.Generic;
using System.Text.RegularExpressions;

//version: 2018-08-08 14:26:51

/// <summary>
/// 字符串逻辑运算
/// </summary>
public class StringLogicJudge
{
    static public bool Judge(string checkStr, string filter)
    {
        if (string.IsNullOrEmpty(checkStr)
            || string.IsNullOrEmpty(filter))
        {
            return true;
        }
        //换行符号去掉
        //空格不能去掉，空格可能是资源命名的空格
        filter = filter.Replace("\n", "");
        if (GrammarPass(filter) == false)
        {
            UnityEngine.Debug.LogError("[StringLogicJudge]语法检查不通过:" + filter);
            return false;
        }

        return DoSearch(checkStr, filter);
    }

    #region 查找

    static private bool DoSearch(string checkStr, string filter)
    {
        List<string> ret = GetOneLogicPiece(filter);

        //一个逻辑语句，下面两种情况
        //1.带符号:a&b
        //2.不带符号:a
        if (ret == null || (ret.Count != 1 && ret.Count != 3))
        {
            UnityEngine.Debug.LogError("[StringLogicJudge]过滤串错误:" + filter);
            return false;
        }

        //debug
        //foreach (string s in ret)
        //{
        //    Debug.LogWarning(filter + ":" + s);
        //}

        if (ret.Count == 1)
        {
            return JudgeOne(checkStr, ret[0]);
        }

        if (ret[1] == "&")
        {
            return DoSearch(checkStr, ret[0]) && DoSearch(checkStr, ret[2]);
        }
        else
        {
            return DoSearch(checkStr, ret[0]) || DoSearch(checkStr, ret[2]);
        }
    }

    static private bool JudgeOne(string checkStr, string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            UnityEngine.Debug.LogError("filter error:" + filter);
            return false;
        }

        filter = filter.Replace("(", "").Replace(")", "");
        if (filter.Length <= 0)
        {
            return false;
        }

        if (filter[0] == '!')
        {
            if (filter.Length > 1)
            {
                return !checkStr.Contains(filter.Substring(1));
            }
            else
            {
                return false;
            }
        }
        else
        {
            return checkStr.Contains(filter);
        }
    }

    /// <summary>
    /// 获取一个逻辑语句
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    static private List<string> GetOneLogicPiece(string filter)
    {
        List<string> ret = new List<string>();
        if (string.IsNullOrEmpty(filter))
        {
            return ret;
        }
        char[] chs = filter.ToCharArray();
        int cnt = 0;//括号匹配
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

    #endregion 查找

    #region 语法检查

    /// <summary>
    /// <para>搜索串能包含的字符，再加0-9/a-z/A-Z/汉字</para>
    /// <para>[/]和[.]可能路径用，[@]Unity中的动画文件名中有</para>
    /// <para>[&]、[|]、[!]、[(]、[)]是逻辑符号</para>
    /// </summary>
    static private List<char> m_oks = new List<char>()
    {
        '&','|','!','(',')',
        '~','@','#','$','%','^',
        '_',' ','/','.','-','+','=',
        '[',']','{','}'
    };

    /// <summary>
    /// 语法检测
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    static private bool GrammarPass(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return true;
        }
        char[] chs = filter.ToCharArray();
        for (int i = 0; i < chs.Length; i++)
        {
            if (Regex.IsMatch(chs[i].ToString(), @"[\u4e00-\u9fa5]"))
            {
                continue;
            }
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

    #endregion 语法检查
}