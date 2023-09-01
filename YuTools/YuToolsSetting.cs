using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI组件绑定设置
/// </summary>
public class UIBindSetting
{
    //view层代码路径
    public static string ViewDir = Application.dataPath + "/GameAssets/Scripts/Framework/Game/Module";
    //view层模版文件路径
    public static string ViewTempletePath = Application.dataPath + "/GameAssets/DevTool/Editor/Common/YuTools/Template/UIBindTemplete.txt";
    //命名空间模板
    public static string NameSpaceTemplete = "using {0};";
    //字段模板
    public static string FieldTemplete = "public {0} {1};";
    //组件赋值模板
    public static string MethodTemplete = "{0} = ViewRoot.FindByType<{1}>(\"{2}\");";
    //GO赋值模板
    public static string gameObjectMethodTemplete = "{0} = ViewRoot.FindByPath(\"{1}\");";
    //命名规则对应组件
    public static Dictionary<string, string> DicNameSpace = new Dictionary<string, string>(16)
    {
        //["go_"] = "GameObject"    需要控制的GameObject用go_开头
        ["btn_"] = "EventAdaptor",
        ["button_"] = "EventAdaptor",
        ["tx_"] = "LanguageText",
        ["text_"] = "LanguageText",
        ["img_"] = "Image",
        ["image_"] = "Image",
        ["go_"] = "GameObject",
        ["tran_"] = "Transform",
        ["rect_"] = "RectTransform",
        ["sr_"] = "ScrollRect",
        ["scrollrect_"] = "ScrollRect",
        ["sld_"] = "Slider",
        ["slider_"] = "Slider",
        ["dl_"] = "Dropdown",
    };
}
