using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SendScreen : MonoBehaviour
{
    /// <summary>
    /// 帧率
    /// </summary>
    [Tooltip("帧率")]
    public int frameRate = 25;
    public Camera m_Camera;
    public RawImage m_TarImg;

    private RenderTexture rt;
    private int width;
    private int height;
    private Texture2D tex_output;
    private Texture2D tex_input;
    private Rect rect;
    private Color[] readColors;

    private void Start()
    {
        width = Screen.width;
        height = Screen.height;
        rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        tex_output = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex_input = new Texture2D(width, height, TextureFormat.ARGB32, false);
        rect = new Rect(0, 0, width, height);
        this.StartCoroutine("IE_CaptureScreen");
    }

    private IEnumerator IE_CaptureScreen()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(0.1f);
            CaptureOneFrame();
        }
    }

    /// <summary>
    /// 捕获一帧
    /// </summary>
    private void CaptureOneFrame()
    {
        //Debug.LogWarning("CaptureOneFrame");

        m_Camera.targetTexture = rt;
        m_Camera.Render();
        RenderTexture.active = rt;
        tex_output.ReadPixels(rect, 0, 0);
        tex_output.Apply();
        m_TarImg.texture = tex_output;
        m_Camera.targetTexture = null;

        //readColors = tex_output.GetPixels();
        //print(readColors.Length + " " + rect.width + " " + rect.height);
    }
}