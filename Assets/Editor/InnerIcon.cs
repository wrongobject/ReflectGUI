using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public static class InnerIcon 
{
    private static MethodInfo _loadIconMethod;

    private const string _Help = "_Help";
    private const string _WaitSpin00 = "WaitSpin00";
    private const string _Popup = "_Popup";
    private const string _TimelineWindow = "UnityEditor.Timeline.TimelineWindow";
    private const string _PrefabIcon = "Prefab Iconba";
    private const string _AudioIcon = "Profiler.Audio";
    private const string _EyeOpen = "animationvisibilitytoggleon";
    private const string _EyeOff = "animationvisibilitytoggleoff";
    private const string _Trash = "TreeEditor.Trash";
    private const string _Play = "PlayButton";
    private const string _CreateAddNew = "CreateAddNew";
    private const string _Folder = "Project";
    private const string _Save = "SaveAs";

    private static Texture help;
    private static Texture waitSpin00;
    private static Texture popup;
    private static Texture timelineWindow;
    private static Texture prefabIcon;
    private static Texture audioIcon;
    private static Texture eyeOpen;
    private static Texture eyeOff;
    private static Texture trash;
    private static Texture play;
    private static Texture createAddNew;
    private static Texture folder;
    private static Texture save;
   
    public static Texture Help { get { return LoadInnerIcon(_Help,ref help); } }
    public static Texture WaitSpin00 { get { return LoadInnerIcon(_WaitSpin00, ref waitSpin00); } }
    public static Texture Popup { get { return LoadInnerIcon(_Popup, ref popup); } }
    public static Texture TimelineWindow { get { return LoadInnerIcon(_TimelineWindow, ref timelineWindow); } }
    public static Texture PrefabIcon { get { return LoadInnerIcon(_PrefabIcon, ref prefabIcon); } }
    public static Texture AudioIcon { get { return LoadInnerIcon(_AudioIcon, ref audioIcon); } }
    public static Texture EyeOpen { get { return LoadInnerIcon(_EyeOpen, ref eyeOpen); } }
    public static Texture EyeOff { get { return LoadInnerIcon(_EyeOff, ref eyeOff); } }
    public static Texture Trash { get { return LoadInnerIcon(_Trash, ref trash); } }
    public static Texture Play { get { return LoadInnerIcon(_Play, ref play); } }
    public static Texture CreateAddNew { get { return LoadInnerIcon(_CreateAddNew, ref createAddNew); } }
    public static Texture Folder { get { return LoadInnerIcon(_Folder, ref folder); } }
    public static Texture Save { get { return LoadInnerIcon(_Save, ref save); } }

    private static Texture LoadInnerIcon(string name,ref Texture texture)
    {
        if (texture) return texture;
        if (_loadIconMethod == null)
        {
            _loadIconMethod = typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic);
        }
        if (_loadIconMethod == null) return null;

        texture = _loadIconMethod.Invoke(null, new object[] { name }) as Texture;
        return texture;
    }
}