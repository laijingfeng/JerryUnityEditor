using System.IO;
using UnityEditor;
using UnityEngine;

public class Atlas2Sprite : Editor
{
    [MenuItem("Assets/Atlas2Sprite", false)]
    private static void Work()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        if (selection == null
            || selection.Length != 1)
        {
            return;
        }
        Debug.LogWarning("注意：需要设置Read/Write Enabled为true，并且只有部分Format才可以，如RGBA32");
        string filePath = AssetDatabase.GetAssetPath(selection[0]);
        object[] objs = AssetDatabase.LoadAllAssetsAtPath(filePath);
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string outDir = Application.dataPath + "/../" + fileName + "/";
        if (!Directory.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
        }
        foreach (object obj in objs)
        {
            if (obj.GetType() == typeof(UnityEngine.Sprite))
            {
                UnityEngine.Sprite st = (UnityEngine.Sprite)obj;
                Texture2D tx = new Texture2D((int)st.rect.width, (int)st.rect.height, st.texture.format, false);
                tx.SetPixels(st.texture.GetPixels((int)st.rect.xMin, (int)st.rect.yMin, (int)st.rect.width, (int)st.rect.height));
                tx.Apply();
                File.WriteAllBytes(outDir + obj.ToString().Replace("(UnityEngine.Sprite)", "") + ".png", tx.EncodeToPNG());
            }
        }
        Debug.LogWarning(fileName + " finish");
    }

    [MenuItem("Assets/Atlas2Sprite", true)]
    private static bool WorkValidation()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        if (selection == null
            || selection.Length != 1)
        {
            return false;
        }
        string path = AssetDatabase.GetAssetPath(selection[0]);
        if (path.EndsWith(".png"))
        {
            return true;
        }
        return false;
    }
}