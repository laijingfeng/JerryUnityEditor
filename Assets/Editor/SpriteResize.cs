using System.IO;
using UnityEditor;
using UnityEngine;

public class SpriteResize : Editor
{
    [MenuItem("Assets/适配成4的倍数尺寸", false)]
    private static void Work()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        if (selection == null
            || selection.Length != 1)
        {
            return;
        }

        string filePath = AssetDatabase.GetAssetPath(selection[0]);
        Texture2D srcTex = selection[0] as Texture2D;
        if (srcTex.width % 4 == 0 && srcTex.height % 4 == 0)
        {
            Debug.LogWarning("【" + Path.GetFileName(filePath) + "】的尺寸已经是4的倍数了");
            return;
        }

        TextureImporter importer = TextureImporter.GetAtPath(filePath) as TextureImporter;
        if (!importer.isReadable)
        {
            Debug.LogWarning("需要勾选Read/Write Enabled，转完后记得改回原来的设置");
            return;
        }
        if (!srcTex.format.ToString().Equals("RGBA32"))
        {
            Debug.LogWarning("图的压缩需要设置为【RGBA 32 bit】，现在是【" + srcTex.format.ToString() + "】，转完后记得改回原来的设置");
            return;
        }

        int addW = (4 - (srcTex.width % 4)) % 4;
        int addH = (4 - (srcTex.height % 4)) % 4;
        int addWHalf = addW / 2;
        int addHHalf = addH / 2;

        Texture2D desTex = new Texture2D(srcTex.width + addW, srcTex.height + addH, srcTex.format, false);
        for (int i = 0; i < desTex.width; i++)
        {
            for (int j = 0; j < desTex.height; j++)
            {
                if (i < addWHalf || j < addHHalf
                    || i >= srcTex.width + addWHalf
                    || j >= srcTex.height + addHHalf)
                {
                    desTex.SetPixel(i, j, new Color(0, 0, 0, 0));
                }
                else
                {
                    desTex.SetPixel(i, j, srcTex.GetPixel(i - addWHalf, j - addHHalf));
                }
            }
        }
        desTex.Apply();
        string desTexPath = Application.dataPath + "/../" + filePath;
        File.WriteAllBytes(desTexPath, desTex.EncodeToPNG());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.LogWarning("【" + Path.GetFileName(filePath) + "】完成");
    }

    [MenuItem("Assets/适配成4的倍数尺寸", true)]
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