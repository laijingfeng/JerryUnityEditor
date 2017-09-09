using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Jerry
{
    [System.Serializable]
    public class AutoBundle : ScriptableObject
    {
        public bool showLog;
        public List<AutoBundleRule> sets;

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

        public bool IsMatch(AssetImporter importer, out string setName)
        {
            setName = string.Empty;
            if (sets == null || sets.Count < 1)
            {
                return false;
            }
            foreach (AutoBundleRule s in sets)
            {
                if (s.Match(importer))
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
                    s.ApplySettings(importer);
                    return true;
                }
            }
            return false;
        }
    }
}