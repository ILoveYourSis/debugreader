using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Logger : MonoBehaviour
{
    [SerializeField] List<string> tags;
    [SerializeField] List<bool> enables;

    Dictionary<string, bool> _tag2Enable;
    private void Awake()
    {
        _tag2Enable = new Dictionary<string, bool>();
        for (int i = 0; i < tags.Count; ++i)
        {
            if (_tag2Enable.ContainsKey(tags[i]))
            {
                Debug.LogError("[U] Ignore Duplicate Tag:" + tags[i]);
                continue;
            }
            _tag2Enable.Add(tags[i], enables[i]);
        }
    }

    void log(string tag, string info)
    {
        if (_tag2Enable.ContainsKey(tag) && _tag2Enable[tag])
            Debug.Log(formatLogString("LOG", tag, info));
    }

    void warn(string tag, string info)
    {
        if (_tag2Enable.ContainsKey(tag) && _tag2Enable[tag])
            Debug.LogWarning(formatLogString("WARN", tag, info));
    }

    void error(string tag, string info)
    {
        if (_tag2Enable.ContainsKey(tag) && _tag2Enable[tag])
            Debug.LogError(formatLogString("ERROR", tag, info));
    }

    static string formatLogString(string logType, string tag, string info)
    {
        return string.Format("{0} {1} [{2}]{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), logType, tag.ToUpper(), info);
    }

    static Logger instance;
    static Logger GetInstance()
    {
        if (instance == null) instance = (Instantiate(Resources.Load("Logger")) as GameObject).GetComponent<Logger>();
        return instance;
    }

    public static void Log(string info)
    {
        Log("default", info);
    }
    public static void Log(string tag, string info)
    {
#if UNITY_EDITOR
        Debug.Log(formatLogString("LOG", tag, info));
#else
        GetInstance().log(tag, info);
#endif
    }

    public static void Warn(string info)
    {
        Warn("default", info);
    }
    public static void Warn(string tag, string info)
    {
#if UNITY_EDITOR
        Debug.LogWarning(formatLogString("WARN", tag, info));
#else
        GetInstance().warn(tag, info);
#endif
    }

    public static void Error(string info)
    {
        Error("default", info);
    }
    public static void Error(string tag, string info)
    {
#if UNITY_EDITOR
        Debug.LogError(formatLogString("ERROR", tag, info));
#else
        GetInstance().error(tag, info);
#endif
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Logger))]
    public class LoggerInspector : Editor
    {
        SerializedObject so;
        SerializedProperty tagsProp;
        SerializedProperty enablesProp;
        void OnEnable()
        {
            so = new SerializedObject(target);
            tagsProp = so.FindProperty("tags");
            enablesProp = so.FindProperty("enables"); 
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical();
            if (tagsProp != null)
            {
                for (int i = 0; i < tagsProp.arraySize; ++i)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Tag:", GUILayout.Width(40));
                    tagsProp.GetArrayElementAtIndex(i).stringValue = GUILayout.TextField(tagsProp.GetArrayElementAtIndex(i).stringValue, GUILayout.Width(120));
                    GUILayout.Label("Enable:", GUILayout.Width(50));
                    enablesProp.GetArrayElementAtIndex(i).boolValue = GUILayout.Toggle(enablesProp.GetArrayElementAtIndex(i).boolValue, "");
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        tagsProp.DeleteArrayElementAtIndex(i);
                        enablesProp.DeleteArrayElementAtIndex(i);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("+"))
                {
                    int size = tagsProp.arraySize;
                    tagsProp.InsertArrayElementAtIndex(size) ;
                    tagsProp.GetArrayElementAtIndex(size).stringValue = "NewTag";
                    enablesProp.InsertArrayElementAtIndex(size);
                    enablesProp.GetArrayElementAtIndex(size).boolValue = true;
                }
            }
            GUILayout.EndVertical();
            if(EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
            }
        }
    }
#endif
}
