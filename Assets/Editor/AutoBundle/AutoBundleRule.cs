using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 自动设置Bundle的规则
/// </summary>
[System.Serializable]
public class AutoBundleRule
{
    /// <summary>
    /// 路径过滤方式
    /// </summary>
    public enum PathFilterType
    {
        /// <summary>
        /// 文件名
        /// </summary>
        Name,
        /// <summary>
        /// 全路径
        /// </summary>
        Path,
    }

    public static AutoBundleRule CreateAutoBundleRule(string name)
    {
        AutoBundleRule set = (AutoBundleRule)Activator.CreateInstance(typeof(AutoBundleRule), true);
        set.Init(name);
        return set;
    }

    /// <summary>
    /// <para>是否启用</para>
    /// <para>不启用相当于不存在</para>
    /// </summary>
    public bool m_InUse = true;
    /// <summary>
    /// <para>暂时忽略</para>
    /// <para>要调试某个资源的时候，可以将管辖的设置暂时定义为[暂时忽略]，不会强制设置属性，同时也挡住继续往上查找下一个</para>
    /// </summary>
    public bool m_TmpIgnore = false;
    public PathFilterType m_PathFilterType;
    public string m_PathFilter;
    public string m_MyName;
    public string m_Bundle;
    /// <summary>
    /// 是否启用BundleElse
    /// </summary>
    public bool m_UseBundleElse;
    /// <summary>
    /// 否则设置这个Bundle
    /// </summary>
    public string m_BundleElse;

    /// <summary>
    /// 绘制
    /// </summary>
    public virtual void Draw()
    {
        EditorGUILayout.BeginVertical("box");

        this.m_InUse = EditorGUILayout.Toggle(new GUIContent("是否启用", "是否启用，不启用相当于不存在"), this.m_InUse);
        this.m_TmpIgnore = EditorGUILayout.Toggle(new GUIContent("暂时忽略", "暂时忽略，要调试某个资源的时候，可以将管辖的设置暂时定义为[暂时忽略]，不会强制设置属性，同时也挡住继续往上查找下一个"), this.m_TmpIgnore);

        this.m_MyName = EditorGUILayout.TextField("规则名称", this.m_MyName);

        this.m_PathFilterType = (PathFilterType)EditorGUILayout.EnumPopup("过滤范围", this.m_PathFilterType);

        EditorGUILayout.LabelField("过滤规则");
        this.m_PathFilter = EditorGUILayout.TextArea(this.m_PathFilter, GUILayout.MinWidth(180));
        EditorGUILayout.HelpBox("&:与\n|:或\n!:非\n_name0&(!hi|cc)\n(路径含_name0)且((路径不含hi)或(路径含cc))", MessageType.Info, true);

        this.m_Bundle = EditorGUILayout.TextField("AssetBundle", this.m_Bundle);
        this.m_UseBundleElse = EditorGUILayout.Toggle(new GUIContent("启用否则Bundle", "不合规则时，设置为另一个AseetBundle"), this.m_UseBundleElse);
        if (this.m_UseBundleElse)
        {
            this.m_BundleElse = EditorGUILayout.TextField("否则AssetBundle", this.m_BundleElse);
        }
        EditorGUILayout.HelpBox("$FILENAME可以替换不带后缀的名称\n$FOLDERNAME可以替换直接文件夹名", MessageType.Info, true);

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// BundleElse是否能够启用
    /// </summary>
    /// <param name="importer"></param>
    /// <returns></returns>
    public virtual bool CanUseBundleElse(AssetImporter importer)
    {
        if (m_InUse == false)
        {
            return false;
        }
        if (!this.m_UseBundleElse)
        {
            return false;
        }
        if (importer == null)
        {
            return false;
        }
        return true;
    }

    public virtual bool Match(AssetImporter importer)
    {
        if (m_InUse == false)
        {
            return false;
        }
        if (importer == null)
        {
            return false;
        }
        return StringLogicJudge.Judge((m_PathFilterType == PathFilterType.Path) ? importer.assetPath : Path.GetFileName(importer.assetPath), m_PathFilter);
    }

    public virtual void Init(string name)
    {
        m_InUse = true;
        m_PathFilter = string.Empty;
        m_MyName = name;
        m_PathFilterType = PathFilterType.Name;
    }

    /// <summary>
    /// 执行设置
    /// </summary>
    /// <param name="importer"></param>
    /// <param name="isBundleElse"></param>
    /// <returns></returns>
    public virtual bool ApplySettings(AssetImporter importer, bool isBundleElse = false)
    {
        if (m_TmpIgnore)
        {
            return false;
        }
        string fileName = Path.GetFileNameWithoutExtension(importer.assetPath).ToLower();
        string folderName = Path.GetFileName(Path.GetDirectoryName(importer.assetPath));
        string bundle = isBundleElse ? m_BundleElse : m_Bundle;
        if (!string.IsNullOrEmpty(bundle))
        {
            bundle = bundle.Replace("$FILENAME", fileName);
            bundle = bundle.Replace("$FOLDERNAME", folderName);
        }
        importer.assetBundleName = bundle;
        return true;
    }
}