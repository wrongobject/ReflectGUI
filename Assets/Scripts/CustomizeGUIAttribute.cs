using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
/// <summary>
/// 自定义GUI方法属性
/// </summary>
public class CustomizeGUIAttribute : Attribute
{
    /// <summary>
    /// 方法所在类型名字
    /// </summary>
    public string typeName;
    /// <summary>
    /// GUI方法名字
    /// </summary>
    public string methodName;
    /// <summary>
    /// flag
    /// </summary>
    public BindingFlags flag;
    /// <summary>
    /// CustomizeGUIAttribute
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    /// <param name=""></param>
    public CustomizeGUIAttribute(string typeName,string methodName,BindingFlags flag = BindingFlags.Public)
    {
        this.typeName = typeName;
        this.methodName = methodName;
        //只使用静态方法
        this.flag = flag | BindingFlags.Static;
    }

    //提供的方法需满足如下参数  
    //method.Invoke(null,new object[]{string des,T data,int space})
   
}