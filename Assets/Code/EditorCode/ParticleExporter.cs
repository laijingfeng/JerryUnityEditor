using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class ParticleExporter : MonoBehaviour
{
    /// <summary>
    /// 导出帧率
    /// </summary>
    public int frameRate = 25;
    /// <summary>
    /// 需要导出几帧
    /// </summary>
    public float frameCount = 100;
    /// <summary>
    /// 相机
    /// </summary>
    public Camera exportCamera;
    /// <summary>
    /// 左上角
    /// </summary>
    public Transform leftTop;
    /// <summary>
    /// 右下角
    /// </summary>
    public Transform rightBottom;
    public string folderName;
    public bool canWork = false;
    /// <summary>
    /// 自定义宽高
    /// </summary>
    public Vector2 manualSize;
    public bool showHelp = true;

    /// <summary>
    /// 工作中
    /// </summary>
    private bool working = false;
    private string realFolder = "";
    private float originaltimescaleTime;
    /// <summary>
    /// 结束了
    /// </summary>
    private bool over = false;
    private int currentIndex;
    private string folder = "";

    private int left, top, right, bottom;

    public void Start()
    {
        showHelp = true;
        canWork = false;
        working = false;
        over = false;
    }

    private void DoWork()
    {
        if (working)
        {
            return;
        }
        Debug.LogError("NewWork");

        folder = Application.dataPath + "/../Images/";
        realFolder = Path.Combine(folder, folderName);
        if (Directory.Exists(realFolder))
        {
            Directory.Delete(realFolder, true);
        }
        if (!Directory.Exists(realFolder))
        {
            Directory.CreateDirectory(realFolder);
        }

        Time.captureFramerate = frameRate;
        originaltimescaleTime = Time.timeScale;

        ShowSize();

        currentIndex = 0;
        over = false;
        working = true;
    }

    void OnDrawGizmos()
    {
        if (!canWork)
        {
            return;
        }

        Vector3 rightTop = new Vector3(rightBottom.position.x, leftTop.position.y, leftTop.position.z);
        Vector3 leftBottom = new Vector3(leftTop.position.x, rightBottom.position.y, leftTop.position.z);

        Gizmos.DrawLine(leftTop.position, rightTop);
        Gizmos.DrawLine(rightTop, rightBottom.position);
        Gizmos.DrawLine(rightBottom.position, leftBottom);
        Gizmos.DrawLine(leftBottom, leftTop.position);
    }

    private void ShowSize()
    {
        Vector3 p1 = exportCamera.WorldToScreenPoint(leftTop.position);

        if (manualSize.x > 0 && manualSize.y > 0)
        {
            left = (int)(p1.x);
            right = left + (int)manualSize.x - 1;
            top = Screen.height - (int)p1.y;

            Vector3 p2 = new Vector3(right, 0, p1.z);
            bottom = top + (int)manualSize.y - 1;
            p2.y = Screen.height - bottom;
            
            Vector3 p = exportCamera.ScreenToWorldPoint(p2);
            p.z = leftTop.position.z;
            rightBottom.position = p;
        }
        else
        {
            Vector3 p2 = exportCamera.WorldToScreenPoint(rightBottom.position);
            left = (int)(p1.x);
            right = (int)(p2.x);
            top = Screen.height - (int)p1.y;
            bottom = Screen.height - (int)p2.y;
        }

        Debug.LogError(string.Format("宽:{0} 高:{1}", right - left + 1, bottom - top + 1));
    }

    private void OnGUI()
    {
        if (!showHelp)
        {
            return;
        }
        GUILayout.Box("leftTop和rightBottom选定范围"
            + "\n建议打开Gizmos，可以看到范围线"
            + "\nmanualSize可以指定宽高"
            + "\nN键：执行工作"
            + "\nM键：计算调整范围");
    }

    void Update()
    {
        if (!canWork)
        {
            return;
        }

        if (!working)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                DoWork();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                ShowSize();
            }
            return;
        }

        if (!over && currentIndex >= frameCount)
        {
            over = true;
            working = false;
            Debug.LogError("Finish");
            return;
        }

        StartCoroutine(CaptureFrame());
    }

    IEnumerator CaptureFrame()
    {
        Time.timeScale = 0;
        yield return new WaitForEndOfFrame();

        currentIndex++;

        int width = Screen.width;
        int height = Screen.height;

        RenderTexture blackCamRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture whiteCamRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

        exportCamera.targetTexture = blackCamRenderTexture;
        exportCamera.backgroundColor = Color.black;
        exportCamera.Render();
        RenderTexture.active = blackCamRenderTexture;
        Texture2D texb = GetTex2D();

        exportCamera.targetTexture = whiteCamRenderTexture;
        exportCamera.backgroundColor = Color.white;
        exportCamera.Render();
        RenderTexture.active = whiteCamRenderTexture;
        Texture2D texw = GetTex2D();

        if (texw && texb)
        {
            Texture2D outputtex = new Texture2D(right - left + 1, bottom - top + 1, TextureFormat.ARGB32, false);

            for (int y = 0; y < bottom - top + 1; ++y)
            {
                for (int x = 0; x < right - left + 1; ++x)
                {
                    float alpha;
                    alpha = texw.GetPixel(x, y).r - texb.GetPixel(x, y).r;
                    alpha = 1.0f - alpha;
                    Color color;
                    if (alpha == 0)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = texb.GetPixel(x, y);
                    }
                    color.a = alpha;
                    outputtex.SetPixel(x, y, color);
                }
            }

            //SavePng(texw, currentIndex, "0");
            //SavePng(texb, currentIndex, "1");
            SavePng(outputtex, currentIndex, "2");

            exportCamera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(outputtex);
            outputtex = null;
            DestroyImmediate(blackCamRenderTexture);
            blackCamRenderTexture = null;
            DestroyImmediate(whiteCamRenderTexture);
            whiteCamRenderTexture = null;
            DestroyImmediate(texb);
            texb = null;
            DestroyImmediate(texw);
            texb = null;

            System.GC.Collect();

            Time.timeScale = originaltimescaleTime;
        }
    }

    private void SavePng(Texture2D tex, int idx, string tag = "")
    {
        string filename = String.Format("{0}/{1:D04}_{2}.png", realFolder, idx, tag);
        byte[] pngShot = tex.EncodeToPNG();
        File.WriteAllBytes(filename, pngShot);
        pngShot = null;
    }

    private Texture2D GetTex2D()
    {
        int width = right - left + 1;
        int height = bottom - top + 1;
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(left, top, width, height), 0, 0);
        tex.Apply();
        return tex;
    }
}