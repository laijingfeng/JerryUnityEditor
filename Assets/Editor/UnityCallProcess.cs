using System.Diagnostics;

//version: 2017-09-02 08:26:04

public class UnityCallProcess
{
    /// <summary>
    /// 执行外部程序
    /// </summary>
    /// <param name="processName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static bool CallProcess(string processName, string param)
    {
        ProcessStartInfo process = new ProcessStartInfo
        {
            CreateNoWindow = false,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = processName,
            Arguments = param,
        };

        //UnityEngine.Debug.Log(processName + " " + param);

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