using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public abstract class FinderToolBasePath : FinderToolBase
{
    /// <summary>
    /// 是否取消
    /// </summary>
    protected static bool isCancel;

    protected override string GetSupportInfoExt()
    {
        string ext = "默认使用GUID匹配查找";
        if (string.IsNullOrEmpty(base.GetSupportInfoExt()))
        {
            return ext;
        }
        return string.Format("{0},{1}", base.GetSupportInfoExt(), ext);
    }

    public override void Work(params object[] param)
    {
        results.Clear();
        if (param == null || param.Length != 3)
        {
            SetTip("内部参数错误", MessageType.Error);
            return;
        }
        WorkPath((UnityEngine.Object)param[0], (string)param[1], (UnityEngine.Object)param[2]);
    }

    protected abstract void WorkPath(UnityEngine.Object findObject, string findPath, UnityEngine.Object newObject);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">绝对路径</param>
    /// <returns></returns>
    protected bool IsMyCarrier(string path)
    {
        return IsMyCarrier(FinderToolMgrBase.Path2Type(path));
    }

    /// <summary>
    /// <para>获取对象的FileID</para>
    /// <para>Hierarchy的对象无效</para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected string GetFileID(UnityEngine.Object obj)
    {
        PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
        SerializedObject srlzedObject = new SerializedObject(obj);
        inspectorModeInfo.SetValue(srlzedObject, InspectorMode.Debug, null);
        SerializedProperty localIdProp = srlzedObject.FindProperty("m_LocalIdentfierInFile");
        return localIdProp.intValue.ToString();
    }

    /// <summary>
    /// 获取一个目录下的文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static List<string> GetFiles(string path)
    {
        List<string> filePath = new List<string>();
        if (File.Exists(path))
        {
            filePath.Add(path);
        }
        else
        {
            filePath.AddRange(Directory.GetFiles(path));
        }
        return filePath;
    }

    /// <summary>
    /// 查找文件
    /// </summary>
    /// <param name="path">根目录</param>
    /// <param name="isMatch">匹配函数</param>
    /// <param name="handle">处理函数</param>
    /// <param name="finish">结束回调</param>
    /// <returns></returns>
    private static IEnumerator IE_SearchDir(string path, Func<string, bool> isMatch, Action<string> handle, Action finish)
    {
        List<string> dirs = new List<string>();
        dirs.Add(path);
        string workDir = string.Empty;

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (dirs.Count <= 0 || isCancel)
            {
                break;
            }

            workDir = dirs[0];
            dirs.RemoveAt(0);

            List<string> filePath = GetFiles(workDir);
            foreach (string p in filePath)
            {
                if (p.EndsWith(".meta"))
                {
                    continue;
                }
                if (isMatch(p))
                {
                    handle(p);
                }
            }

            dirs.AddRange(Directory.GetDirectories(workDir));
        }
        finish();
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="absPath"></param>
    /// <param name="workOneFile"></param>
    /// <param name="finish">结束回调</param>
    protected void DoWorkPath(string absPath, Action<string> workOneFile, Action finish = null)
    {
        isCancel = false;
        int cnt = 0;

        EditorCoroutineLooper.StartLoop(new FinderJustTrickForIE(), IE_SearchDir(absPath, IsMyCarrier, (filePath) =>
        {
            workOneFile(filePath);
            cnt++;
            isCancel = EditorUtility.DisplayCancelableProgressBar(string.Format("查找中，进度是无用的，已查找{0}个资源", cnt), EditorUtil.PathAbsolute2Assets(filePath), 1);
        },
        () =>
        {
            EditorUtility.ClearProgressBar();
            SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
            if (finish != null)
            {
                finish();
            }
        }));

        SetTip(string.Format("查找结果如下({0}):", results.Count), MessageType.Info);
    }
}