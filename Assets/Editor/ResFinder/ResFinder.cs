using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Jerry
{
    public class ResFinder : EditorWindow
    {
        public const string OUTPUT_CONTENT_FILE = "ResFinderOutput.txt";

        /// <summary>
        /// 路径
        /// </summary>
        private string findPath = "Assets";
        private string findPath2 = "";
        private Rect pathRect;
        /// <summary>
        /// 过滤路径
        /// </summary>
        private string pathFilter = "";
        private string pathFilter2 = "";
        /// <summary>
        /// 名字
        /// </summary>
        private string withName = "";
        private string withName2 = "";
        /// <summary>
        /// Bundle
        /// </summary>
        private string withBundle = "";
        private string withBundle2 = "";
        /// <summary>
        /// 后缀
        /// </summary>
        private string withPostfix = "";
        private string withPostfix2 = "";
        /// <summary>
        /// 查找结果
        /// </summary>
        private List<Object> results = new List<Object>();
        private Vector2 scrollPos;
        private string tip = "设置条件，进行查找";
        private bool working = false;
        private List<string> workingPath = new List<string>();
        private bool useRegularExpression = false;

        [MenuItem("JerryWins/ResFinder")]
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

            pathFilter = EditorGUILayout.TextField(new GUIContent("过滤路径", "支持逻辑运算符"), pathFilter);
            EditorGUILayout.HelpBox("&:与\n|:或\n!:非\n_name0&(!hi|cc)\n(名称含_name0)且((名称不含hi)或(名称含cc))", MessageType.Info, true);
            withName = EditorGUILayout.TextField(new GUIContent("过滤名字", "支持正则和逻辑运算符"), withName);
            EditorGUILayout.BeginHorizontal();
            useRegularExpression = EditorGUILayout.ToggleLeft(new GUIContent("正则", "是否使用正则表达式"), useRegularExpression, GUILayout.MaxWidth(50));
            if (!useRegularExpression)
            {
                EditorGUILayout.HelpBox("&:与\n|:或\n!:非\n_name0&(!hi|cc)\n(名称含_name0)且((名称不含hi)或(名称含cc))", MessageType.Info, true);
            }
            EditorGUILayout.EndHorizontal();
            withBundle = EditorGUILayout.TextField(new GUIContent("过滤Bundle", "忽略大小写"), withBundle);
            withPostfix = EditorGUILayout.TextField(new GUIContent("过滤后缀", "忽略大小写"), withPostfix);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("查找", "点击进行查找")))
            {
                Work();
            }
            if (working)
            {
                if (GUILayout.Button(new GUIContent("停止", "点击停止查找")))
                {
                    working = false;
                    EditorCoroutineLooper.StopLoop(IE_GetFiles());
                }
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button(new GUIContent("输出查找结果", "点击输出查找结果")))
            {
                if (!working)
                {
                    string outFilePath = Application.dataPath + "/../" + ResFinder.OUTPUT_CONTENT_FILE;
                    if (System.IO.File.Exists(outFilePath))
                    {
                        File.Delete(outFilePath);
                    }

                    using (StreamWriter writer = new StreamWriter(outFilePath, true, Encoding.UTF8))
                    {
                        writer.WriteLine(string.Format("查找结果:{0}", results.Count));
                    }
                    for (int i = 0, imax = results.Count; i < imax; i++)
                    {
                        using (StreamWriter writer = new StreamWriter(outFilePath, true, Encoding.UTF8))
                        {
                            writer.WriteLine(results[i].name);
                        }
                    }
                    UnityEngine.Debug.LogWarning(string.Format("输出查找内容完成，见{0}", ResFinder.OUTPUT_CONTENT_FILE));
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(tip);

            if (results.Count > 0)
            {
                if (GUILayout.Button(new GUIContent("选中查找结果", "点击选中查找的结果")))
                {
                    Selection.objects = results.ToArray();
                }
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(300));
                for (int i = 0, imax = results.Count; i < imax; i++)
                {
                    EditorGUILayout.ObjectField(results[i], typeof(Object), true);
                }
                GUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        private void Work()
        {
            working = false;
            results.Clear();
            workingPath.Clear();
            withName2 = withName;
            withBundle2 = withBundle;
            withPostfix2 = withPostfix;
            findPath2 = findPath;
            pathFilter2 = pathFilter;
            EditorCoroutineLooper.StopLoop(IE_GetFiles());

            if (string.IsNullOrEmpty(withName2)
                && string.IsNullOrEmpty(withBundle2)
                && string.IsNullOrEmpty(withPostfix2)
                && string.IsNullOrEmpty(pathFilter2))
            {
                tip = "至少设置一个过滤条件";
                return;
            }

            workingPath.Add(EditorUtil.PathAssets2Absolute(findPath2));
            working = true;

            EditorCoroutineLooper.StartLoop(this, IE_GetFiles());
        }

        private IEnumerator IE_GetFiles()
        {
            while (working && workingPath.Count > 0)
            {
                GetFiles();
                if (workingPath.Count <= 0)
                {
                    break;
                }
                this.Repaint();
                yield return new WaitForEndOfFrame();
            }
            working = false;
            this.Repaint();
        }

        private void GetFiles()
        {
            if (!working || workingPath.Count <= 0)
            {
                return;
            }

            string path = workingPath[0];
            workingPath.RemoveAt(0);

            List<string> files = new List<string>(Directory.GetFiles(path, "*.*"));
            foreach (string file in files)
            {
                if (FilterAsset(file))
                {
                    results.Add(AssetDatabase.LoadMainAssetAtPath(EditorUtil.PathAbsolute2Assets(file)));
                }
            }

            foreach (string strDirectory in Directory.GetDirectories(path))
            {
                workingPath.Add(strDirectory);
            }
            tip = string.Format("查找结果如下({0}):", results.Count);
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
                if (!extenName.Equals("." + withPostfix2.ToLower()))
                {
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(withBundle2))
            {
                string assetPath = EditorUtil.PathAbsolute2Assets(path);
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (!withBundle.ToLower().Equals(importer.assetBundleName))
                {
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(pathFilter2))
            {
                string filePath = EditorUtil.PathAbsolute2Assets(path);
                filePath = Path.GetDirectoryName(filePath);
                return StringLogicJudge.Judge(filePath.ToLower(), pathFilter2.ToLower());
            }
            if (!string.IsNullOrEmpty(withName2))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (useRegularExpression)
                {
                    return Regex.IsMatch(fileName, @withName2);
                }
                else
                {
                    return StringLogicJudge.Judge(fileName.ToLower(), withName2.ToLower());
                }
            }
            return true;
        }
    }
}