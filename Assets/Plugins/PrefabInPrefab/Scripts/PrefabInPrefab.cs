using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PrefabInPrefab : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab = null;
    private GameObject generatedObject = null;
    private Action<GameObject> _getChildCallback = null;

    public static void GetChild(Transform tf, Action<GameObject> callback)
    {
        if (tf == null)
        {
            callback(null);
            return;
        }
        PrefabInPrefab pip = tf.GetComponent<PrefabInPrefab>();
        if (pip == null)
        {
            callback(null);
            return;
        }
        if (pip.generatedObject != null)
        {
            callback(pip.generatedObject);
        }
        else
        {
            pip._getChildCallback += callback;
        }
    }

    void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            StartInEditMode();
            return;
        }
#endif
        
        InstantiatePrefab();
        if (this.generatedObject != null && this._getChildCallback != null)
        {
            this._getChildCallback(this.generatedObject);
            this._getChildCallback = null;
        }
    }

    private void InstantiatePrefab()
    {
        if (prefab == null)
        {
            return;
        }

        generatedObject = Instantiate(prefab) as GameObject;

        generatedObject.transform.SetParent(this.transform);
        if(generatedObject.transform.parent != null)
        {
            generatedObject.name = generatedObject.transform.parent.name;
        }
        generatedObject.transform.localPosition = Vector3.zero;
        generatedObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        generatedObject.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Prefab名称，给工具用
    /// </summary>
    /// <returns></returns>
    public string PrefabName()
    {
        if (prefab == null)
        {
            return "";
        }
        return prefab.name;
    }

#if UNITY_EDITOR

    private string lastPrefabUpdateTime;

    private bool visibleVirtualPrefab
    {
        get { return this.gameObject.activeInHierarchy && this.enabled; }
    }

    void StartInEditMode()
    {
        DrawDontEditablePrefab();
    }

    void OnEnable()
    {
        ForceDrawDontEditablePrefab();
    }

    void OnDisable()
    {
        DrawDontEditablePrefab(true);
    }

    public void ForceDrawDontEditablePrefab()
    {
        lastPrefabUpdateTime = "";
        DrawDontEditablePrefab();
    }

    public void DrawDontEditablePrefab(bool start = false)
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (prefab == null || !visibleVirtualPrefab)
        {
            if (Child != null)
            {
                // param changed
                DeleteChildren(start);
                UpdateGameView();
            }
            return;
        }

        if (!PrefabUpdated())
        {
            return;
        }
        if (ValidationError())
        {
            return;
        }

        DeleteChildren(start);

        if (PrefabUtility.GetPrefabType(this.gameObject) != PrefabType.PrefabInstance)
        {
            UpdateGameView();
            return;
        }

        InstantiatePrefab();
        
        // 自分の1つ上のGameObjectが所属しているPrefabのRootの親の下.
        var foundRoot = PrefabUtility.FindPrefabRoot(transform.parent.gameObject).transform.parent;
        if (foundRoot == null)
        {
            // 親オブジェクトは、ドラッグアンドドロップした瞬間は見つからない
            EditorApplication.delayCall += () =>
            {
                if (generatedObject == null) { return; }
                var parent = PrefabUtility.FindPrefabRoot(transform.parent.gameObject).transform.parent;
                generatedObject.transform.SetParent(parent == null ? null : parent.transform);
            };
        }
        else
        {
            generatedObject.transform.SetParent(foundRoot.transform);
        }
        generatedObject.name = string.Format(">PrefabInPrefab{0}", GetInstanceID());
        generatedObject.tag = "EditorOnly";
        foreach (var childTransform in generatedObject.GetComponentsInChildren<Transform>())
        {
            childTransform.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        var child = generatedObject.AddComponent<PrefabInPrefabAsset.VirtualPrefab>();
        child.stepparent = this.gameObject;
        child.original = this;
        child.UpdateTransform();

        UpdateGameView();
    }

    private bool PrefabUpdated()
    {
        var prefabUpdateTime = GetPrefabUpdateTime();
        if (lastPrefabUpdateTime == prefabUpdateTime && Child != null)
        {
            return false;
        }
        lastPrefabUpdateTime = prefabUpdateTime;
        return true;
    }

    /// <summary>
    /// 删除子节点
    /// </summary>
    private void DeleteChildren(bool start = false)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EditorOnly"))
        {
            if (obj.name != string.Format(">PrefabInPrefab{0}", GetInstanceID()))
            {
                continue;
            }
            if (start == false)
            {
                DestroyImmediate(obj);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }

    private string GetPrefabFilePath()
    {
        return AssetDatabase.GetAssetPath(prefab);
    }

    private string GetPrefabUpdateTime()
    {
        string result;
        using (System.IO.FileStream fs = new System.IO.FileStream(GetPrefabFilePath(), System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            using (System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] bs = sha1.ComputeHash(fs);
                result = BitConverter.ToString(bs).ToLower().Replace("-", "");
            }
        }

        return result;
    }

    /// <summary>
    /// 更新视图
    /// </summary>
    private void UpdateGameView()
    {
        if (Application.isPlaying)
        {
            return;
        }
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        SceneView.RepaintAll();

        // force redraw anything(ex. NGUI's UICamera)
        GameObject dummy = new GameObject();
        dummy.transform.parent = null;
        DestroyImmediate(dummy);
    }

    private bool ValidationError()
    {
        // check circular reference
        if (CheckCircularReference(this, null))
        {
            Debug.LogError("Can't circular reference.");
            Reset();
            return true;
        }

        // This game object can't be root.
        // Because this is not in prefab.
        if (this.transform.parent == null)
        {
            // copy&paseした時に、なぜか一瞬だけparentがnullになるので、
            // 少し遅らせる.
            EditorApplication.delayCall += () =>
            {
                if (this.transform.parent == null)
                {
                    Debug.LogError("Can't attach PrefabInPrefab to root gameobject.");
                    Reset();
                }
                else
                {
                    ForceDrawDontEditablePrefab();
                }
            };

            //stop
            return true;
        }

        return false;
    }

    void Reset()
    {
        prefab = null;
        DeleteChildren();
    }

    bool CheckCircularReference(PrefabInPrefab target, List<int> usedPrefabs)
    {
        if (target.prefab == null)
        {
            return false;
        }

        if (usedPrefabs == null)
        {
            usedPrefabs = new List<int>();
        }

        int id = target.prefab.GetInstanceID();
        if (usedPrefabs.Contains(id))
        {
            return true;
        }
        usedPrefabs.Add(id);

        foreach (var nextTarget in ((GameObject)target.prefab).GetComponentsInChildren<PrefabInPrefab>(true))
        {
            if (nextTarget == this)
            {
                continue;
            }
            if (CheckCircularReference(nextTarget, new List<int>(usedPrefabs)))
            {
                return true;
            }
        }

        return false;
    }

    public GameObject Child
    {
        get
        {
            if (Application.isPlaying && generatedObject == null && prefab != null)
            {
                Debug.LogError("Prefab In Prefab is Uninitialized. You can use this after Awake().");
            }
            return generatedObject;
        }
    }

#endif
}