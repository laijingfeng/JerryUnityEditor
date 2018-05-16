#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#endif

public static class EditorCoroutineLooper
{
#if UNITY_EDITOR
    private static Dictionary<IEnumerator, object> m_loopers = new Dictionary<IEnumerator, object>();
    private static bool M_Started = false;
    private static List<IEnumerator> M_DropItems = new List<IEnumerator>();

    /// <summary>
    /// 开启Loop
    /// </summary>
    /// <param name="mb">脚本</param>
    /// <param name="iterator">方法</param>
    public static void StartLoop(object mb, IEnumerator iterator)
    {
        if (mb != null && iterator != null)
        {
            if (!m_loopers.ContainsKey(iterator))
            {
                m_loopers.Add(iterator, mb);
            }
            else
            {
                m_loopers[iterator] = mb;
            }
        }
        if (!M_Started)
        {
            M_Started = true;
            EditorApplication.update += Update;
        }
    }

    public static void StopLoop(IEnumerator iterator)
    {
        if (iterator != null && m_loopers.ContainsKey(iterator))
        {
            m_loopers.Remove(iterator);
        }
    }

    private static void Update()
    {
        if (m_loopers.Count > 0)
        {
            //出现过报错：InvalidOperationException: out of sync，现换foreach为for
            List<IEnumerator> list = new List<IEnumerator>(m_loopers.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                if (i >= list.Count || list[i] == null)
                {
                    continue;
                }
                var key = list[i];
                var mb = m_loopers[list[i]];
                //卸载时丢弃Looper
                if (mb == null)
                {
                    M_DropItems.Add(key);
                    continue;
                }
                //隐藏时别执行Loop
                if (mb is MonoBehaviour)
                {
                    if (!(mb as MonoBehaviour).gameObject.activeInHierarchy)
                    {
                        continue;
                    }
                }
                //执行Loop，执行完毕也丢弃Looper
                IEnumerator ie = key;
                if (!ie.MoveNext())
                {
                    M_DropItems.Add(key);
                }
            }
            //集中处理丢弃的Looper
            for (int i = 0; i < M_DropItems.Count; i++)
            {
                if (M_DropItems[i] != null)
                {
                    m_loopers.Remove(M_DropItems[i]);
                }
            }
            M_DropItems.Clear();
        }

        if (m_loopers.Count == 0)
        {
            EditorApplication.update -= Update;
            M_Started = false;
        }
    }
#endif
}