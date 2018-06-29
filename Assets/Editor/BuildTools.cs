using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#if HOTFIX_ENABLE
using CSObjectWrapEditor;
using XLua;
#endif

//Version: 2018-06-29-00

//Unity5.6.1里用到/../这种路径的时候，会被判定为导出到Assets目录了，所以不要出现这种写法
public class BuildTools : Editor
{
#if UNITY_ANDROID
    [MenuItem("Tools/导出APK(Mono2x)", false, 0)]
    static public void ExportAndroidApk_Mono()
    {
        DoSettings();

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        string exportPath = string.Format("{0}/{1}_Mono2x_{2}({3})_{4}_{5}.apk",
            Application.dataPath.Replace("/Assets", ""),
            PlayerSettings.productName,
            PlayerSettings.bundleVersion,
            PlayerSettings.Android.bundleVersionCode,
            System.DateTime.Now.ToString("yyyyMMdd_HHmmss"),
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));

        DoBuild(exportPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Tools/导出APK(IL2CPP)", false, 0)]
    static public void ExportAndroidApk_IL2CPP()
    {
        DoSettings();

        //IL2CPP单设置打包参数是不行的，必须更改配置
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        string exportPath = string.Format("{0}/{1}_IL2CPP_{2}({3})_{4}_{5}.apk",
            Application.dataPath.Replace("/Assets", ""),
            PlayerSettings.productName,
            PlayerSettings.bundleVersion,
            PlayerSettings.Android.bundleVersionCode,
            System.DateTime.Now.ToString("yyyyMMdd_HHmmss"),
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));

        DoBuild(exportPath, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Tools/导出Android工程", false, 2)]
    static public void ExportAndroidProject()
    {
        DoSettings();
        string exportPath = Application.dataPath.Replace("/Assets", "") + "/ExportAndroid";
        if (Directory.Exists(exportPath))
        {
            Directory.Delete(exportPath, true);
        }
        Directory.CreateDirectory(exportPath);
        DoBuild(exportPath, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
    }

    [MenuItem("Tools/仅仅做Android设置", false, 3)]
    static public void JustDoSettings()
    {
        DoSettings();
    }
#endif

#if UNITY_WEBGL
    [MenuItem("Tools/导出WebGL", false, 0)]
    static public void ExportWebGL()
    {
        HandleXlua();
        string exportPath = Application.dataPath.Replace("/Assets", "") + "/WebGL";
        if (Directory.Exists(exportPath))
        {
            Directory.Delete(exportPath, true);
        }
        Directory.CreateDirectory(exportPath);
        DoBuild(exportPath, BuildTarget.WebGL, BuildOptions.Il2CPP);
    }
#endif

#if UNITY_IOS
    [MenuItem("Tools/导出iOS", false, 0)]
    static public void ExportIOS()
    {
        HandleXlua();
        string dirName = "iOS" + System.DateTime.Now.ToString("yyyMMdd_HHmmss");
        string exportPath = Application.dataPath.Replace("/Assets", "") + "/" + dirName;

        if (Directory.Exists(exportPath))
        {
            Directory.Delete(exportPath, true);
        }
        Directory.CreateDirectory(exportPath);
        DoBuild(exportPath, BuildTarget.iOS, BuildOptions.Il2CPP);
        
        ModifyIOS(dirName);
    }

    static private void ModifyIOS(string dirName)
    {
        string curDir = Directory.GetCurrentDirectory();
        string toolsPath = curDir + "/iOSRes/";
        try
        {
            Directory.SetCurrentDirectory(toolsPath);
            UnityCallProcess.CallProcess("python.exe", string.Format("{0} {1}", toolsPath + "modify_ios.py", "project_name-" + dirName), false);
            Directory.SetCurrentDirectory(curDir);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
            Directory.SetCurrentDirectory(curDir);
        }
    }

#endif

    private static void DoSettings()
    {
        //PlayerSettings.productName = "UnityProject";
        PlayerSettings.companyName = "Jerry";
        PlayerSettings.applicationIdentifier = string.Format("com.jerry.lai.{0}", PlayerSettings.productName);
        PlayerSettings.Android.keystoreName = "./jerry.keystore";
        PlayerSettings.Android.keystorePass = "jerrylai@jingfeng*1990";
        PlayerSettings.Android.keyaliasName = "jerrylai";
        PlayerSettings.Android.keyaliasPass = "lai123";

        HandleXlua();
    }

    private static void HandleXlua()
    {
#if HOTFIX_ENABLE
        Generator.ClearAll();
        Generator.GenAll();
        //有时还会出现提示信息说没有GenCode，经过测试是误提示，所以强制设置上
        DelegateBridge.Gen_Flag = true;
#endif
    }

    private static string[] GetLevels()
    {
        if (EditorBuildSettings.scenes == null || EditorBuildSettings.scenes.Length <= 0)
        {
            return null;
        }
        List<string> ret = new List<string>();
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
        {
            if (s.enabled == true)
            {
                ret.Add(s.path);
            }
        }
        return ret.ToArray();
    }

    private static void DoBuild(string path, BuildTarget tar, BuildOptions opt)
    {
        string[] levels = GetLevels();
        if (levels == null || levels.Length <= 0)
        {
            Debug.LogWarning("打包的场景列表为空，请在BuildSettings的ScenesInBuild设置要打包的场景");
            return;
        }
        BuildPipeline.BuildPlayer(levels,
        path,
        tar,
        opt);
        Debug.Log(string.Format("build sucess to {0} at {1}", path, System.DateTime.Now));
    }
}