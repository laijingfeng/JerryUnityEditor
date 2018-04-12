using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Jerry
{
    /// <summary>
    /// 资源重命名
    /// </summary>
    public class ResRename : EditorWindow
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
        /// 替换规则
        /// </summary>
        private string replaceRule;
        private bool useRegularExpression = false;

        [MenuItem("JerryWins/ResRename")]
        private static void Open()
        {
            GetWindow<ResRename>();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.MinHeight(300));
            EditorGUILayout.BeginVertical("box");

            pathRect = EditorGUILayout.GetControlRect();
            findPath = EditorGUI.TextField(pathRect, new GUIContent("操作目录：", "拖拽需要的目录到这里即可"), findPath);
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

            withName = EditorGUILayout.TextField(new GUIContent("过滤名字：", "支持正则和逻辑运算符"), withName);

            EditorGUILayout.BeginHorizontal();
            useRegularExpression = EditorGUILayout.ToggleLeft(new GUIContent("正则", "是否使用正则表达式"), useRegularExpression, GUILayout.MaxWidth(50));
            if (!useRegularExpression)
            {
                EditorGUILayout.HelpBox("&:与\n|:或\n!:非\n_name0&(!hi|cc)\n(名称含_name0)且((名称不含hi)或(名称含cc))", MessageType.Info, true);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("替换规则：");
            replaceRule = EditorGUILayout.TextArea(replaceRule);

            if (GUILayout.Button(new GUIContent("替换", "点击进行替换")))
            {
                Work();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void Work()
        {
            if (string.IsNullOrEmpty(replaceRule))
            {
                UnityEngine.Debug.LogWarning("没有设置替换规则");
                return;
            }
            string[] rules = replaceRule.Split('\n');
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] rule;
            foreach (string r in rules)
            {
                if (string.IsNullOrEmpty(r.Trim()))
                {
                    continue;
                }
                rule = r.Trim().Split('=');
                if (rule != null && rule.Length == 2)
                {
                    if (dic.ContainsKey(rule[0]))
                    {
                        dic[rule[0]] = rule[1];
                    }
                    else
                    {
                        dic.Add(rule[0], rule[1]);
                    }
                }
            }
            if (dic.Keys.Count <= 0)
            {
                UnityEngine.Debug.LogWarning("没有设置替换规则");
                return;
            }

            Dictionary<string, string>.Enumerator e = dic.GetEnumerator();
            while (e.MoveNext())
            {
                Debug.LogWarning(e.Current.Key + " " + e.Current.Value);
            }
        }

        private bool FilterAsset(string path)
        {
            if (path.EndsWith(".meta"))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(withName))
            {
                string fileName = Path.GetFileName(path);
                if (useRegularExpression)
                {
                    return Regex.IsMatch(fileName, @withName);
                }
                else
                {
                    return StringLogicJudge.Judge(fileName.ToLower(), withName.ToLower());
                }
            }
            return true;
        }
    }
}