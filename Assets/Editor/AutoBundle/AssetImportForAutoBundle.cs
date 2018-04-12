using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jerry
{
    public class AssetImportForAutoBundle : AssetPostprocessor
    {
        /// <summary>
        /// 查找一个AutoBundle
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static private AutoBundle FindAutoBundle(AssetImporter impoter, string path)
        {
            return SearchRecursive(impoter, path);
        }

        /// <summary>
        /// 当前目录查找，然后往上递归
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static private AutoBundle SearchRecursive(AssetImporter impoter, string path)
        {
            foreach (var findAsset in AssetDatabase.FindAssets("t:AutoBundle", new[] { Path.GetDirectoryName(path) }))
            {
                var p = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(findAsset));
                if (p == Path.GetDirectoryName(path))
                {
                    string setName = string.Empty;
                    AutoBundle rule = AssetDatabase.LoadAssetAtPath<AutoBundle>(AssetDatabase.GUIDToAssetPath(findAsset));
                    if (rule != null && rule.IsMatch(impoter, out setName))
                    {
                        return rule;
                    }
                }
            }

            path = Directory.GetParent(path).FullName;
            path = path.Replace('\\', '/');
            path = path.Remove(0, Application.dataPath.Length);
            path = path.Insert(0, "Assets");
            if (path != "Assets")
            {
                return SearchRecursive(impoter, path);
            }
            return null;
        }

        /// <summary>
        /// 资源是否需要检测
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static private bool NeedCheck(string s)
        {
            if (!s.Contains("."))//文件夹
            {
                return false;
            }
            if (s.Contains("/Editor/") || s.Contains("/Plugins/"))//编辑器和插件
            {
                return false;
            }
            string extension = Path.GetExtension(s);
            if (extension.Equals(".cs") || extension.Equals(".dll"))//代码
            {
                return false;
            }
            //插件的文件
            if (extension.Equals(".a") || extension.Equals(".so") || extension.Equals(".jar") || extension.Equals(".aar"))
            {
                return false;
            }
            return true;
        }

        static private void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string s in importedAssets)
            {
                if (NeedCheck(s))
                {
                    FindRuleAndSet(s);
                }
            }
        }

        static private void FindRuleAndSet(string path)
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            AutoBundle rule = FindAutoBundle(importer, importer.assetPath);
            if (rule == null)
            {
                return;
            }
            rule.ApplySettings(importer);
        }
    }
}