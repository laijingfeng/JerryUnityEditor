using System.IO;
using UnityEditor;

//version: 2018-01-04 11:39:56

public class SVNTool
{
    [MenuItem("Tools/Svn/Update_Project", false, 0)]
    static public void SvnUpdate()
    {
        DoSvnUpdate(EditorUtil.PathAssets2Absolute2(""));
    }

    [MenuItem("Tools/Svn/Commit_Project", false, 0)]
    static public void SvnCommit()
    {
        DoSvnCommit(EditorUtil.PathAssets2Absolute2(""));
    }

    [MenuItem("Assets/Svn/Update_Select", false)]
    static public void SvnUpdateAssets()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        if (selection == null || selection.Length <= 0)
        {
            UnityEngine.Debug.LogWarning("没有选中文件或文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(selection[0]);
        if (path.Contains("."))
        {
            path = Path.GetDirectoryName(path);
        }
        path = EditorUtil.PathAssets2Absolute2(path);
        DoSvnUpdate(path);
    }

    [MenuItem("Assets/Svn/Commit_Select", false)]
    static public void SvnCommitAssets()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        if (selection == null || selection.Length <= 0)
        {
            UnityEngine.Debug.LogWarning("没有选中文件或文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(selection[0]);
        if (path.Contains("."))
        {
            path = Path.GetDirectoryName(path);
        }
        path = EditorUtil.PathAssets2Absolute2(path);
        DoSvnCommit(path);
    }

    /// <summary>
    /// 用public方便其他插件使用
    /// </summary>
    /// <param name="path"></param>
    static public void DoSvnUpdate(string path)
    {
        UnityEngine.Debug.Log("Update " + EditorUtil.PathAbsolute2Assets(path));
        string param = string.Format(@"/command:update /path:""{0}"" /notempfile /closeonend:0", path);
        UnityCallProcess.CallProcess("TortoiseProc.exe", param);
    }

    /// <summary>
    /// 用public方便其他插件使用
    /// </summary>
    /// <param name="path"></param>
    static public void DoSvnCommit(string path)
    {
        UnityEngine.Debug.Log("Commit " + EditorUtil.PathAbsolute2Assets(path));
        string param = string.Format(@"/command:commit /path:""{0}"" /logmsg:"""" /notempfile /closeonend:0", path);
        UnityCallProcess.CallProcess("TortoiseProc.exe", param);
    }
}