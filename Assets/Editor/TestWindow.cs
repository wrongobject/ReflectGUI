using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TestWindow : EditorWindow
{
    [MenuItem("Tools/TestWindow")]
    static void DoIt()
    {
        TestWindow window = GetWindow<TestWindow>();        
        window.Show();
    }
    TestClass testClass = new TestClass();
    private void OnGUI()
    {       
        ReflectGUI.Draw(testClass);        
    }
}
public enum EType
{
    [InspectorName("属A")]
    A,
    [InspectorName("属B")]
    B,
    [InspectorName("属C")]
    C,
}
public class TestClass
{
    [InspectorName("属性A")]
    public List<int> a = new List<int>();
    [InspectorName("属性B")]
    public int b;
    [InspectorName("属性C")]
    public string c;
    [InspectorName("属性枚举")]
    public EType eType;
    [InspectorName("属性")]
    [CustomizeGUI("TestClass", "RepalaceGUI", System.Reflection.BindingFlags.Public)]
    public EType eType2;
    [InspectorName("属性Dict")]
    public Dictionary<int, string> dict = new Dictionary<int, string>() { { 1,"ds"}, { 2, "fg" } };
    [InspectorName("属性Dict ClasB")]
    public Dictionary<string, ClasB> dictB = new Dictionary<string, ClasB>();
    public static EType RepalaceGUI(string des, EType data,int space)
    {
        EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField(des);

        GUILayout.Space(space);
        data = (EType)EditorGUILayout.EnumPopup(data);
        EditorGUILayout.EndHorizontal();
        return data;
    }
}

public class ClasB
{
    public int a;
    public string b;
}