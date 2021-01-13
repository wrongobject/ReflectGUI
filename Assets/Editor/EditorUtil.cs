using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
public class EditorUtil 
{
    private static Color _defaultBackgroundColor = new Color(0.8f, 0.8f, 0.8f);
    private static Color _darkColor = new Color(0, 0, 0, 0.7f);
    private static Color _lightColor = new Color(1, 1, 1, 0.7f);
    private static int _beginCount = 0;
    private static Stack<Color> _backGroiundColorStack = new Stack<Color>();
    private static Stack<Color> _textColorStack = new Stack<Color>();

    private static MethodInfo _loadIconMethod;

    public static void BeginContents()
    {
        _beginCount++;
        GUILayout.BeginHorizontal();
        EditorGUILayout.BeginHorizontal("TextArea",GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }
    public static void EndContents()
    {
        if (_beginCount > 0)
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
            _beginCount--;
        }
    }

    public static bool DrawHeader(string title,string key,bool defaultExpend = true)
    {
        GUILayout.Space(3.0f);
        key = key + title;
        bool expanded = EditorPrefs.GetBool(key, defaultExpend);
        if (!expanded) GUI.backgroundColor = _defaultBackgroundColor;

        EditorGUILayout.BeginHorizontal();
        GUI.changed = false;

        if (expanded) title = "\u25BC" + (char)0x200a + title;
        else title = "\u25BA" + (char)0x200a + title;

        GUILayout.BeginHorizontal();
        GUI.contentColor = EditorGUIUtility.isProSkin ? _lightColor : _darkColor;
        if (!GUILayout.Toggle(true, title, "dragtab", GUILayout.MinWidth(20f))) expanded = !expanded;
        GUI.contentColor = Color.white;
        if (GUI.changed) EditorPrefs.SetBool(key, expanded);
        GUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!expanded) GUILayout.Space(3);

        return expanded;
    }

    public static void CahngeGUITextColor(Color color)
    {
        _textColorStack.Push(GUI.contentColor);
        GUI.contentColor = color;
    }
    public static void RecoverGUITextColor()
    {
        if (_textColorStack.Count > 0)
        {
            GUI.contentColor = _textColorStack.Pop();
        }
    }
    public static void ChangeGUIBackgroundColor(Color color)
    {
        _backGroiundColorStack.Push(GUI.backgroundColor);
        GUI.backgroundColor = color;
    }
    public static void RecoverBackgroundColor()
    {
        if (_backGroiundColorStack.Count > 0)
        {
            GUI.backgroundColor = _backGroiundColorStack.Pop();
        }
    }
    public static Texture LoadInnerIcon(string name)
    {
        if (_loadIconMethod == null)
        {
            _loadIconMethod = typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic);
        }
        if (_loadIconMethod == null) return null;
        return _loadIconMethod.Invoke(null, new object[] { name }) as Texture;
    }
}
