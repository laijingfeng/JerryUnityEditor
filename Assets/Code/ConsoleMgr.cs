
public static class ConsoleMgr
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void ClearConsole()
    {
        // This simply does "LogEntries.Clear()" the long way:  
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
}