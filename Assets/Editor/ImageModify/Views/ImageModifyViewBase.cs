using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImageModifyViewBase
{
    protected string m_WorkPath = "";
    protected string m_Tip = "";
    protected MessageType m_TipType = MessageType.Info;
    /// <summary>
    /// 上下左右(0123)
    /// </summary>
    protected int m_Dir = 0;
    protected int m_Idx = 1;
    protected int m_Cnt = 1;
    protected string m_Name = "";
    protected Texture2D m_SrcTex = null;

    private List<GUIContent> m_DirList = new List<GUIContent>()
    {
        new GUIContent("上"),
        new GUIContent("下"),
        new GUIContent("左"),
        new GUIContent("右"),
    };

    public void Draw(string workPath)
    {
        m_WorkPath = workPath;

        ChildDraw();

        GUILayout.Space(10);
        DrawName();
        GUILayout.Space(10);

        if (GUILayout.Button("点击开始"))
        {
            Work();
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.HelpBox(m_Tip, m_TipType, true);
        EditorGUILayout.EndVertical();
    }

    protected string Dir2RowOrColumn()
    {
        if (m_Dir == 0 || m_Dir == 1)
        {
            return "行";
        }
        return "列";
    }

    protected void DrawDir()
    {
        m_Dir = EditorGUILayout.Popup(new GUIContent("方位"), m_Dir, m_DirList.ToArray());
    }

    protected void DrawIdx()
    {
        m_Idx = EditorGUILayout.IntField(new GUIContent(Dir2RowOrColumn() + "数(1开始)"), m_Idx);
    }

    protected void DrawCnt()
    {
        m_Cnt = EditorGUILayout.IntField(new GUIContent("数量"), m_Cnt);
    }

    protected void DrawName()
    {
        m_Name = EditorGUILayout.TextField(new GUIContent("输出名字后缀(空是覆盖)"), m_Name);
    }

    protected virtual void ChildDraw()
    {
    }

    protected void SaveDesTex(Texture2D des)
    {
        string desTexPath = m_WorkPath;
        if (!string.IsNullOrEmpty(m_Name))
        {
            desTexPath = desTexPath.Replace(".png", m_Name + ".png");
        }
        File.WriteAllBytes(desTexPath, des.EncodeToPNG());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    protected bool CheckSrcTex()
    {
        m_SrcTex = AssetDatabase.LoadAssetAtPath<Texture2D>(m_WorkPath) as Texture2D;
        TextureImporter importer = TextureImporter.GetAtPath(m_WorkPath) as TextureImporter;
        if (!importer.isReadable)
        {
            SetTip("图需要勾选Read/Write Enabled", MessageType.Warning);
            return false;
        }
        if (!m_SrcTex.format.ToString().Equals("RGBA32"))
        {
            SetTip("图的压缩需要设置为【RGBA 32 bit】", MessageType.Warning);
            return false;
        }
        return true;
    }

    protected virtual bool Work()
    {
        if (string.IsNullOrEmpty(m_WorkPath))
        {
            SetTip("修改对象不能为空", MessageType.Warning);
            return false;
        }
        if (!m_WorkPath.EndsWith(".png"))
        {
            SetTip("修改对象只能是png", MessageType.Warning);
            return false;
        }
        return true;
    }

    protected void SetTip(string tip, MessageType tipType)
    {
        m_Tip = tip;
        m_TipType = tipType;
    }
}