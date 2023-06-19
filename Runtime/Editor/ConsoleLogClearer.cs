using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
namespace TFramework{
    public class ConsoleLogClearer : Editor{
        [MenuItem("TFramework/Clear Developer Console Log")]
        public static void ClearConsoleLog(){
            Assembly assembly = Assembly.GetAssembly (typeof(SceneView));
            Type logEntries = assembly.GetType ("UnityEditor.LogEntries");
            logEntries.GetMethod ("Clear").Invoke(new object(), null);
            //Debug.ClearDeveloperConsole();
        }
    }
}