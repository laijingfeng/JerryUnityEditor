using System.IO;
using UnityEditor;
using UnityEngine;

public class ImageModifyViewOther : ImageModifyViewBase
{
    protected override void ChildDraw()
    {
        base.ChildDraw();

        EditorGUILayout.LabelField("Atlas输出散图");
        EditorGUILayout.LabelField("输出路径是工程根目录，图片名命名的文件夹");
        GUILayout.Space(10);
    }

    protected override bool Work()
    {
        bool ret = base.Work();
        if (ret == false)
        {
            return ret;
        }

        if (!CheckSrcTex())
        {
            return false;
        }

        string fileName = Path.GetFileNameWithoutExtension(m_WorkPath);
        string outDir = Application.dataPath + "/../" + fileName + "/";
        if (Directory.Exists(outDir))
        {
            Directory.Delete(outDir, true);
        }
        Directory.CreateDirectory(outDir);
        object[] objs = AssetDatabase.LoadAllAssetsAtPath(m_WorkPath);
        foreach (object obj in objs)
        {
            if (obj.GetType() == typeof(UnityEngine.Sprite))
            {
                UnityEngine.Sprite st = (UnityEngine.Sprite)obj;
                Texture2D tx = new Texture2D((int)st.rect.width, (int)st.rect.height, st.texture.format, false);
                tx.SetPixels(st.texture.GetPixels((int)st.rect.xMin, (int)st.rect.yMin, (int)st.rect.width, (int)st.rect.height));
                tx.Apply();
                File.WriteAllBytes(outDir + obj.ToString().Replace(" (UnityEngine.Sprite)", "") + ".png", tx.EncodeToPNG());
            }
        }

        SetTip("完成", MessageType.Info);

        return true;
    }
}