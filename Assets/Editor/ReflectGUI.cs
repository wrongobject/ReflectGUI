using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
/// <summary>
/// 反射 GUI 绘制类
/// </summary>
public static class ReflectGUI
{
    /// <summary>
    /// 一级空白空间
    /// </summary>
    private const int SPACE_COUNT = 20;
    /// <summary>
    /// mini按钮大小
    /// </summary>
    private const int MINI_BUTOTN_SIZE = 20;

    private static readonly Type TypeInt = typeof(int);
    private static readonly Type TypeUInt = typeof(uint);
    private static readonly Type TypeString = typeof(string);
    private static readonly Type TypeFloat = typeof(float);
    private static readonly Type TypeDouble = typeof(double);
    private static readonly Type TypeLong = typeof(long);
    private static readonly Type TypeList = typeof(List<>);
    private static readonly Type TypeDict = typeof(Dictionary<,>);
    private static readonly List<Type> _basetypeList = new List<Type>() {
        TypeInt,TypeUInt,TypeString,TypeFloat,TypeDouble,TypeLong
    };
    private static Dictionary<int, bool> breviaryDict = new Dictionary<int, bool>();
    private static Dictionary<Type, object> cacheKeyDict = new Dictionary<Type, object>();
    private static Dictionary<Type, object> cacheValueDict = new Dictionary<Type, object>();
    /// <summary>
    /// DrawList 函数反射信息
    /// </summary>
    private static MethodInfo drawList;
    private static MethodInfo DrawListMethod
    {
        get
        {
            if (drawList == null)
            {
                drawList = typeof(ReflectGUI).GetMethod("DrawList");
            }
            return drawList;
        }
    }
    /// <summary>
    /// DrawDict 函数反射信息
    /// </summary>
    private static MethodInfo drawDict;
    private static MethodInfo DrawDictMethod
    {
        get
        {
            if (drawDict == null)
            {
                drawDict = typeof(ReflectGUI).GetMethod("DrawDict");
            }
            return drawDict;
        }
    }

    public static void DrawInt(string title,ref int value)
    {        
        value = EditorGUILayout.IntField(title,value);        
    }
    public static void DrawUint(string title, ref uint value)
    {
        value = (uint)EditorGUILayout.IntField(title, (int)value);
    }
    public static void DrawString(string title, ref string value)
    {
        value = EditorGUILayout.TextField(title, value);
    }
    public static void DrawFloat(string title, ref float value)
    {
        value = EditorGUILayout.FloatField(title, value);
    }
    public static void DrawDouble(string title, ref double value)
    {
        value = EditorGUILayout.DoubleField(title, value);
    }
    public static void DrawLong(string title, ref long value)
    {
        value = EditorGUILayout.LongField(title, value);
    }
    public static void DrawEnum(string title,ref Enum value)
    {
        value = EditorGUILayout.EnumPopup(title,value);
    }
    /// <summary>
    /// 绘制整个结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="space"></param>
    public static void Draw<T>(T data,int space = 0)
    {
        Draw(typeof(T),data,space);
    }

    private static object Draw(Type type, object data,int space = 0)
    {
        if (CheckAttribute(type, ref data, space))
        {
            return data;
        }        
        var attrName = type.GetCustomAttribute<InspectorNameAttribute>();
        string des = attrName == null ? type.Name : attrName.displayName;
        if (!Draw_Breviary(data, des, space))
        {
            return data;
        }
        space += SPACE_COUNT;
        Dictionary<string, FieldInfo> infos = ReflectHelper.GetFields(type);
        foreach (var item in infos)
        {
            object fdata = item.Value.GetValue(data);
            if (IsBaseType(item.Value.FieldType))
            {
                DrawBaseField(item.Value, ref fdata,space);
            }
            else if (item.Value.FieldType.IsGenericType)
            {
                Type[] types = item.Value.FieldType.GenericTypeArguments;
                if (types.Length == 1 && item.Value.FieldType == TypeList.MakeGenericType(types))
                {
                    var method = DrawListMethod;
                    MethodInfo draw = method.MakeGenericMethod(types);
                    attrName = item.Value.GetCustomAttribute<InspectorNameAttribute>();
                    des = attrName == null ? item.Value.FieldType.Name : attrName.displayName;
                    draw.Invoke(null, new object[] { fdata, des, space });
                }
                else if (types.Length == 2 && item.Value.FieldType == TypeDict.MakeGenericType(types))
                {
                    var method = DrawDictMethod;
                    MethodInfo draw = method.MakeGenericMethod(types);
                    attrName = item.Value.GetCustomAttribute<InspectorNameAttribute>();
                    des = attrName == null ? item.Value.FieldType.Name : attrName.displayName;
                    draw.Invoke(null, new object[] { fdata, des, space });
                }
            }
            else if (item.Value.FieldType.IsClass)
            {                
                if (fdata == null)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    if (GUILayout.Button("Create"))
                    {
                        fdata = type.Assembly.CreateInstance(type.FullName);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {                                       
                    Draw(item.Value.FieldType, fdata, space);                    
                }
            }
            item.Value.SetValue(data,fdata);
        }       
        return data;
    }
    /// <summary>
    /// 绘制list,反射调用
    /// </summary>  
    public static List<T> DrawList<T>(List<T> data,string des,int space)
    {        
        Type type = typeof(List<T>);
        if (data == null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            if (GUILayout.Button("Create" + type.Name))
            {
                data = new List<T>();
            }
            EditorGUILayout.EndHorizontal();
            return data;
        }

        if (Draw_Breviary(data, des, space))
        {
            space += SPACE_COUNT;
            Type typeT = typeof(T);
            for (int i = 0; i < data.Count; i++)
            {
                T item = data[i];
                EditorGUILayout.BeginHorizontal();
                if (IsBaseType(typeT))
                {
                    object obj = data[i];
                    DrawBaseType(i.ToString(), typeT, ref obj,space);
                    data[i] = (T)obj;
                }
                else
                {
                    Draw(typeT,data[i],space);
                }
                if (GUILayout.Button("-",GUILayout.Width(MINI_BUTOTN_SIZE)))
                {
                    data.Remove(item);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            if (GUILayout.Button("+"))
            {
                MethodInfo methodAdd = type.GetMethod("Add");
                if (methodAdd != null)
                {
                    object newobj = typeT.Assembly.CreateInstance(typeT.FullName);
                    methodAdd.Invoke(data,new object[] { newobj});
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        return data;
    }
    /// <summary>
    /// 绘制dict,反射调用
    /// </summary>   
    public static void DrawDict<K, V>(Dictionary<K, V> data, string des, int space)
    {
        if (!Draw_Breviary(data, des, space))
        {
            return;
        }
        space += SPACE_COUNT;
        Type typek = typeof(K);
        Type typev = typeof(V);
        foreach (var item in data)
        {
            K key = item.Key;
            V value = item.Value;

            GUILayout.BeginHorizontal();
            GUILayout.Space(space);
            if (IsBaseType(typek))
            {
                GUI.changed = false;
                object obj = key;
                DrawBaseType("Key", typek, ref obj, 0);
                if (GUI.changed)
                {
                    if (!data.ContainsKey((K)obj))
                    {
                        data.Add((K)obj,value);
                        data.Remove(key);
                    }
                    break;
                }
            }
            else
            {
                EditorGUILayout.BeginVertical();
                Draw(key);
                EditorGUILayout.EndVertical();
            }
            if (IsBaseType(typev))
            {
                GUI.changed = false;
                object obj = value;
                DrawBaseType("Value", typev, ref obj, 0);
                if (GUI.changed)
                {
                    data[key] = (V)obj;
                    break;
                }
            }
            else
            {
                EditorGUILayout.BeginVertical();
                Draw(value);
                EditorGUILayout.EndVertical();
            }
            if (GUILayout.Button("Remove"))
            {
                data.Remove(key);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Space(space);
        EditorUtil.BeginContents();

        object newkey = null;
        if (!cacheKeyDict.TryGetValue(typek, out newkey))
        {
            newkey = CreateInstance(typek);
            cacheKeyDict[typek] = newkey;
        }
        object newvalue = null;
        if (!cacheValueDict.TryGetValue(typev, out newvalue))
        {
            newvalue = CreateInstance(typev);
            cacheValueDict[typev] = newvalue;
        }
        if (IsBaseType(typek))
        {
            GUI.changed = false;
            DrawBaseType("Key", typek, ref newkey, 0);
            if(GUI.changed)
                cacheKeyDict[typek] = newkey;
        }
        else
        {
            EditorGUILayout.BeginVertical();
            Draw((K)newkey);
            EditorGUILayout.EndVertical();
        }

        if (IsBaseType(typev))
        {
            GUI.changed = false;
            DrawBaseType("Value", typev, ref newvalue, 0);
            if (GUI.changed)
                cacheValueDict[typev] = newvalue;
        }
        else
        {
            EditorGUILayout.BeginVertical();
            Draw((V)newvalue);
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add"))
        {
            if (!data.ContainsKey((K)newkey))
            {
                data.Add((K)newkey,(V)newvalue);
                cacheKeyDict.Remove(typek);
                cacheValueDict.Remove(typev);
            }
        }
        EditorUtil.EndContents();
        GUILayout.EndHorizontal();
    }
    /// <summary>
    /// 创建实例，区分string
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static object CreateInstance(Type type)
    {
        if (type == TypeString)
        {
            return "";
        }
        return type.Assembly.CreateInstance(type.FullName);
    }

    /// <summary>
    /// 绘制基础field
    /// </summary>  
    private static void DrawBaseField(FieldInfo info,ref object data,int space)
    {
        if (CheckAttribute(info, ref data, space))
        {
            return;
        }
        
        var attr = info.GetCustomAttribute<InspectorNameAttribute>();
        string name = attr == null ? info.Name : attr.displayName;        
        DrawBaseType(name, info.FieldType,ref data,space);       
    }
    /// <summary>
    /// 绘制基础类型
    /// </summary>   
    private static void DrawBaseType(string des,Type type,ref object data,int space)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(space);
        if (type == TypeInt)
        {
            int value = (int)data;
            DrawInt(des, ref value);
            data = value;
        }
        else if (type == TypeFloat)
        {
            float value = (float)(data);
            DrawFloat(des, ref value);
            data = value;
        }
        else if (type == TypeUInt)
        {
            uint value = (uint)(data);
            DrawUint(des, ref value);
            data = value;
        }
        else if (type == TypeString)
        {
            object obj = (data);
            string value = obj == null ? string.Empty : obj.ToString();
            DrawString(des, ref value);
            data = value;
        }
        else if (type == TypeDouble)
        {
            double value = (double)(data);
            DrawDouble(des, ref value);
            data = value;
        }
        else if (type == TypeLong)
        {
            long value = (long)(data);
            DrawLong(des, ref value);
            data = value;
        }
        else if (type.IsEnum)
        {
            Enum value = (Enum)(data);
            DrawEnum(des,ref value);
            data = value;
        }
        EditorGUILayout.EndHorizontal();
    }
    /// <summary>
    /// 是否是基础类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsBaseType(Type type)
    {
        if (_basetypeList.Contains(type))
            return true;
        if (type.IsEnum)
            return true;
        return false;
    }
    /// <summary>
    /// 检查属性并处理
    /// </summary>  
    private static bool CheckAttribute(Type type, ref object data, int space)
    {
        bool result = false;
        var attrCustomGUI = type.GetCustomAttribute<CustomizeGUIAttribute>();
        if (attrCustomGUI != null)
        {
            Type classType = Type.GetType(attrCustomGUI.typeName);
            if (classType != null)
            {
                MethodInfo method = classType.GetMethod(attrCustomGUI.methodName,attrCustomGUI.flag);
                if (method != null)
                {
                    var attrName = type.GetCustomAttribute<InspectorNameAttribute>();
                    string des = attrName == null ? type.Name : attrName.displayName;
                    if (Draw_Breviary(data, des, space))
                    {
                        space += SPACE_COUNT;
                        data = method.Invoke(null, new object[] { des, data, space });
                    }
                }
            }
            result = true;
        }

        var attrHide = type.GetCustomAttribute<HideInInspector>();
        if (attrHide != null)
        {
            result = true;
        }
        return result;
    }
    /// <summary>
    /// 检查属性并处理
    /// </summary>  
    private static bool CheckAttribute(FieldInfo info, ref object data, int space)
    {
        bool result = false;
        var attrCustomGUI = info.GetCustomAttribute<CustomizeGUIAttribute>();
        if (attrCustomGUI != null)
        {
            Type classType = Type.GetType(attrCustomGUI.typeName);
            if (classType != null)
            {
                MethodInfo method = classType.GetMethod(attrCustomGUI.methodName, attrCustomGUI.flag);
                if (method != null)
                {
                    var attrName = info.GetCustomAttribute<InspectorNameAttribute>();
                    string des = attrName == null ? info.Name : attrName.displayName;

                    data = method.Invoke(null, new object[] { des, data, space });
                }
            }
            result = true;
        }

        var attrHide = info.GetCustomAttribute<HideInInspector>();
        if (attrHide != null)
        {
            result = true;
        }
        return result;
    }
    /// <summary>
    /// 收缩栏
    /// </summary>
    public static bool Draw_Breviary(object data,string des,int space = 0,bool defaultValue = true)
    {        
        int hashCode = data.GetHashCode();
        bool isClicked = GetClicked(hashCode, defaultValue);
        GUILayout.BeginHorizontal();
        GUILayout.Space(space);
        isClicked = EditorGUILayout.Foldout(isClicked, des, true);
        SetClicked(hashCode,isClicked);
        GUILayout.EndHorizontal();
        return isClicked;
    }

    private static bool GetClicked(int hashCode,bool defaultValue)
    {
        bool result;
        if (!breviaryDict.TryGetValue(hashCode, out result))
        {
            result = defaultValue;
            breviaryDict.Add(hashCode, result);
        }
        return result;
    }
    private static void SetClicked(int hashCode,bool value)
    {
        breviaryDict[hashCode] = value;
    }
    /// <summary>
    /// 释放缓存的数据
    /// </summary>
    public static void Dispose()
    {
        breviaryDict.Clear();
        cacheKeyDict.Clear();
        cacheValueDict.Clear();
    }
}
