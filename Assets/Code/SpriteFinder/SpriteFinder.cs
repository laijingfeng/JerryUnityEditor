#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFinder : MonoBehaviour
{
    private Image img;
    public Transform prefab;
    public string configPath = "/../Assets/Resources/Prefabs/UI/";

    [ContextMenu("进行图片查找")]
    public void DoFinder()
    {
        img = this.gameObject.GetComponent<Image>();
        if (img == null || img.sprite == null)
        {
            Debug.Log("查找Image或Sprite不存在");
            return;
        }

        string imgPath = AssetDatabase.GetAssetPath(img.sprite);
        string imgGuid = AssetDatabase.AssetPathToGUID(imgPath);
        string spriteName = img.sprite.name;
        string spriteID = "";

        Debug.Log("将查找:<color=green>" + spriteName + "</color>");

        Match mt = Regex.Match(File.ReadAllText(imgPath + ".meta"), @"(\d+): " + spriteName, RegexOptions.Singleline);
        if (mt != null)
        {
            spriteID = mt.Value;
            spriteID = spriteID.Split(':')[0];
        }
        else
        {
            Debug.Log("查找异常");
            return;
        }

        //Debug.Log(spriteName + "\npath:" + imgPath + "\nguid:" + imgGuid + "\nspriteID:" + spriteID);

        string findPath = Application.dataPath + configPath;
        if (Directory.Exists(findPath) == false)
        {
            Debug.Log("查找目录不存在");
            return;
        }

        string[] files = Directory.GetFiles(findPath, "*.*", SearchOption.AllDirectories)
            .Where(s => Path.GetExtension(s).ToLower().Equals(".prefab")).ToArray();

        Debug.Log("查找结果如下（Prefab拖到Hierarchy并指定到工具的Prefab可以查详情）：");

        if (files == null || files.Length <= 0)
        {
            Debug.LogWarning("查找目录没有Prefab");
            return;
        }

        foreach (string f in files)
        {
            if (Regex.IsMatch(File.ReadAllText(f), @"m_Sprite: {fileID: " + spriteID + ", guid: " + imgGuid + ", type: 3}"))
            {
                Debug.LogFormat(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(f)), "<color=yellow>{0}</color>", Path.GetFileName(f));
            }
        }

        Debug.Log("查找结束");
    }

    [ContextMenu("查找具体节点")]
    public void DoFinderDetail()
    {
        if (prefab == null)
        {
            Debug.Log("请先指定Prefab");
            return;
        }

        img = this.gameObject.GetComponent<Image>();
        if (img == null || img.sprite == null)
        {
            Debug.Log("查找Image或Sprite不存在");
            return;
        }

        string imgPath = AssetDatabase.GetAssetPath(img.sprite);
        string imgGuid = AssetDatabase.AssetPathToGUID(imgPath);
        string spriteName = img.sprite.name;

        Debug.Log("节点查找结果如下：");

        Image[] imgs = prefab.GetComponentsInChildren<Image>(true);
        foreach (Image im in imgs)
        {
            if (im == null || im.sprite == null || im.sprite.name.Equals(spriteName) == false)
            {
                continue;
            }
            if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(im.sprite)).Equals(imgGuid))
            {
                Debug.LogFormat(im, "<color=red>节点：{0}</color>", im.name);
            }
        }

        Debug.Log("查找结束");
    }

    private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}
#endif