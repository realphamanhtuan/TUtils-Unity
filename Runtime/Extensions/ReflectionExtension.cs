using System.Reflection;
using System;
namespace TUtils {
    public static class ReflectionExtension {
        /// <summary>
        /// Get all fields of an object, using BindingFlags Public, NonPublic, and Instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (string, Type)[] GetFields(this object obj) {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = obj.GetType().GetFields(bindingFlags);
            (string, Type)[] ret = new (string, Type)[fields.Length];
            for (int i = 0; i < fields.Length; i++) 
                ret[i] = (fields[i].Name, fields[i].FieldType);
            return ret;
        }
        /// <summary>
        /// Get value of a field, searching using BindingFlags Public, NonPublic, and Instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetFieldValue<T>(this object obj, string name) {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            if (field == null) return default(T);
            try {
                return (T)(field.GetValue(obj));
            } catch {
                return default(T);
            }
        }
        /// <summary>
        /// Get type of a field, searching using BindingFlags Public, NonPublic, and Instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type GetFieldType(this object obj, string name) {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            if (field == null) return null;
            return field.FieldType;
        }
        /// <summary>
        /// Set value of a field, searching using BindingFlags Public, NonPublic, and Instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool SetFieldValue<T>(this object obj, string name, T value) {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            if (field != null) {
                try {
                    field.SetValue(obj, value);
                    return true;
                } catch (System.Exception e) {
                    Tebug.Warning("Cannot set field value. Exception:", e.Message);
                    return false;
                }
            } else return false;
        }
    }
}