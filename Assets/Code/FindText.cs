using UnityEngine;
using UnityEngine.UI;

public class FindText : MonoBehaviour
{
    public string Content;

    [ContextMenu("Do")]
    private void Do()
    {
        Text[] texts = this.transform.GetComponentsInChildren<Text>(true);
        foreach (Text t in texts)
        {
            if (t.text.Contains(Content))
            {
                Debug.LogWarning("find:" + EditorUtil.GetTransformHieraichyPath(t.transform), t);
            }
        }
        Debug.LogWarning("find finish");
    }
}