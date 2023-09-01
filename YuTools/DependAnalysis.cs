using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DependAnalysis : EditorWindow
{
    private static Object[] _targetObjects;
    private bool[] _foldoutArr;
    private Object[][] _beDependArr;
    private static int _targetCount;
    private Vector2 _scrollPos;
    string[] _withoutExtensions = new string[] { ".prefab", ".unity", ".mat", ".asset", ".controller" };

    [MenuItem("Assets/YuTools/FindReferences &#F", false, 1)]
    static void FindReferences()
    {
        _targetObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);
        _targetCount = _targetObjects == null ? 0 : _targetObjects.Length;
        if (_targetCount == 0) return;
        DependAnalysis window = GetWindow<DependAnalysis>("依赖分析");
        window.Init();
        window.Show();
    }
    void Init()
    {
        _beDependArr = new Object[_targetCount][];
        _foldoutArr = new bool[_targetCount];
        EditorStyles.foldout.richText = true;
        for (int i = 0; i < _targetCount; i++) _beDependArr[i] = GetBeDepend(_targetObjects[i]);
    }

    [System.Obsolete]
    private void OnGUI()
    {
        if (_beDependArr.Length != _targetCount) return;
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        Object[] objArr;
        int count;
        string objName;
        for (int i = 0; i < _targetCount; i++)
        {
            objArr = _beDependArr[i];
            count = objArr == null ? 0 : objArr.Length;
            objName = Path.GetFileName(AssetDatabase.GetAssetPath(_targetObjects[i]));
            string info = count == 0
                ? $"<color=yellow>{objName}【{count}】</color>"
                : $"{objName}【{count}】";
            _foldoutArr[i] = EditorGUILayout.Foldout(_foldoutArr[i], info);
            if (_foldoutArr[i])
            {
                if (count > 0)
                {
                    foreach (var obj in objArr)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        EditorGUILayout.ObjectField(obj, typeof(Object));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    EditorGUILayout.LabelField("【Null】");
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
    private Object[] GetBeDepend(Object target)
    {
        if (target == null) return null;
        string path = AssetDatabase.GetAssetPath(target);
        if (string.IsNullOrEmpty(path)) return null;
        string guid = AssetDatabase.AssetPathToGUID(path);
        string[] files = Directory.GetFiles(Application.dataPath, "*",
            SearchOption.AllDirectories).Where(s => _withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        List<Object> objects = new List<Object>();
        foreach (var file in files)
        {
            string assetPath = file.Replace(Application.dataPath, "");
            assetPath = "Assets" + assetPath;
            string readText = File.ReadAllText(file);

            if (!readText.StartsWith("%YAML"))
            {
                var depends = AssetDatabase.GetDependencies(assetPath, false);
                if (depends != null)
                {
                    foreach (var dep in depends)
                    {
                        if (dep == path)
                        {
                            objects.Add(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
                            break;
                        }
                    }
                }
            }
            else if (Regex.IsMatch(readText, guid)) objects.Add(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
        }
        return objects.ToArray();
    }

    private void OnDestroy()
    {
        _targetObjects = null;
        _beDependArr = null;
        _foldoutArr = null;
    }
}
