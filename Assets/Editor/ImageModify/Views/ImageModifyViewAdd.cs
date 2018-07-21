using UnityEditor;
using UnityEngine;

public class ImageModifyViewAdd : ImageModifyViewBase
{
    protected Color m_Color = Color.white;

    protected void DrawColor()
    {
        m_Color = EditorGUILayout.ColorField(new GUIContent("颜色"), m_Color);
    }

    protected override void ChildDraw()
    {
        base.ChildDraw();

        EditorGUILayout.LabelField("从");
        DrawDir();
        DrawIdx();
        EditorGUILayout.LabelField("前，增加");
        DrawCnt();
        EditorGUILayout.LabelField(Dir2RowOrColumn() + "的");
        DrawColor();
        EditorGUILayout.LabelField("像素");

        GUILayout.Space(10);
        DrawName();
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

        if (m_Idx <= 0)
        {
            SetTip("开始位置需要为正", MessageType.Warning);
            return false;
        }

        if (m_Cnt <= 0)
        {
            SetTip("数量为0或负，无需修改", MessageType.Info);
            return false;
        }

        bool isRow = true;//是行方向
        int W = m_SrcTex.width;
        int H = m_SrcTex.height;
        int idx = 0;//[idx,idx+Cnt)

        if (m_Dir == 0 || m_Dir == 1)
        {
            H += m_Cnt;
            isRow = true;
            if (m_Dir == 1)
            {
                idx = m_Idx - 1;
            }
            else
            {
                idx = m_SrcTex.height - m_Idx + 1;
            }

            if (idx < 0 || idx + m_Cnt > H)
            {
                SetTip("设置越界了", MessageType.Error);
                return false;
            }
        }
        else
        {
            W += m_Cnt;
            isRow = false;
            if (m_Dir == 2)
            {
                idx = m_Idx - 1;
            }
            else
            {
                idx = m_SrcTex.width - m_Idx + 1;
            }

            if (idx < 0 || idx + m_Cnt > W)
            {
                SetTip("设置越界了", MessageType.Error);
                return false;
            }
        }
        
        Texture2D desTex = new Texture2D(W, H, m_SrcTex.format, false);
        if (isRow)
        {
            for (int i = 0; i < desTex.width; i++)
            {
                for (int j = 0; j < desTex.height; j++)
                {
                    if (j >= idx && j < idx + m_Cnt)
                    {
                        desTex.SetPixel(i, j, m_Color);
                    }
                    else if (j < idx)
                    {
                        desTex.SetPixel(i, j, m_SrcTex.GetPixel(i, j));
                    }
                    else
                    {
                        desTex.SetPixel(i, j, m_SrcTex.GetPixel(i, j - m_Cnt));
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < desTex.width; i++)
            {
                for (int j = 0; j < desTex.height; j++)
                {
                    if (i >= idx && i < idx + m_Cnt)
                    {
                        desTex.SetPixel(i, j, m_Color);
                    }
                    else if (i < idx)
                    {
                        desTex.SetPixel(i, j, m_SrcTex.GetPixel(i, j));
                    }
                    else
                    {
                        desTex.SetPixel(i, j, m_SrcTex.GetPixel(i - m_Cnt, j));
                    }
                }
            }
        }
        desTex.Apply();

        SaveDesTex(desTex);

        SetTip("完成", MessageType.Info);

        return true;
    }
}