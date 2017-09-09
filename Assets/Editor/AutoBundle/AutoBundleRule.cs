using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jerry
{
    [System.Serializable]
    public class AutoBundleRule
    {
        public enum PathFilterType
        {
            Name,
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

        public virtual void Draw()
        {
            EditorGUILayout.BeginVertical("box");

            this.m_InUse = EditorGUILayout.Toggle(new GUIContent("InUse", "是否启用，不启用相当于不存在"), this.m_InUse);
            this.m_TmpIgnore = EditorGUILayout.Toggle(new GUIContent("TmpIgnore", "暂时忽略，要调试某个资源的时候，可以将管辖的设置暂时定义为[暂时忽略]，不会强制设置属性，同时也挡住继续往上查找下一个"), this.m_TmpIgnore);

            this.m_MyName = EditorGUILayout.TextField("Name", this.m_MyName);

            this.m_PathFilterType = (PathFilterType)EditorGUILayout.EnumPopup("PathFilterType", this.m_PathFilterType);

            EditorGUILayout.LabelField("PathFilter");
            this.m_PathFilter = EditorGUILayout.TextArea(this.m_PathFilter, GUILayout.MinWidth(180));
            EditorGUILayout.HelpBox("&:与\n|:或\n!:非\n_name0&(!hi|cc)\n(路径含_name0)且((路径不含hi)或(路径含cc))", MessageType.Info, true);

            this.m_Bundle = EditorGUILayout.TextField("Bundle", this.m_Bundle);

            EditorGUILayout.EndVertical();
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
        /// 
        /// </summary>
        /// <param name="importer"></param>
        /// <returns></returns>
        public virtual bool ApplySettings(AssetImporter importer)
        {
            if (m_TmpIgnore)
            {
                return false;
            }
            importer.assetBundleName = m_Bundle;
            return true;
        }
    }
}