using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace TUtils{
    /// <summary>
    /// Python-style logging functions that automatically serializes and concatenates your parameters.
    /// </summary>
    public static class Tebug{  
        static Tebug(){
            if (!TUtilsConfig.SHOW_DEBUG_LOG)
                Debug.Log("SHOW_DEBUG_LOG flag is not enabled. Logs won't be printed.");
            if (!TUtilsConfig.SHOW_VERBOSE_LOG)
                Debug.Log("SHOW_VERBOSE_LOG flag is not enabled. Verbose logs won't be printed.");
        }
        static string ObjectToString(object x){
            if (x == null) return "null";
            if (!(x is string)){
                if (x is KeyValuePair)
                {
                    return $"{ObjectToString(((KeyValuePair<object, object>)x).Key)}: {ObjectToString(((KeyValuePair<object, object>)x).Value)}";
                } else if (x is IEnumerable){
                    StringBuilder stringBuilder = new StringBuilder();
                    bool first = true;
                    if (x is ISet<object> || x is IDictionary<object, object>) stringBuilder.Append('{');
                    else stringBuilder.Append('[');
                    foreach (var t in x as IEnumerable){
                        if (!first)
                            stringBuilder.Append(", ");
                        first = false;
                        stringBuilder.Append(ObjectToString(t));
                    }
                    if (x is ISet<object> || x is IDictionary<object, object>) stringBuilder.Append('}');
                    else stringBuilder.Append(']');
                    return stringBuilder.ToString();
                } else return x.ToString();
            } else return (string) x;
        }
        static string ParamsToString(object[] x){
            if (x == null) return "null";
            if (x.Length == 0) return "";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ObjectToString(x[0]));
            for (int i = 1; i < x.Length; ++i){
                stringBuilder.Append(' ');
                stringBuilder.Append(ObjectToString(x[i]));
            }
            return stringBuilder.ToString();
        }
        public static void Log(params object[] x){
            if (TUtilsConfig.SHOW_DEBUG_LOG)
                Debug.Log(ParamsToString(x));
        }
        public static void Verbose(params object[] x){
            if (TUtilsConfig.SHOW_VERBOSE_LOG)
                Debug.Log(ParamsToString(x));
        }
        public static void Error(params object[] x){
            if (TUtilsConfig.SHOW_DEBUG_LOG)
                Debug.LogError(ParamsToString(x));
        }
        public static void Warning(params object[] x){
            if (TUtilsConfig.SHOW_DEBUG_LOG)
                Debug.LogWarning(ParamsToString(x));
        }
    }
}