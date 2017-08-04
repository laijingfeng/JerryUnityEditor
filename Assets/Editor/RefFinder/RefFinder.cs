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

    public enum FindFromType
    {
        FromPath = 0,
        FromObject,
        FromCurScene,
    }

    private Object findObject = null;

    private string[] toolbarTexts = 
    { 
        FindFromType.FromPath.ToString(),
        FindFromType.FromObject.ToString(),
        FindFromType.FromCurScene.ToString(),
    };

    private int toolbarIdx = 0;
    private List<Finder_Base> finderList = new List<Finder_Base>();

    private void DoReset()
    {
        ResetPathFind();
        ResetObjectFind();
        ResetCurSceneFind();

        finderList.Clear();
        finderList.Add(new Finder_Sprite());
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(300));

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
            case 2:
                {
                    DrawCurSceneFind();
                }
                break;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        if (GUILayout.Button(new GUIContent() { text = "Reset", tooltip = "如果异常了，可尝试重置" }))
        {
            DoReset();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private void SetTip(string info, MessageType type)
    {
        switch (toolbarIdx)
        {
            case 0:
                {
                    pathFindSet.SetTip(info, type);
                }
                break;
            case 1:
                {
                    objectFindSet.SetTip(info, type);
                }
                break;
            case 2:
                {
                    curSceneFindSet.SetTip(info, type);
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

    public class CommonSet
    {
        public FindFromType type = FindFromType.FromPath;
        public Finder_Base finder = null;
        public string findTip = "设定对象和目录，进行引用查找";
        public MessageType findTipType = MessageType.Info;

        public CommonSet(FindFromType fromType)
        {
            type = fromType;
        }

        public void SetTip(string info, MessageType type)
        {
            findTip = info;
            findTipType = type;
        }

        public void RefreshFinderTip()
        {
            if (finder != null)
            {
                switch (type)
                {
                    case FindFromType.FromPath:
                        {
                            findTip = finder.pathFindTip;
                            findTipType = finder.pathFindTipType;
                        }
                        break;
                    case FindFromType.FromObject:
                        {
                            findTip = finder.objectFindTip;
                            findTipType = finder.objectFindTipType;
                        }
                        break;
                    case FindFromType.FromCurScene:
                        {
                            findTip = finder.curSceneFindTip;
                            findTipType = finder.curSceneFindTipType;
                        }
                        break;
                }
            }
        }

        public void DrawTip()
        {
            EditorGUILayout.HelpBox(findTip, findTipType, true);
        }

        public void DrawResult()
        {
            if (finder != null)
            {
                switch (type)
                {
                    case FindFromType.FromPath:
                        {
                            finder.DrawPathFind();
                        }
                        break;
                    case FindFromType.FromObject:
                        {
                            finder.DrawObjectFind();
                        }
                        break;
                    case FindFromType.FromCurScene:
                        {
                            finder.DrawCurSceneFind();
                        }
                        break;
                }
            }
        }
    }

    #endregion Commom

    #region PathFind

    private string findPath = "Assets";
    private Rect pathRect;
    private CommonSet pathFindSet = new CommonSet(FindFromType.FromPath);

    private void ResetPathFind()
    {
        pathFindSet.finder = null;
        pathFindSet.findTip = "设定对象和目录，进行引用查找";
        pathFindSet.findTipType = MessageType.Info;
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
        pathFindSet.RefreshFinderTip();
        pathFindSet.DrawTip();
        EditorGUILayout.EndVertical();
        pathFindSet.DrawResult();
    }

    private void DoPathFind()
    {
        pathFindSet.finder = null;

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
                pathFindSet.finder = finder;
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
    private CommonSet objectFindSet = new CommonSet(FindFromType.FromObject);

    private void ResetObjectFind()
    {
        objectFindSet.finder = null;
        objectFindSet.findTip = "设定对象和目标，进行引用查找";
        objectFindSet.findTipType = MessageType.Info;
    }

    private void DrawObjectFind()
    {
        objectTarget = EditorGUILayout.ObjectField("目标对象", objectTarget, typeof(Object), true);
        if (GUILayout.Button("Find"))
        {
            DoObjectFind();
        }
        EditorGUILayout.BeginVertical();
        objectFindSet.RefreshFinderTip();
        objectFindSet.DrawTip();
        EditorGUILayout.EndVertical();
        objectFindSet.DrawResult();
    }

    private void DoObjectFind()
    {
        objectFindSet.finder = null;

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
                objectFindSet.finder = finder;
                break;
            }
        }

        if (!match)
        {
            SetTip("查找对象是不支持的类型", MessageType.Warning);
        }
    }

    #endregion ObjectFind

    #region CurSceneFind

    private CommonSet curSceneFindSet = new CommonSet(FindFromType.FromCurScene);

    private void ResetCurSceneFind()
    {
        curSceneFindSet.finder = null;
        curSceneFindSet.findTip = "在当前场景显示对象中查找";
        curSceneFindSet.findTipType = MessageType.Info;
    }

    private void DrawCurSceneFind()
    {
        if (GUILayout.Button("Find"))
        {
            DoCurSceneFind();
        }
        EditorGUILayout.BeginVertical();
        curSceneFindSet.RefreshFinderTip();
        curSceneFindSet.DrawTip();
        EditorGUILayout.EndVertical();
        curSceneFindSet.DrawResult();
    }

    private void DoCurSceneFind()
    {
        curSceneFindSet.finder = null;

        if (CommonCheck() == false)
        {
            return;
        }

        bool match = false;
        foreach (Finder_Base finder in finderList)
        {
            if (finder.Match(findObject.GetType()))
            {
                match = true;
                finder.CurSceneFind(findObject);
                curSceneFindSet.finder = finder;
                break;
            }
        }

        if (!match)
        {
            SetTip("查找对象是不支持的类型", MessageType.Warning);
        }
    }

    #endregion CurSceneFind
}