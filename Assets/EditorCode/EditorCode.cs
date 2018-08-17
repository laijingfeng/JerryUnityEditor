#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EditorCode : EditorWindow
{
    private static float m_ShowTime = 0;
    private static string m_Tip = "";
    private static EditorCode m_Window;
    private static float m_EndTime = 0;

    /// <summary>
    /// <para>显示提示</para>
    /// <para>注意只有UNITY_EDITOR才可以使用</para>
    /// </summary>
    /// <param name="msg">提示信息</param>
    /// <param name="confirm">是否需要确认才关闭</param>
    /// <param name="time">自动关闭时间</param>
    public static void ShowTip(string msg, bool confirm = false, float time = 2.0f)
    {
        if (confirm)
        {
            EditorUtility.DisplayDialog("提示", msg, "关闭");
        }
        else
        {
            m_ShowTime = time;
            m_Tip = msg;
            ShowWin();
        }
    }

    private static void ShowWin()
    {
        EditorApplication.update -= DoUpdate;
        if (m_Window != null)
        {
            m_Window.Close();
            m_Window = null;
        }

        m_EndTime = Time.realtimeSinceStartup + m_ShowTime;
        EditorApplication.update += DoUpdate;

        Rect rect = new Rect(0, 0, 400, 400);
        rect.x = (Screen.currentResolution.width - rect.width) * 0.5f;
        rect.y = (Screen.currentResolution.height - rect.height) * 0.5f;
        m_Window = EditorWindow.GetWindowWithRect<EditorCode>(rect, true, "提示", false);
        //强制设置位置，不然会使用上次的
        m_Window.position = rect;
        m_Window.Show(true);
    }

    private static void DoUpdate()
    {
        if (Time.realtimeSinceStartup > m_EndTime)
        {
            EditorApplication.update -= DoUpdate;
            if (m_Window != null)
            {
                m_Window.Close();
                m_Window = null;
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        
        GUILayout.Space(10);

        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.wordWrap = true;
        style.normal.textColor = Color.red;
        GUILayout.Label(new GUIContent(m_Tip), style);

        GUILayout.Space(60);

        if (GUILayout.Button(string.Format("关闭({0:F1}秒后自动关闭)", m_ShowTime)))
        {
            this.Close();
            m_Window = null;
            m_EndTime = 0;
        }

        GUILayout.EndVertical();
    }
}
#endif