using UnityEngine;

namespace TUtils
{
    public static class CommonExtension{
        /// <summary>
        /// Check if an object is one of the items of a set.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="set"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsInSet<T>(this T t, params T[] set){
            int length = set.Length;
            for (int i = 0; i < length; ++i){
                if (t.Equals(set[i])) return true;
            }
            return false;
        }
        /// <summary>
        /// Return a random item of the array.
        /// </summary>
        /// <param name="arr"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomItem<T>(this T[] arr){
            if (arr == null || arr.Length <= 0) return default(T);
            return arr[UnityEngine.Random.Range(0, arr.Length)];
        }
        /// <summary>
        /// Return a random item of the list.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomItem<T>(this System.Collections.Generic.List<T> list){
            if (list == null || list.Count <= 0) return default(T);
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        public static Vector3 alterMember(this Vector3 v, float x = float.NaN, float y = float.NaN, float z = float.NaN){
            Vector3 r = v;
            if (!float.IsNaN(x)) r.x = x;
            if (!float.IsNaN(y)) r.y = y;
            if (!float.IsNaN(z)) r.z = z;
            return r;
        }
        public static Vector2 alterMember(this Vector2 v, float x = float.NaN, float y = float.NaN){
            Vector3 r = v;
            if (!float.IsNaN(x)) r.x = x;
            if (!float.IsNaN(y)) r.y = y;
            return r;
        }
        public static Color alterMember(this Color c, float r = float.NaN, float g = float.NaN, float b = float.NaN, float a = float.NaN){
            Color ret = new Color(c.r, c.g, c.b, c.a);
            if (!float.IsNaN(r)) ret.r = r;
            if (!float.IsNaN(g)) ret.g = g;
            if (!float.IsNaN(b)) ret.b = b;
            if (!float.IsNaN(a)) ret.a = a;
            return ret;
        }
    }
}