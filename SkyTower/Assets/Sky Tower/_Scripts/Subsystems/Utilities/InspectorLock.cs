#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class InspectorLock
{
    private static EditorWindow _mouseOverWindow;
    
    [MenuItem("Stuff/Select Inspector under mouse cursor (use hotkey) #&q")]
    static void SelectLockableInspector()
    {
        if (EditorWindow.mouseOverWindow.GetType().Name == "InspectorWindow")
        {
            _mouseOverWindow = EditorWindow.mouseOverWindow;
            Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
            Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
            int indexOf = findObjectsOfTypeAll.ToList().IndexOf(_mouseOverWindow);
            EditorPrefs.SetInt("LockableInspectorIndex", indexOf);
        }
    }
    
    [MenuItem("Stuff/Toggle Lock #q")]
    static void ToggleInspectorLock()
    {
        if (_mouseOverWindow == null)
        {
            if (!EditorPrefs.HasKey("LockableInspectorIndex"))
                EditorPrefs.SetInt("LockableInspectorIndex", 0);
            int i = EditorPrefs.GetInt("LockableInspectorIndex");
 
            Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
            Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
            _mouseOverWindow = (EditorWindow)findObjectsOfTypeAll[i];
        }
 
        if (_mouseOverWindow != null &&  _mouseOverWindow.GetType().Name == "InspectorWindow")
        {
            Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
            PropertyInfo propertyInfo = type.GetProperty("isLocked");
            bool value = (bool)propertyInfo.GetValue(_mouseOverWindow, null);
            propertyInfo.SetValue(_mouseOverWindow, !value, null);
            _mouseOverWindow.Repaint();
        }
    }
    
    [MenuItem("Stuff/Clear Console #&c")]
    static void ClearConsole()
    {
        //var logEntries = Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var logEntries = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.LogEntries");
        logEntries.GetMethod("Clear").Invoke(null,null);
        
        //PrintAllMethods("UnityEditor.LogEntries");
    }

    static void PrintAllMethods(string assemblyName)
    {
        var assembly = Assembly.GetAssembly(typeof(Editor)).GetType(assemblyName);
        var methods = assembly.GetMethods();
        
        foreach (var method in methods)
        {
            var parametersInfo = method.GetParameters();
            List<Type> parameters = new List<Type>();
            foreach (var parameter in parametersInfo)
            {
                parameters.Add(parameter.ParameterType);
            }

            string log = "";
            log = parameters.Aggregate(log, (current, p) => current + $" {p.Name} ");
            Debug.Log($"{method.Name} ({log})  IsPrivate - {method.IsPrivate}  IsPublic - {method.IsPublic}" +
                      $"  IsAssembly - {method.IsAssembly}  IsAbstract - {method.IsAbstract} IsConstructor - " +
                      $"{method.IsConstructor} IsStatic -  {method.IsStatic} IsVirtual - {method.IsVirtual}"
                      );
        }
    }
}

#endif 