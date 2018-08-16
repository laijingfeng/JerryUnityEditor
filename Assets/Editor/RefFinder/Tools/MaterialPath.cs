using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class MaterialPath : FinderToolBasePath
{
    protected override string GetSupportInfoExt()
    {
        string ext = "模型和预设使用的是从特定对象查找的接口";
        if (string.IsNullOrEmpty(base.GetSupportInfoExt()))
        {
            return ext;
        }
        return string.Format("{0},{1}", base.GetSupportInfoExt(), ext);
    }

    protected override void WorkPath(Object findObject, string findPath, Object newObject)
    {
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findPathAbs = Application.dataPath + "/../" + findPath;
        string[] files = Directory.GetFiles(findPathAbs, "*.*", SearchOption.AllDirectories)
            .Where(s => IsMyCarrier(s)).ToArray();

        if (files != null && files.Length > 0)
        {
            int startIndex = 0;
            string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);

            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                Object fileObj = AssetDatabase.LoadAssetAtPath<Object>(EditorUtil.PathAbsolute2Assets(file));
                bool useRegex = true;
                if (fileObj != null)
                {
                    FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(fileObj);
                    if (type == FinderToolMgrBase.AssetType.GameObject
                        || type == FinderToolMgrBase.AssetType.Fbx)
                    {
                        useRegex = false;
                        results.AddRange(MaterialObject.DoOneGameObject(findObject, fileObj as GameObject));
                    }
                }

                if (useRegex)
                {
                    if (Regex.IsMatch(File.ReadAllText(file), findObjectGuid))
                    {
                        results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(file)));
                    }
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
                }
            };
        }

        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }
}