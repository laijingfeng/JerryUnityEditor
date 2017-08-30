#if UNITY_EDITOR_WIN
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoad]
class UpdateUnityEditorTitle
{
    static UpdateUnityEditorTitle()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DoUpdateTitleFunc;
    }

    static void DoUpdateTitleFunc(int instanceID, Rect selectionRect)
    {
        UpdateUnityEditorProcess.Instance.SetTitle();
    }
}

public class UpdateUnityEditorAssetHandler
{
    [OnOpenAssetAttribute(1)]
    public static bool AssetHandlerStep(int instanceID, int line)
    {
        UpdateUnityEditorProcess.lasttime = 0f;
        return false; // we did not handle the open
    }
}

class UpdateUnityEditorProcess
{
    public IntPtr hwnd = IntPtr.Zero;
    private bool haveMainWindow = false;
    private IntPtr mainWindowHandle = IntPtr.Zero;
    private int processId = 0;
    private IntPtr hwCurr = IntPtr.Zero;
    private static StringBuilder sbtitle = new StringBuilder(255);
    private static string UTitle = Application.dataPath;
    public static float lasttime = 0;
    public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);
    private static UpdateUnityEditorProcess _instance;

    public static UpdateUnityEditorProcess Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UpdateUnityEditorProcess();
                _instance.hwnd = _instance.GetMainWindowHandle(Process.GetCurrentProcess().Id);
            }
            return _instance;
        }
    }

    public void SetTitle()
    {
        //UnityEngine.Debug.Log(string.Format("{0} - {1}", Time.realtimeSinceStartup, lasttime));
        if (Time.realtimeSinceStartup > lasttime)
        {
            sbtitle.Length = 0;
            lasttime = Time.realtimeSinceStartup + 2f;
            int length = GetWindowTextLength(hwnd);
            GetWindowText(hwnd.ToInt32(), sbtitle, 255);
            string strTitle = sbtitle.ToString();
            string[] ss = strTitle.Split('-');
            if (strTitle.Length > 0 && !ss[0].Contains(UTitle))
            {
                SetWindowText(hwnd.ToInt32(), string.Format("{0} - {1}", UTitle, strTitle));
            }
        }
    }

    public IntPtr GetMainWindowHandle(int processId)
    {
        if (!this.haveMainWindow)
        {
            this.mainWindowHandle = IntPtr.Zero;
            this.processId = processId;
            EnumThreadWindowsCallback callback = new EnumThreadWindowsCallback(this.EnumWindowsCallback);
            EnumWindows(callback, IntPtr.Zero);
            GC.KeepAlive(callback);
            this.haveMainWindow = true;
        }
        return this.mainWindowHandle;
    }

    private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
    {
        int num;
        GetWindowThreadProcessId(new HandleRef(this, handle), out num);
        if ((num == this.processId) && this.IsMainWindow(handle))
        {
            this.mainWindowHandle = handle;
            return false;
        }
        return true;
    }

    private bool IsMainWindow(IntPtr handle)
    {
        return (!(GetWindow(new HandleRef(this, handle), 4) != IntPtr.Zero) && IsWindowVisible(new HandleRef(this, handle)));
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool IsWindowVisible(HandleRef hWnd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private extern static int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Auto)]
    public extern static int SetWindowText(int hwnd, string lpString);
}
#endif