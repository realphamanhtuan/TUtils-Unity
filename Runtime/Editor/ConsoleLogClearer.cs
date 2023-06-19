using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
namespace TUtils{
    public class ConsoleLogClearer : Editor{
        [MenuItem("TUtils/Clear Developer Console Log")]
        public static void ClearConsoleLog(){
            Assembly assembly = Assembly.GetAssembly (typeof(SceneView));
            Type logEntries = assembly.GetType ("UnityEditor.LogEntries");
            logEntries.GetMethod ("Clear").Invoke(new object(), null);
            //Debug.ClearDeveloperConsole();
        }
    }
}