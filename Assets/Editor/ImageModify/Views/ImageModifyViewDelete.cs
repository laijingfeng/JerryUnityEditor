﻿using UnityEditor;
using UnityEngine;

public class ImageModifyViewDelete : ImageModifyViewBase
{
    protected override void ChildDraw()
    {
        base.ChildDraw();

        EditorGUILayout.LabelField("从");
        DrawDir();
        DrawIdx();
        EditorGUILayout.LabelField("开始，删除");
        DrawCnt();
        EditorGUILayout.LabelField(Dir2RowOrColumn() + "的像素");

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
            H -= m_Cnt;
            isRow = true;
            if (m_Dir == 1)
            {
                idx = m_Idx - 1;
            }
            else
            {
                idx = m_SrcTex.height - m_Idx + 1 - m_Cnt;
            }

            if (idx < 0 || idx + m_Cnt > m_SrcTex.height)
            {
                SetTip("设置越界了", MessageType.Error);
                return false;
            }
        }
        else
        {
            W -= m_Cnt;
            isRow = false;
            if (m_Dir == 2)
            {
                idx = m_Idx - 1;
            }
            else
            {
                idx = m_SrcTex.width - m_Idx + 1 - m_Cnt;
            }

            if (idx < 0 || idx + m_Cnt > m_SrcTex.width)
            {
                SetTip("设置越界了", MessageType.Error);
                return false;
            }
        }

        Texture2D desTex = new Texture2D(W, H, m_SrcTex.format, false);
        if (isRow)
        {
            for (int i = 0; i < m_SrcTex.width; i++)
            {
                for (int j = 0; j < m_SrcTex.height; j++)
                {
                    if (j >= idx && j < idx + m_Cnt)
                    {
                        continue;
                    }
                    else if (j < idx)
                    {
                        desTex.SetPixel(i, j, m_SrcTex.GetPixel(i, j));
                    }
                    else
                    {
                        desTex.SetPixel(i, j - m_Cnt, m_SrcTex.GetPixel(i, j));
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < m_SrcTex.width; i++)
            {
                for (int j = 0; j < m_SrcTex.height; j++)
                {
                    if (i >= idx && i < idx + m_Cnt)
                    {
                        continue;
                    }
                    else if (i < idx)
                    {
                        desTex.SetPixel(i, j, m_SrcTex.GetPixel(i, j));
                    }
                    else
                    {
                        desTex.SetPixel(i - m_Cnt, j, m_SrcTex.GetPixel(i, j));
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