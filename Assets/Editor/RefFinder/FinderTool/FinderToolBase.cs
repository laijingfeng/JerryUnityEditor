using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class FinderToolBase
{
    public string tip = "";
    public MessageType tipType = MessageType.Info;
    protected List<Object> results = new List<Object>();
    public FinderToolMgrBase mgr = null;

    public abstract void Work(params object[] param);
    public void Draw()
    {
        if (results.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            foreach (Object obj in results)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
            EditorGUILayout.EndVertical();
        }
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

    protected string MyCarrierListStr()
    {
        if (mgr != null)
        {
            return mgr.MyCarrierListStr();
        }
        return "";
    }
}