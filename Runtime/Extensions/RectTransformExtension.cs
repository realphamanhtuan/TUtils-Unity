using UnityEngine;
namespace TUtils
{
    public static class RectTransformUtility{
        public static void SetLeft(this RectTransform rt, float left){
            rt.offsetMin = rt.offsetMin.alterMember(x:left);
        }
        public static float GetLeft(this RectTransform rt){
            return rt.offsetMin.x;
        }

        public static void SetRight(this RectTransform rt, float right){
            rt.offsetMax = rt.offsetMax.alterMember(x: -right);
        }
        public static float GetRight(this RectTransform rt){
            return -rt.offsetMax.x;
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = rt.offsetMax.alterMember(y:-top);
        }
        public static float GetTop(this RectTransform rt){
            return -rt.offsetMax.y;
        }

        public static void SetBottom(this RectTransform rt, float bottom){
            rt.offsetMin = rt.offsetMin.alterMember(y:bottom);
        }
        public static float GetBottom(this RectTransform rt){
            return rt.offsetMin.y;
        }

        public static void SetAnchoredX(this RectTransform rt, float x){
            rt.anchoredPosition = rt.anchoredPosition.alterMember(x:x);
        }
        public static float GetAnchoredX(this RectTransform rt){
            return rt.anchoredPosition.x;
        }

        public static void SetAnchoredY(this RectTransform rt, float y){
            rt.anchoredPosition = rt.anchoredPosition.alterMember(y:y);
        }
        public static float GetAnchoredY(this RectTransform rt){
            return rt.anchoredPosition.y;
        }

        public static void SetWidth(this RectTransform rt, float w){
            rt.sizeDelta = rt.sizeDelta.alterMember(x:w);
        }
        public static float GetWidth(this RectTransform rt){
            return rt.sizeDelta.x;
        }

        public static void SetHeight(this RectTransform rt, float h){
            rt.sizeDelta = rt.sizeDelta.alterMember(y:h);
        }
        public static float GetHeight(this RectTransform rt){
            return rt.sizeDelta.y;
        }
    }
}