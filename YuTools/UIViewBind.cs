using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class UIViewBind
{
    static GameObject selected;
    public static string viewDir;

    /// <summary>
    /// 忽略的组件类型列表
    /// </summary>
    static List<Type> IgnoreComponentTypeList = new List<Type>()
    {
        typeof(CanvasRenderer),
        typeof(RectTransform),
    };

    [MenuItem("Assets/YuTools/BindUI &#Y", false, 0)]
    static void ShowBindUIWindow()
    {
        Rect rect = new Rect(0, 0, 600, 200);
        UIBindWindow uIBindWindow = EditorWindow.GetWindowWithRect<UIBindWindow>(rect, true, "生成UI绑定脚本");
        uIBindWindow.Show();
    }

    /// <summary>
    /// 开始绑定
    /// </summary>
    public static void BindUIView()
    {
        viewDir = UIBindWindow.viewDir == null ? UIBindSetting.ViewDir : UIBindWindow.viewDir;
        GameObject go = UIBindWindow.go as GameObject;
        selected = go;
        if (go == null)
        {
            EditorUtility.DisplayDialog("无法生成", "未选中文件", "确定");
            return;
        }
        //Component view = go.GetComponent(typeof(View));
        string className = go.name;
        string viewPath = viewDir + "/" + className + ".cs";
        if (!className.StartsWith("UI"))
        {
            EditorUtility.DisplayDialog("无法生成", "不是View预制体", "确定");
            return;
        }
        if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.Regular)
        {
            EditorUtility.DisplayDialog("无法生成", "不是预制体", "确定");
            return;
        }
        if (!Directory.Exists(viewDir))
        {
            Directory.CreateDirectory(viewDir);
        }
        CreateBindUICode(className, ".cs");
        AssetDatabase.Refresh();
    }

    static void CreateBindUICode(string className, string fileType)
    {

        // if (File.Exists(viewDir + "/" + className + fileType))
        // {
        //     ULogger.Error(2, "已经存在该脚本！");
        //     return;
        // }
        var allSetting = Directory.GetFiles(Application.dataPath, className + fileType, SearchOption.AllDirectories);
        if (allSetting.Length > 0)
        {
            EditorUtility.DisplayDialog("无法生成", "已存在同名脚本", "确定");
            return;
        }

        FileStream fileStream = File.Create(viewDir + "/" + className + fileType);

        string viewTempleteContent = File.ReadAllText(UIBindSetting.ViewTempletePath, Encoding.UTF8);
        fileStream.Close();
        fileStream.Dispose();

        StringBuilder fieldStr = new StringBuilder();
        StringBuilder methodStr = new StringBuilder();
        StringBuilder nameSpaceStr = new StringBuilder();
        nameSpaceStr.AppendLine(UIBindSetting.NameSpaceTemplete.Replace("{0}", "UnityEngine"));
        nameSpaceStr.AppendLine(UIBindSetting.NameSpaceTemplete.Replace("{0}", "Leyo.Framework"));

        List<string> tempNameSpaceList = new List<string>();
        tempNameSpaceList.Add("UnityEngine");
        tempNameSpaceList.Add("Leyo.Framework");
        List<ComponentInfo> infoList = new List<ComponentInfo>();
        GetAllUINode(null, selected.transform, infoList);

        foreach (var info in infoList)
        {
            //字段
            string tempFieldStrStr = UIBindSetting.FieldTemplete.Replace("{0}", info.TypeStr);
            tempFieldStrStr = tempFieldStrStr.Replace("{1}", info.FieldStrName);
            fieldStr.AppendLine("        " + tempFieldStrStr);

            //绑定
            SetMethod(methodStr, info);

            //命名空间
            SetNameSpace(nameSpaceStr, tempNameSpaceList, info);
        }
        //模板tag替换字符
        using (StreamWriter sw = new StreamWriter(viewDir + "/" + className + fileType))
        {
            string content = viewTempleteContent;
            content = content.Replace("#NAMESPACE#", nameSpaceStr.ToString());
            content = content.Replace("#CLASSNAME#", className);
            content = content.Replace("#FIELD_BIND#", fieldStr.ToString());
            content = content.Replace("#METHOD_BIND#", methodStr.ToString());
            sw.Write(content);
            sw.Close();
        }
    }

    /// <summary>
    /// 设置绑定文本
    /// </summary>
    /// <param name="methodStr"></param>
    /// <param name="info"></param>
    static void SetMethod(StringBuilder methodStr, ComponentInfo info)
    {
        string tempMethodStr;
        switch (info.TypeStr)
        {
            case "GameObject":
                tempMethodStr = UIBindSetting.gameObjectMethodTemplete.Replace("{0}", info.FieldStrName);
                tempMethodStr = tempMethodStr.Replace("{1}", info.Path);
                methodStr.AppendLine("            " + tempMethodStr);
                break;
            default:
                tempMethodStr = UIBindSetting.MethodTemplete.Replace("{0}", info.FieldStrName);
                tempMethodStr = tempMethodStr.Replace("{1}", info.TypeStr);
                tempMethodStr = tempMethodStr.Replace("{2}", info.Path);
                methodStr.AppendLine("            " + tempMethodStr);
                break;
        }
    }
    /// <summary>
    /// 设置命名空间
    /// </summary>
    /// <param name="content"></param>
    static void SetNameSpace(StringBuilder nameSpaceStr, List<string> content, ComponentInfo info)
    {
        if (!content.Contains(info.NameSpace) && !string.IsNullOrEmpty(info.NameSpace))
        {
            string newStr = UIBindSetting.NameSpaceTemplete.Replace("{0}", info.NameSpace);
            nameSpaceStr.AppendLine(newStr);
            content.Add(info.NameSpace);
        }
    }

    /// <summary>
    /// 递归获取所有子物体
    /// </summary>
    /// <param name="path"></param>
    /// <param name="tran"></param>
    /// <param name="infoList"></param>
    static void GetAllUINode(string path, Transform tran, List<ComponentInfo> infoList)
    {
        for (int i = 0; i < tran.childCount; i++)
        {
            var child = tran.GetChild(i);
            var childPath = string.IsNullOrEmpty(path) ? child.name : path + "/" + child.name;
            if (CheckName(child.name))
            {
                ComponentInfo info = new ComponentInfo()
                {
                    Path = childPath,
                    TypeStr = GetTypeName(child.name),
                    go = child.gameObject,
                };
                info.NameSpace = GetNameSpace(info.TypeStr, child);
                if (!IsSameComponent(info.FieldStrName, infoList))
                {
                    infoList.Add(info);
                }
            }
            GetAllUINode(childPath, child, infoList);
        }
    }
    /// <summary>
    /// 根据规范命名来获取子对象
    /// </summary>
    static bool CheckName(string childname)
    {
        if (childname.Contains("_"))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 是否有同名
    /// </summary>
    /// <param name="childname"></param>
    /// <param name="infoList"></param>
    /// <returns></returns>
    static bool IsSameComponent(string childname, List<ComponentInfo> infoList)
    {
        foreach (var info in infoList)
        {
            if (info.FieldStrName == childname)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 获取物体组件类型名称
    /// </summary>
    /// <param name="childname"></param>
    /// <returns></returns>
    static string GetTypeName(string childname)
    {
        string typeName = "GameObject";
        foreach (var item in UIBindSetting.DicNameSpace)
        {
            if (childname.ToLower().StartsWith(item.Key))
            {
                typeName = item.Value;
            }
        }
        return typeName;
    }
    /// <summary>
    /// 获取所需命名空间
    /// </summary>
    /// <param name="type"></param>
    /// <param name="tran"></param>
    /// <returns></returns>
    static string GetNameSpace(string type, Transform tran)
    {
        Component[] components = tran.GetComponents(typeof(Component));
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].GetType().Name == type)
            {
                return components[i].GetType().Namespace;
            }
        }
        return "UnityEngine";
    }
}
/// <summary>
/// 组件信息
/// </summary>
public class ComponentInfo
{
    public string Path;
    public string TypeStr;
    public string NameSpace;
    public GameObject go;
    public string FieldStrName
    {
        get
        {
            return go.name;
        }
    }
}
