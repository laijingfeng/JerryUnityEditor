using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BMFontEditor : EditorWindow
{
    [MenuItem("Tools/BMFont Maker")]
    static public void OpenBMFontMaker()
    {
        EditorWindow.GetWindow<BMFontEditor>(false, "BMFont Maker", true).Show();
    }

    [SerializeField]
    private Font targetFont;
    [SerializeField]
    private Material fontMaterial;
    [SerializeField]
    private Texture2D fontTexture;

    private string tipInfo = string.Empty;

    void OnGUI()
    {
        targetFont = EditorGUILayout.ObjectField("字体", targetFont, typeof(Font), false) as Font;
        fontMaterial = EditorGUILayout.ObjectField("材质", fontMaterial, typeof(Material), false) as Material;
        fontTexture = EditorGUILayout.ObjectField("贴图", fontTexture, typeof(Texture2D), false) as Texture2D;
        GUILayout.Label("切图用字符命名，空格用SPACE");

        if (GUILayout.Button("Create BMFont"))
        {
            MakeFont();
        }

        GUILayout.Space(10);
        GUILayout.Label(tipInfo);
    }

    private void MakeFont()
    {
        tipInfo = "";
        if (targetFont == null)
        {
            tipInfo = "[字体]不能为空";
            return;
        }
        if (fontMaterial == null)
        {
            tipInfo = "[材质]不能为空";
            return;
        }
        if (fontTexture == null)
        {
            tipInfo = "[贴图]不能为空";
            return;
        }

        string texturePath = AssetDatabase.GetAssetPath(fontTexture);
        Object[] objs = AssetDatabase.LoadAllAssetsAtPath(texturePath);

        List<Sprite> sprites = new List<Sprite>();
        foreach (Object obj in objs)
        {
            if (obj.GetType() == typeof(UnityEngine.Sprite)
                && (obj.name.Length == 1 || obj.name.Equals("SPACE")))//用字符命名，空格不能用，所以特判
            {
                sprites.Add(obj as Sprite);
            }
        }

        if (sprites.Count <= 0)
        {
            tipInfo = "贴图没有切图，或切图没有符合命名规则的";
            return;
        }

        float lineSpace = 0.1f;//字体行间距

        fontMaterial.shader = Shader.Find("GUI/Text Shader");
        fontMaterial.SetTexture("_MainTex", fontTexture);
        targetFont.material = fontMaterial;
        CharacterInfo[] characterInfo = new CharacterInfo[sprites.Count];

        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite s = sprites[i];

            if (s.rect.height > lineSpace)
            {
                lineSpace = s.rect.height;
            }

            CharacterInfo info = new CharacterInfo();
            info.index = (int)s.name[0];
            if (s.name.Equals("SPACE"))
            {
                info.index = (int)' ';
            }
            Rect rect = s.rect;

            //==设置字符映射到材质上的坐标==
            info.uvBottomLeft = new Vector2(rect.x / fontTexture.width, rect.y / fontTexture.height);
            info.uvBottomRight = new Vector2((rect.x + rect.width) / fontTexture.width, rect.y / fontTexture.height);
            info.uvTopLeft = new Vector2(rect.x / fontTexture.width, (rect.y + rect.height) / fontTexture.height);
            info.uvTopRight = new Vector2((rect.x + rect.width) / fontTexture.width, (rect.y + rect.height) / fontTexture.height);

            //==设置字符顶点的偏移位置和宽高==

            //根据pivot设置字符的偏移，绘制的时候如何偏移(extend)
            //x轴上是[0,width]的区域
            //y轴上的理解是：在lineSpace高的区间绘制，extend设置的是读到的uv应该映射到lineSpace区间的哪一段，用minY和maxY来描述，参考系是中心，然后上正下负
            //所以正常显示应该满足maxY-minY=height，如果minY是0的话就是从中心开始往上绘制

            //下面我们要做的是把字符绘制到lineSpace空间的Sprite.pivot位置

            //s.pivot.y的范围是[0,height]，center是0.5*height
            if (s.pivot.y > 0.5 * rect.height)
            {
                //锚点占据上部的比例
                float offset_rate = (s.pivot.y - 0.5f * rect.height) / (0.5f * rect.height);
                info.maxY = (int)(0.5f * lineSpace * offset_rate);
                info.minY = info.maxY - (int)rect.height;
            }
            else if (s.pivot.y < 0.5 * rect.height)
            {
                float offset_rate = (0.5f * rect.height - s.pivot.y) / (0.5f * rect.height);
                info.minY = -(int)(0.5f * lineSpace * offset_rate);
                info.maxY = (int)(rect.height + info.minY);
            }
            else
            {
                info.minY = (int)(-rect.height / 2);
                info.maxY = (int)(rect.height + info.minY);
            }

            info.minX = 0;
            info.maxX = (int)rect.width;
            
            //==设置字符的宽度==
            info.advance = (int)rect.width;
            characterInfo[i] = info;
        }

        targetFont.characterInfo = characterInfo;

        EditorUtility.SetDirty(targetFont);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        tipInfo = string.Format("完成，LineSpacing是{0}，需要手动设置", lineSpace);
    }
}