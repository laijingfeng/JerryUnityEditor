using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildTools : Editor
{
    [MenuItem("Tools/导出APK", false, 1)]
    static public void ExportAndroidApk()
    {
        DoSettings();
        string exportPath = string.Format("{0}/../{1}_{2}.apk",
            Application.dataPath, PlayerSettings.productName, System.DateTime.Now.ToString("HHmmss"));
        DoBuild(exportPath, BuildOptions.None);
    }

    [MenuItem("Tools/导出Android工程", false, 1)]
    static public void ExportAndroidProject()
    {
        DoSettings();
        string exportPath = Application.dataPath + "/../ExportAndroid";
        if (Directory.Exists(exportPath))
        {
            Directory.Delete(exportPath, true);
        }
        Directory.CreateDirectory(exportPath);
        DoBuild(exportPath, BuildOptions.AcceptExternalModificationsToPlayer);
    }

    private static void DoSettings()
    {
        //PlayerSettings.productName = "UnityProject";
        PlayerSettings.bundleIdentifier = string.Format("com.jerry.lai.{0}", PlayerSettings.productName);
        PlayerSettings.keystorePass = "jerrylai@jingfeng*1990";
        PlayerSettings.keyaliasPass = "lai123";
    }

    private static void DoBuild(string path, BuildOptions opt)
    {
        BuildPipeline.BuildPlayer(new string[] 
        {
            "Assets/Main.unity",
        },
        path,
        BuildTarget.Android,
        opt);
    }
}