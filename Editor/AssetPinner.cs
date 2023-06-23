using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection;
namespace TUtils{
    public class AssetPinner : Editor
    {
        ///EDITOR OPERATION REGION
        static string GetCurrentSelectionGuid(){
            UnityEngine.Object obj = Selection.activeObject;
            if (obj != null)
                return AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(obj)).ToString();
            else {
                Type projectWindowUtilType = typeof(ProjectWindowUtil);
                MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
                object o = getActiveFolderPath.Invoke(null, new object[0]);
                string pathToCurrentFolder = o.ToString();
                return AssetDatabase.GUIDFromAssetPath(pathToCurrentFolder).ToString();
            }
        }
        private static bool Select(string guid, bool setActiveObject)
        {
            EditorUtility.FocusProjectWindow();
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(guid));
            if (obj == null) return false;
            if (setActiveObject)
                Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
            return true;
        } 
        ///END EDITOR OPERATION REGION

        ///PREF PREFIX REGION
        const string prefPrefix = "TUtils_AssetPinner";
        static string GetPrefPrefix(){
            return prefPrefix + "_" + Application.productName;
        }
        ///END PREF PREFIX REGION

        ///BASIC PIN, RETRIEVE, FIND REGION
        static string PathKey(int index) {
            return GetPrefPrefix() + "_" + index.ToString() + "_Path";
        }
        static string GetGUID(int index){
            string path = EditorPrefs.GetString(PathKey(index), null);
            return !string.IsNullOrWhiteSpace(path) ? path : null;
        }
        static void SetPath(int index, string path){
            EditorPrefs.SetString(PathKey(index), path);
        }
        static bool Pin(int index){
            string key = PathKey(index);
            string guid = GetCurrentSelectionGuid();
            if (string.IsNullOrWhiteSpace(guid)) return false;
            EditorPrefs.SetString(key, guid);
            SetLastUseUTC(index);
            Tebug.Log(AssetDatabase.GUIDToAssetPath(guid), "->", index);
            return true;
        }
        static bool Retrieve(int index){
            string guid = EditorPrefs.GetString(PathKey(index));
            if (!string.IsNullOrWhiteSpace(guid)){
                if (!Select(guid, true)) return false;
                SetLastUseUTC(index);
                SetLastFoundOrRetrieved(index);
                Tebug.Log(index, "->", AssetDatabase.GUIDToAssetPath(guid));
                return true;
            } else return false;
        }
        static bool Find(int index){
            string guid = EditorPrefs.GetString(PathKey(index));
            if (!string.IsNullOrWhiteSpace(guid)){
                if (!Select(guid, false)) return false;
                SetLastUseUTC(index);
                SetLastFoundOrRetrieved(index);
                Tebug.Log(index, "->", AssetDatabase.GUIDToAssetPath(guid));
                return true;
            } else return false;
        }

        ///END BASIC PIN, RETRIEVE, FIND REGION
        
        ///ADVANCED REGION
        static string LastUseKey(int index) {
            return GetPrefPrefix() + "_" + index.ToString() + "_LastUse";
        }
        static int GetLRUIndex(){
            string pathToCheckIfAlreadyPinned = GetCurrentSelectionGuid();
            if (!string.IsNullOrWhiteSpace(pathToCheckIfAlreadyPinned)){
                for (int i = 1; i < 6; ++i){
                    string iPath = GetGUID(i);
                    if (!string.IsNullOrWhiteSpace(iPath) && iPath.Equals(pathToCheckIfAlreadyPinned))
                        return i;
                }
            }
            int resultIndex = 1;
            int resultIndexLastUsed = EditorPrefs.GetInt(LastUseKey(resultIndex), 0);
            //index counts from 1
            for (int i = 2; i < 6; ++i){
                int iLastUse = EditorPrefs.GetInt(LastUseKey(i), 0);
                if (iLastUse < resultIndexLastUsed) {
                    resultIndexLastUsed = iLastUse;
                    resultIndex = i;
                }
            }
            return resultIndex;
        }
        static void SetLastUseUTC(int index){
            EditorPrefs.SetInt(LastUseKey(index), (int) System.DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }
        static string LastFoundOrRetrievedIndexKey(){
            return GetPrefPrefix() + "_LastFoundOrRetrievedIndex";
        }
        static int GetNextFoundOrRetrieved(){
            int nextFoundOrRetrieved = (EditorPrefs.GetInt(LastFoundOrRetrievedIndexKey(), 0)) % 5 + 1;
            for (int i = 0; i < 5; ++i) {//5 tries
                string guid = GetGUID(nextFoundOrRetrieved);
                if (guid != null && !string.IsNullOrWhiteSpace(AssetDatabase.GUIDToAssetPath(guid))){
                    EditorPrefs.SetInt(LastFoundOrRetrievedIndexKey(), nextFoundOrRetrieved);
                    return nextFoundOrRetrieved;
                } else {
                    nextFoundOrRetrieved = nextFoundOrRetrieved % 5 + 1;
                }
            }
            return -1;
        }
        static void SetLastFoundOrRetrieved(int index){
            EditorPrefs.SetInt(LastFoundOrRetrievedIndexKey(), index);
        }
        ///END ADVANCED REGION

        [MenuItem("Assets/TUtils/AssetPinner/Pin to LRU slot")]
        public static void PinToLRUSlot(){Pin(GetLRUIndex());}
        [MenuItem("Assets/TUtils/AssetPinner/Pin to 1")]
        public static void PinTo1(){Pin(1);}
        [MenuItem("Assets/TUtils/AssetPinner/Pin to 2")]
        public static void PinTo2(){Pin(2);}
        [MenuItem("Assets/TUtils/AssetPinner/Pin to 3")]
        public static void PinTo3(){Pin(3);}
        [MenuItem("Assets/TUtils/AssetPinner/Pin to 4")]
        public static void PinTo4(){Pin(4);}
        [MenuItem("Assets/TUtils/AssetPinner/Pin to 5")]
        public static void PinTo5(){Pin(5);}
        
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve Next &`")]
        public static void RetrieveNext(){Retrieve(GetNextFoundOrRetrieved());}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve Next", true)]
        public static bool ValidateRetrieveNext(){
            for (int i = 1; i < 6; ++i)
                if (GetGUID(i) != null) return true;
            return false;
        }
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 1 &1")]
        public static void Retrieve1(){Retrieve(1);}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 1", true)]
        public static bool ValidateRetrieve1(){return GetGUID(1) != null;}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 2 &2")]
        public static void Retrieve2(){Retrieve(2);}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 2", true)]
        public static bool ValidateRetrieve2(){return GetGUID(2) != null;}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 3 &3")]
        public static void Retrieve3(){Retrieve(3);}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 3", true)]
        public static bool ValidateRetrieve3(){return GetGUID(3) != null;}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 4 &4")]
        public static void Retrieve4(){Retrieve(4);}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 4", true)]
        public static bool ValidateRetrieve4(){return GetGUID(4) != null;}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 5 &5")]
        public static void Retrieve5(){Retrieve(5);}
        [MenuItem("Assets/TUtils/AssetPinner/Retrieve 5", true)]
        public static bool ValidateRetrieve5(){return GetGUID(5) != null;}

        [MenuItem("Assets/TUtils/AssetPinner/Find Next")]
        public static void FindNext(){Find(GetNextFoundOrRetrieved());}
        [MenuItem("Assets/TUtils/AssetPinner/Find Next", true)]
        public static bool ValidateFindNext(){
            for (int i = 1; i < 6; ++i)
                if (GetGUID(i) != null) return true;
            return false;
        }
        [MenuItem("Assets/TUtils/AssetPinner/Find 1")]
        public static void Find1(){Find(1);}
        [MenuItem("Assets/TUtils/AssetPinner/Find 1", true)]
        public static bool ValidateFind1(){return GetGUID(1) != null;}
        [MenuItem("Assets/TUtils/AssetPinner/Find 2")]
        public static void Find2(){Find(2);}
        [MenuItem("Assets/TUtils/AssetPinner/Find 2", true)]
        public static bool ValidateFind2(){return GetGUID(2) != null;}
        [MenuItem("Assets/TUtils/AssetPinner/Find 3")]
        public static void Find3(){Find(3);}
        [MenuItem("Assets/TUtils/AssetPinner/Find 3", true)]
        public static bool ValidateFind3(){return GetGUID(3) != null;}
        [MenuItem("Assets/TUtils/AssetPinner/Find 4")]
        public static void Find4(){Find(4);}
        [MenuItem("Assets/TUtils/AssetPinner/Find 4", true)]
        public static bool ValidateFind4(){return GetGUID(4) != null;}
        [MenuItem("Assets/TUtils/AssetPinner/Find 5")]
        public static void Find5(){Find(5);}
        [MenuItem("Assets/TUtils/AssetPinner/Find 5", true)]
        public static bool ValidateFind5(){return GetGUID(5) != null;}

        [MenuItem("Assets/TUtils/AssetPinner/Clear All Pins")]
        public static void ClearAllPins(){
            for (int i = 1; i < 6; ++i){
                EditorPrefs.DeleteKey(PathKey(i));
                EditorPrefs.DeleteKey(LastUseKey(i));
            }
            EditorPrefs.DeleteKey(LastFoundOrRetrievedIndexKey());
        }
    }
}