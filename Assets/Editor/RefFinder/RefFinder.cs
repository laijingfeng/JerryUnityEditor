using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RefFinder : EditorWindow
{
    [MenuItem("Window/RefFinder")]
    private static void Open()
    {
        RefFinder win = GetWindow<RefFinder>();
        win.DoReset();
    }

    #region Commom

    private Object findObject = null;

    private string[] toolbarTexts = { "FromPath", "FromObject" };
    private int toolbarIdx = 0;
    private List<Finder_Base> finderList = new List<Finder_Base>();

    private void DoReset()
    {
        ResetPathFind();
        ResetObjectFind();

        finderList.Clear();
        finderList.Add(new Finder_Sprite());
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        if (GUILayout.Button(new GUIContent() { text = "Reset", tooltip = "如果异常了，可尝试重置" }))
        {
            DoReset();
        }

        findObject = EditorGUILayout.ObjectField("查找对象", findObject, typeof(Object), true);

        toolbarIdx = GUILayout.Toolbar(toolbarIdx, toolbarTexts);
        switch (toolbarIdx)
        {
            case 0:
                {
                    DrawPathFind();
                }
                break;
            case 1:
                {
                    DrawObjectFind();
                }
                break;
        }
        EditorGUILayout.EndVertical();

    }

    private void SetTip(string info, MessageType type)
    {
        switch (toolbarIdx)
        {
            case 0:
                {
                    pathFindTip = info;
                    pathFindTipType = type;
                }
                break;
            case 1:
                {
                    objectFindTip = info;
                    objectFindTipType = type;
                }
                break;
        }
    }

    private bool CommonCheck()
    {
        if (findObject == null)
        {
            SetTip("查找对象不能为空", MessageType.Warning);
            return false;
        }

        return true;
    }

    #endregion Commom

    #region PathFind

    private string findPath = "Assets";
    private Rect pathRect;
    private string pathFindTip = "设定对象和目录，进行引用查找";
    private MessageType pathFindTipType = MessageType.Info;
    private Finder_Base pathFinder = null;

    private void ResetPathFind()
    {
        pathFindTip = "设定对象和目录，进行引用查找";
        pathFindTipType = MessageType.Info;
        pathFinder = null;
    }

    private void DrawPathFind()
    {
        pathRect = EditorGUILayout.GetControlRect();
        findPath = EditorGUI.TextField(pathRect, "查找目录", findPath);
        if ((Event.current.type == EventType.dragUpdated || Event.current.type == EventType.DragExited) &&
            pathRect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null
                && DragAndDrop.paths.Length > 0
                && Directory.Exists(DragAndDrop.paths[0]))
            {
                findPath = DragAndDrop.paths[0];
            }
        }

        if (GUILayout.Button("Find"))
        {
            DoPathFind();
        }

        EditorGUILayout.BeginVertical();
        if (pathFinder != null)
        {
            pathFindTip = pathFinder.pathFindTip;
            pathFindTipType = pathFinder.pathFindTipType;
        }
        EditorGUILayout.HelpBox(pathFindTip, pathFindTipType, true);
        EditorGUILayout.EndVertical();

        if (pathFinder != null && pathFinder.pathFindResults.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            foreach (Object obj in pathFinder.pathFindResults)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void DoPathFind()
    {
        pathFinder = null;

        if (CommonCheck() == false)
        {
            return;
        }

        string findPathAbs = Application.dataPath + "/../" + findPath;

        if (Directory.Exists(findPathAbs) == false)
        {
            SetTip("查找目录不存在", MessageType.Warning);
            return;
        }

        bool match = false;
        foreach (Finder_Base finder in finderList)
        {
            if (finder.Match(findObject.GetType()))
            {
                match = true;
                finder.PathFind(findObject, findPath);
                pathFinder = finder;
                break;
            }
        }

        if (!match)
        {
            SetTip("查找对象是不支持的类型", MessageType.Warning);
        }
    }

    #endregion PathFind

    #region ObjectFind

    private Object objectTarget = null;
    private string objectFindTip = "设定对象和目标，进行引用查找";
    private MessageType objectFindTipType = MessageType.Info;
    private Finder_Base objectFinder = null;

    private void ResetObjectFind()
    {
        objectFindTip = "设定对象和目标，进行引用查找";
        objectFindTipType = MessageType.Info;
        objectFinder = null;
    }

    private void DrawObjectFind()
    {
        objectTarget = EditorGUILayout.ObjectField("目标对象", objectTarget, typeof(Object), true);
        if (GUILayout.Button("Find"))
        {
            DoObjectFind();
        }
        EditorGUILayout.BeginVertical();
        if (objectFinder != null)
        {
            objectFindTip = objectFinder.objectFindTip;
            objectFindTipType = objectFinder.objectFindTipType;
        }
        EditorGUILayout.HelpBox(objectFindTip, objectFindTipType, true);
        EditorGUILayout.EndVertical();

        if (objectFinder != null && objectFinder.objectFindResults.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            foreach (Object obj in objectFinder.objectFindResults)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void DoObjectFind()
    {
        objectFinder = null;

        if (CommonCheck() == false)
        {
            return;
        }

        if (objectTarget == null)
        {
            SetTip("目标对象不能为空", MessageType.Warning);
            return;
        }

        bool match = false;
        foreach (Finder_Base finder in finderList)
        {
            if (finder.Match(findObject.GetType()))
            {
                match = true;
                finder.ObjectFind(findObject, objectTarget);
                objectFinder = finder;
                break;
            }
        }

        if (!match)
        {
            SetTip("查找对象是不支持的类型", MessageType.Warning);
        }
    }

    #endregion ObjectFind
}