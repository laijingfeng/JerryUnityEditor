using System.Diagnostics;

//version: 2018-04-11 23:37:14

/// <summary>
/// Unity执行外部程序
/// </summary>
public class UnityCallProcess
{
    /// <summary>
    /// 执行外部程序
    /// </summary>
    /// <param name="processName">程序名</param>
    /// <param name="param">参数</param>
    /// <param name="withWindow">是否带有系统窗口</param>
    /// <returns>是否成功</returns>
    public static bool CallProcess(string processName, string param, bool withWindow = true)
    {
        ProcessStartInfo process = new ProcessStartInfo
        {
            CreateNoWindow = !withWindow,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = processName,
            Arguments = param,
        };

        Process p = Process.Start(process);
        p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        string error = p.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(error))
        {
            UnityEngine.Debug.LogError(processName + " " + param + "  ERROR! " + "\n" + error);

            string output = p.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                UnityEngine.Debug.Log(output);
            }
            return false;
        }
        return true;
    }
}