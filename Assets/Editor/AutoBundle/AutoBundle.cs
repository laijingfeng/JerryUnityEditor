using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 自动设置Bundle
/// </summary>
[System.Serializable]
public class AutoBundle : ScriptableObject
{
    /// <summary>
    /// 是否显示日志
    /// </summary>
    public bool showLog;

    /// <summary>
    /// 规则集合
    /// </summary>
    [UnityEngine.SerializeField]
    public List<AutoBundleRule> sets;

    /// <summary>
    /// 创建
    /// </summary>
    /// <returns></returns>
    public static AutoBundle CreateAssetRule()
    {
        AutoBundle autoBundle = AutoBundle.CreateInstance<AutoBundle>();
        autoBundle.Init();
        return autoBundle;
    }

    public void Init()
    {
        sets = new List<AutoBundleRule>();
    }

    /// <summary>
    /// 是否匹配
    /// </summary>
    /// <param name="importer"></param>
    /// <param name="setName"></param>
    /// <returns></returns>
    public bool IsMatch(AssetImporter importer, out string setName)
    {
        setName = string.Empty;
        if (sets == null || sets.Count < 1)
        {
            return false;
        }
        foreach (AutoBundleRule s in sets)
        {
            if (s.CanUseBundleElse(importer)
                || s.Match(importer))
            {
                setName = s.m_MyName;
                if (showLog)
                {
                    Debug.Log(string.Format("<color=white>{0}</color> <color=yellow>{1}.{2}</color>", importer.assetPath, this.name, setName));
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 执行设置
    /// </summary>
    /// <param name="importer"></param>
    /// <returns></returns>
    public bool ApplySettings(AssetImporter importer)
    {
        if (sets == null || sets.Count < 1)
        {
            return false;
        }
        foreach (AutoBundleRule s in sets)
        {
            if (s.Match(importer))
            {
                s.ApplySettings(importer, false);
                return true;
            }
            else if (s.CanUseBundleElse(importer))
            {
                s.ApplySettings(importer, true);
                return true;
            }
        }
        return false;
    }
}