using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class PrefabPath : FinderToolBasePath
{
    private string GetFileID(Object obj)
    {
        PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
        SerializedObject srlzedObject = new SerializedObject(obj);
        inspectorModeInfo.SetValue(srlzedObject, InspectorMode.Debug, null);
        SerializedProperty localIdProp = srlzedObject.FindProperty("m_LocalIdentfierInFile");
        return localIdProp.intValue.ToString();
    }

    protected override void WorkPath(Object findObject, string findPath, Object newObject)
    {
        string newObjectPath = "";
        string newObjectGuid = "";
        string newObjectFileId = "";

        if (newObject != null)
        {
            newObjectPath = AssetDatabase.GetAssetPath(newObject);
            newObjectGuid = AssetDatabase.AssetPathToGUID(newObjectPath);
            newObjectFileId = GetFileID(newObject);
        }

        string findObjectPath = AssetDatabase.GetAssetPath(findObject);
        string findObjectFileId = GetFileID(findObject);

        string findPathAbs = Application.dataPath + "/../" + findPath;
        string[] files = Directory.GetFiles(findPathAbs, "*.*", SearchOption.AllDirectories)
            .Where(s => IsMyCarrier(s)).ToArray();

        bool hasDoReplace = false;

        if (files != null && files.Length > 0)
        {
            int startIndex = 0;
            string findObjectGuid = AssetDatabase.AssetPathToGUID(findObjectPath);

            EditorApplication.update = delegate()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
                if (Regex.IsMatch(File.ReadAllText(file), @"prefab: {fileID: " + findObjectFileId + ", guid: " + findObjectGuid + ", type: 2}"))
                {
                    //要替换
                    if (!string.IsNullOrEmpty(newObjectGuid))
                    {
                        string newFile = File.ReadAllText(file)
                            .Replace(@"prefab: {fileID: " + findObjectFileId + ", guid: " + findObjectGuid + ", type: 2}", @"prefab: {fileID: " + newObjectFileId + ", guid: " + newObjectGuid + ", type: 2}");
                        File.WriteAllText(file, newFile);
                        hasDoReplace = true;
                    }

                    results.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(file)));
                }
                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);

                    if (hasDoReplace)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            };
            SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
        }
    }
}