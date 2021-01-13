using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
/// <summary>
/// 反射辅助类，cache以获取的对象
/// </summary>
public static class ReflectHelper
{
    /// <summary>
    /// 缓存的字段内容
    /// </summary>
    private static Dictionary<Type, Dictionary<string, FieldInfo>> typeFieldDict = new Dictionary<Type, Dictionary<string, FieldInfo>>();
    /// <summary>
    /// 缓存时使用的flag
    /// </summary>
    private static Dictionary<Type, BindingFlags> typeFlagDict = new Dictionary<Type, BindingFlags>();
    /// <summary>
    /// 获取所有属性的dict
    /// </summary>  
    public static Dictionary<string, FieldInfo> GetFields<T>(bool forceRefresh = false,BindingFlags flag = BindingFlags.Public | BindingFlags.Instance)
    {
        Type type = typeof(T);
        return GetFields(type, forceRefresh, flag);
    }
    /// <summary>
    /// 获取所有属性的dict
    /// </summary>    
    public static Dictionary<string, FieldInfo> GetFields(Type type, bool forceRefresh = false, BindingFlags flag = BindingFlags.Public | BindingFlags.Instance)
    {
        AddToCache(type,forceRefresh,flag);
        return typeFieldDict[type];
    }
    /// <summary>
    /// 添加到cache
    /// </summary>   
    private static void AddToCache(Type type, bool forceRefresh, BindingFlags flag)
    {
        Dictionary<string, FieldInfo> fieldDict = null;
        if (forceRefresh || !typeFieldDict.TryGetValue(type, out fieldDict) || typeFlagDict[type] != flag)
        {
            FieldInfo[] infos = type.GetFields(flag);
            if (fieldDict == null)
            {
                fieldDict = new Dictionary<string, FieldInfo>();
            }
            else
            {
                fieldDict.Clear();
            }

            foreach (var item in infos)
            {
                fieldDict.Add(item.Name,item);
            }
            typeFieldDict[type] = fieldDict;
            typeFlagDict[type] = flag;
        }
    }
    /// <summary>
    /// 获取字段
    /// </summary>   
    public static FieldInfo GetField<T>(string fieldName, bool forceRefresh = false, BindingFlags flag = BindingFlags.Public | BindingFlags.Instance)
    {
        return GetField(typeof(T), fieldName, forceRefresh, flag);
    }
    /// <summary>
    /// 获取字段
    /// </summary> 
    public static FieldInfo GetField(Type type, string fieldName, bool forceRefresh = false, BindingFlags flag = BindingFlags.Public | BindingFlags.Instance)
    {
        AddToCache(type, forceRefresh, flag);
        var dict = typeFieldDict[type];
        if (dict.TryGetValue(fieldName, out FieldInfo info))
        {
            return info;
        }
        return null;
    }
    /// <summary>
    /// 释放cache内容
    /// </summary>
    public static void Dispose()
    {
        typeFieldDict.Clear();
        typeFlagDict.Clear();
    }
}