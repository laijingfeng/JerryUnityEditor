using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AnimatorControllerPath : FinderToolBasePath
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

    protected override void WorkPath(UnityEngine.Object findObject, string findPath, UnityEngine.Object newObject)
    {
        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);
        string findPathAbs = Application.dataPath + "/../" + findPath;

        DoWorkPath(findPathAbs, (filePath) =>
        {
            UnityEngine.Object fileObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(EditorUtil.PathAbsolute2Assets(filePath));
            bool useRegex = true;
            if (fileObj != null)
            {
                FinderToolMgrBase.AssetType type = FinderToolMgrBase.Object2Type(fileObj);
                if (type == FinderToolMgrBase.AssetType.GameObject
                    || type == FinderToolMgrBase.AssetType.Fbx)
                {
                    useRegex = false;
                    results.AddRange(AnimatorControllerObject.DoOneGameObject(findObject, fileObj as GameObject));
                }
            }

            if (useRegex)
            {
                if (Regex.IsMatch(File.ReadAllText(filePath), findObjectGuid))
                {
                    results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(filePath)));
                }
            }
        });
    }
}