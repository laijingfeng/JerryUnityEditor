using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;

/// <summary>
/// 查找器
/// </summary>
public abstract class FinderToolBase
{
    /// <summary>
    /// 提示信息
    /// </summary>
    public string tip = "";
    /// <summary>
    /// 提示信息
    /// </summary>
    public MessageType tipType = MessageType.Info;

    /// <summary>
    /// 查找结果
    /// </summary>
    protected List<Object> results = new List<Object>();
    public FinderToolMgrBase mgr = null;

    /// <summary>
    /// 获取支持信息
    /// </summary>
    public string GetSupportInfo()
    {
        return string.Format("将查找的资源类型:{0}\n补充说明:{1}", MyCarrierListStr(), GetSupportInfoExt());
    }

    /// <summary>
    /// 额外的支持信息
    /// </summary>
    /// <returns></returns>
    protected virtual string GetSupportInfoExt()
    {
        return "";
    }

    public abstract void Work(params object[] param);

    private Vector2 scrollPos;
    public void Draw()
    {
        if (results.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(300));
            foreach (Object obj in results)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

    public void OutputFindContent()
    {
        string outFilePath = Application.dataPath + "/../" + RefFinder.OUTPUT_CONTENT_FILE;
        if (System.IO.File.Exists(outFilePath))
        {
            File.Delete(outFilePath);
        }
        using (StreamWriter writer = new StreamWriter(outFilePath, true, Encoding.UTF8))
        {
            writer.WriteLine(string.Format("查找结果:{0}", results.Count));
        }
        if (results.Count > 0)
        {
            foreach (Object obj in results)
            {
                using (StreamWriter writer = new StreamWriter(outFilePath, true, Encoding.UTF8))
                {
                    writer.WriteLine(obj.name);
                }
            }
        }
        SetTip(string.Format("输出查找内容完成，见{0}", RefFinder.OUTPUT_CONTENT_FILE), MessageType.Info);
    }

    protected void SetTip(string info, MessageType type)
    {
        tip = info;
        tipType = type;
    }

    protected string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    protected bool IsMyCarrier(FinderToolMgrBase.AssetType type)
    {
        if (mgr != null)
        {
            return mgr.IsMyCarrier(type);
        }
        return false;
    }

    /// <summary>
    /// 我的载体列表
    /// </summary>
    /// <returns></returns>
    protected string MyCarrierListStr()
    {
        if (mgr != null)
        {
            return mgr.MyCarrierListStr();
        }
        return "";
    }
}