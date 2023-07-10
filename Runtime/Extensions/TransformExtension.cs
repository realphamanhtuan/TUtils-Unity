using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace TUtils{
    public static class TransformUtility{
        /// <summary>
        /// Finds the first gameobject that is a child of "transform" and matches the name. Use multiple names as short for chain commands. 
        /// For example, Find("abc").Find("def") and Find("abc", "def") yield the same result. The function uses performance-costly BFS algorithm, so use it as little as you can.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="includeInactiveTransform"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static Transform FindFirstChildWithName(this Transform transform, bool includeInactiveTransform = true, params string[] names){
            if (names == null || names.Length == 0) return null;
            // first: the index of the name array the current pointer is looking for
            // second: the transform to visit
            Queue<(int, Transform)> q = new Queue<(int, Transform)>();
            q.Enqueue((0, transform));
            while (q.Count > 0){
                int nameIndex;
                Transform t;
                (nameIndex, t) = q.Dequeue();

                // nameIndexIncrease == 0: The currently visited transform doesn't match the name it's looking for
                // nameIndexIncrease == 1: The currently visited transform matches the name it's looking for, so increase the delta by 1
                if (t.name.Equals(names[nameIndex]) && (includeInactiveTransform || t.gameObject.activeInHierarchy)) 
                    nameIndex += 1;

                // Loop stops when found the first child
                if (nameIndex >= names.Length)
                    return t;
                else
                    //visits t
                    for (int i = 0; i < t.childCount; ++i)
                        q.Enqueue((nameIndex, t.GetChild(i)));
            }
            return null;
        }
        /// <summary>
        /// Finds all gameobjects that are children of "transform" and matches the name. Use multiple names as short for chain commands. 
        /// For example, Find("abc").Find("def") and Find("abc", "def") yield the same result. The function uses performance-costly BFS algorithm, so use it as little as you can.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="includeInactiveTransform"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static Transform[] FindChildrenWithName(this Transform transform, bool includeInactiveTransform = true, params string[] names){
            HashSet<Transform> ret = new HashSet<Transform>();
            if (names == null || names.Length == 0) return ret.ToArray();
            // first: the index of the name array the current pointer is looking for
            // second: the transform to visit
            Queue<(int, Transform)> q = new Queue<(int, Transform)>();
            HashSet<(int, Transform)> visited = new HashSet<(int, Transform)>();
            q.Enqueue((0, transform));
            while (q.Count > 0){
                int nameIndex;
                Transform t;
                (nameIndex, t) = q.Dequeue();
                Tebug.Log(nameIndex, t.name);
                // maxNameIndexIncrease == 0: The currently visited transform doesn't match the name it's looking for
                // maxNameIndexIncrease == 1: The currently visited transform matches the name it's looking for, so increase the delta by 1
                int maxNameIndexIncrease = 0;
                if (t.name.Equals(names[nameIndex]) && (includeInactiveTransform || t.gameObject.activeInHierarchy)) {
                    if (nameIndex == names.Length - 1)
                        ret.Add(t);
                    else 
                        maxNameIndexIncrease = 1;
                }

                //visits t
                for (int i = 0; i < t.childCount; ++i){
                    Transform child = t.GetChild(i);
                    for (int j = 0; j <= maxNameIndexIncrease; ++j){
                        Tebug.Log("Checking", nameIndex + j, child);
                        if (!visited.Contains((nameIndex + j, child)))
                            q.Enqueue((nameIndex + j, child));
                            visited.Add((nameIndex + j, child));
                    }
                }
            }
            return ret.ToArray();
        }
        /// <summary>
        /// Finds the first gameobject that is a child of "transform" and matches the tag. Use multiple tags as short for chain commands. 
        /// For example, Find("abc").Find("def") and Find("abc", "def") yield the same result. The function uses performance-costly BFS algorithm, so use it as little as you can.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="includeInactiveTransform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform FindFirstChildWithTag(this Transform transform, bool includeInactiveTransform = false, params string[] tags){
            if (tags == null || tags.Length == 0) return null;
            // first: the index of the tag array the current pointer is looking for
            // second: the transform to visit
            Queue<(int, Transform)> q = new Queue<(int, Transform)>();
            q.Enqueue((0, transform));
            while (q.Count > 0){
                int tagIndex;
                Transform t;
                (tagIndex, t) = q.Dequeue();

                // tagIndexIncrease == 0: The currently visited transform doesn't match the tag it's looking for
                // tagIndexIncrease == 1: The currently visited transform matches the tag it's looking for, so increase the delta by 1
                if (t.tag.Equals(tags[tagIndex]) && (includeInactiveTransform || t.gameObject.activeInHierarchy)) 
                    tagIndex += 1;

                // Loop stops when found the first child
                if (tagIndex >= tags.Length)
                    return t;
                else 
                    //visits t
                    for (int i = 0; i < t.childCount; ++i)
                        q.Enqueue((tagIndex, t.GetChild(i)));
            }
            return null;
        }
        /// <summary>
        /// Finds the gameobjects that are children of "transform" and matches the tag. Use multiple tags as short for chain commands. 
        /// For example, Find("abc").Find("def") and Find("abc", "def") yield the same result. The function uses performance-costly BFS algorithm, so use it as little as you can.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="includeInactiveTransform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform[] FindChildrenWithTag(this Transform transform, bool includeInactiveTransform = false, params string[] tags){
            HashSet<Transform> ret = new HashSet<Transform>();
            if (tags == null || tags.Length == 0) return ret.ToArray();
            // first: the index of the tag array the current pointer is looking for
            // second: the transform to visit
            Queue<(int, Transform)> q = new Queue<(int, Transform)>();
            HashSet<(int, Transform)> visited = new HashSet<(int, Transform)>();
            q.Enqueue((0, transform));
            while (q.Count > 0){
                int tagIndex;
                Transform t;
                (tagIndex, t) = q.Dequeue();

                // maxTagIndexIncrease == 0: The currently visited transform doesn't match the tag it's looking for
                // maxTagIndexIncrease == 1: The currently visited transform matches the tag it's looking for, so increase the delta by 1
                int maxTagIndexIncrease = 0;
                if (t.tag.Equals(tags[tagIndex]) && (includeInactiveTransform || t.gameObject.activeInHierarchy)) 
                    maxTagIndexIncrease = 1;

                // Loop stops when found the first child
                if (tagIndex >= tags.Length)
                    ret.Add(t);

                else 
                    //visits t
                    for (int i = 0; i < t.childCount; ++i){
                        Transform child = t.GetChild(i);
                        for (int j = 0; j <= maxTagIndexIncrease; ++j){
                            if (!visited.Contains((tagIndex + j, child)))
                                q.Enqueue((tagIndex + j, child));
                                visited.Add((tagIndex + j, child));
                        }
                    }
            }
            return ret.ToArray();
        }
        public static void CentreTransformToChildren(this Transform transform){
            if (transform.childCount > 0){
                Vector3 sum = Vector3.zero;
                for (int i = 0; i < transform.childCount; ++i){
                    sum += transform.GetChild(i).position;
                }
                sum /= transform.childCount;
                List<GameObject> children = new List<GameObject>();
                for (int i = transform.childCount - 1; i > -1; --i){
                    children.Add(transform.GetChild(i).gameObject);
                    transform.GetChild(i).parent = null;
                }
                transform.position = sum;
                for (int i = 0; i < children.Count; ++i){
                    children[i].transform.parent = transform;
                }
            }
        }
        public static void ReverseChildrensOrders(this Transform transform){
            if (transform == null) return;
            System.Collections.Generic.Queue<Transform> queue = new System.Collections.Generic.Queue<Transform>();
            for (int i = transform.childCount - 1; i > -1; --i){
                queue.Enqueue(transform.GetChild(i));
                transform.GetChild(i).SetParent(null, true);
            }
            while (queue.Count > 0)
                queue.Dequeue().SetParent(transform, true);
        }

        public static Transform GetRootTransform(this Transform transform){
            if (transform.parent == null) return transform;
            return GetRootTransform(transform.parent);
        }
    }
}