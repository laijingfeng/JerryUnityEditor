using System.IO;
using UnityEditor;

//version: 2018-07-31 19:50:35

public class SVNTool
{
    [MenuItem("Tools/Svn/【更新】工程", false, 0)]
    static public void SvnUpdate()
    {
        DoSvnUpdate(EditorUtil.PathAssets2Absolute2(""));
    }

    [MenuItem("Tools/Svn/【提交】工程", false, 0)]
    static public void SvnCommit()
    {
        DoSvnCommit(EditorUtil.PathAssets2Absolute2(""));
    }

    [MenuItem("Tools/Svn/工程【日志】", false, 0)]
    static public void SvnLog()
    {
        DoSvnLog(EditorUtil.PathAssets2Absolute2(""));
    }

    [MenuItem("Assets/Svn/【更新】选中目录", false)]
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

    [MenuItem("Assets/Svn/【提交】选中目录", false)]
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

    [MenuItem("Assets/Svn/选中文件【日志】", false)]
    static public void SvnLogAssets()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        if (selection == null || selection.Length <= 0)
        {
            UnityEngine.Debug.LogWarning("没有选中文件或文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(selection[0]);
        path = EditorUtil.PathAssets2Absolute2(path);
        DoSvnLog(path);
    }

    /// <summary>
    /// <para>用public方便其他插件使用</para>
    /// </summary>
    /// <param name="path"></param>
    static public void DoSvnUpdate(string path)
    {
        UnityEngine.Debug.Log("Update " + EditorUtil.PathAbsolute2Assets(path));
        string param = string.Format(@"/command:update /path:""{0}"" /notempfile /closeonend:0", path);
        UnityCallProcess.CallProcess("TortoiseProc.exe", param);
    }

    /// <summary>
    /// <para>用public方便其他插件使用</para>
    /// </summary>
    /// <param name="path"></param>
    static public void DoSvnCommit(string path)
    {
        UnityEngine.Debug.Log("Commit " + EditorUtil.PathAbsolute2Assets(path));
        string param = string.Format(@"/command:commit /path:""{0}"" /logmsg:"""" /notempfile /closeonend:0", path);
        UnityCallProcess.CallProcess("TortoiseProc.exe", param);
    }

    /// <summary>
    /// <para>显示日志</para>
    /// <para>用public方便其他插件使用</para>
    /// </summary>
    /// <param name="path"></param>
    static public void DoSvnLog(string path)
    {
        string param = string.Format(@"/command:log /path:""{0}"" /notempfile /closeonend:0", path);
        UnityCallProcess.CallProcess("TortoiseProc.exe", param);
    }
}