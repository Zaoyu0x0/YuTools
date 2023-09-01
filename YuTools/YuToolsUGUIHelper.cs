using UnityEngine;
using UnityEditor;
using System.Linq;
using Leyo.FirmWare;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class YuToolsUIPrefabHelper : AssetModificationProcessor
{
    static YuToolsUIPrefabHelper()
    {
        //PrefabUtility.prefabInstanceUpdated -= PrefabInstanceUpdated;
        //PrefabUtility.prefabInstanceUpdated += PrefabInstanceUpdated;
        PrefabStage.prefabSaving -= PrefabStageUpdated;
        PrefabStage.prefabSaving += PrefabStageUpdated;
    }


    static void PrefabInstanceUpdated(GameObject instance)
    {
        //TODO:关闭Raycast Target
        var graphics = instance.GetComponentsInChildren<Graphic>().ToList();
        if (graphics.Count <= 0)
        {
            return;
        }
        foreach (var graphic in graphics)
        {
            //Is Button
            if (graphic.GetComponent<EventAdaptor>() != null)
            {
                continue;
            }
            graphic.raycastTarget = false;
        }
        graphics.Clear();
    }
    static void PrefabStageUpdated(GameObject instance)
    {
        //TODO:关闭Raycast Target
        var graphics = instance.GetComponentsInChildren<Graphic>().ToList();
        if (graphics.Count <= 0)
        {
            return;
        }
        foreach (var graphic in graphics)
        {
            switch (graphic)
            {
                case Image image:
                    if (graphic.name.ToLower().Contains("bg") || graphic.GetComponent<Button>() != null)
                    {
                        image.raycastTarget = true;
                    }
                    else
                    {
                        image.raycastTarget = false;
                    }
                    break;
                default:
                    graphic.raycastTarget = false;
                    break;
            }
        }
        graphics.Clear();
    }
}

