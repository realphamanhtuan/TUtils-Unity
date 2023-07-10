using UnityEngine;
using UnityEditor;
namespace TUtils{
    public class TEditorRedirector : Editor {
        [MenuItem("TFramework/Centre Transform to Children's")]
        public static void CentreTransform(){
            if (Selection.activeGameObject == null) return;
            Transform transform = Selection.activeGameObject.transform;
            if (transform == null) return;
            TransformUtility.CentreTransformToChildren(transform);
        }
        [MenuItem("TFramework/Reverse Children's Orders")]
        public static void ReverseChildrensOrders(){
            if (Selection.activeGameObject == null) return;
            Transform transform = Selection.activeGameObject.transform;
            transform.ReverseChildrensOrders();
        }
    }
}