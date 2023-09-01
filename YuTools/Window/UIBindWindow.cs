using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class UIBindWindow : EditorWindow
{
    //选中的预制体
    public static Object go;
    //存放路径
    public static string viewDir;
    Rect rect;

    /// <summary>
    /// 窗口启动时
    /// </summary>
    private void Awake()
    {
        go = Selection.activeGameObject;
        viewDir = null;
    }

    /// <summary>
    /// 绘制窗口
    /// </summary>
    private void OnGUI()
    {
        EditorGUILayout.LabelField("选择UI预制体");
        go = EditorGUILayout.ObjectField(go, typeof(GameObject), false);

        EditorGUILayout.LabelField("拖入生成脚本存放的文件夹");
        rect = EditorGUILayout.GetControlRect(GUILayout.Width(500));
        viewDir = EditorGUI.TextField(rect, viewDir);
        //改变鼠标状态
        if ((Event.current.type == EventType.DragUpdated) && rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }
        //鼠标在拖拽结束时，并且鼠标所在位置在文本输入框内
        if ((Event.current.type == EventType.DragExited)
          && rect.Contains(Event.current.mousePosition))
        {
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                if (Directory.Exists(DragAndDrop.paths[0]))
                {
                    viewDir = DragAndDrop.paths[0];
                }
            }
        }

        //生成按钮
        if (GUILayout.Button("生成"))
        {
            UIViewBind.BindUIView();
        }
    }
}
