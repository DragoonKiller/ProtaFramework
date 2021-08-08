using System;
using UnityEngine;


namespace Prota.Unity
{
    public static class TransformExt
    {
        public static void ForeachChild(this Transform t, Action<Transform> f)
        {
            for(int i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);
                f(child);
            }
        }
        
        
        private static Action<Transform> currentRecursiveTransformOp;
        private static void ForeachTransformRecursivelyInternal(Transform t)
        {
            currentRecursiveTransformOp(t);
            ForeachChild(t, ForeachTransformRecursivelyInternal);
        }
        public static void ForeachTransformRecursively(this Transform t, Action<Transform> f)
        {
            if(t == null || f == null) return;
            currentRecursiveTransformOp = f;
            ForeachTransformRecursivelyInternal(t);
        }
        
        
        public static int GetDepth(this Transform t)
        {
            int d = 0;
            while(t != null)
            {
                t = t.parent;
                d++;
            }
            return d;
        }
    }
    
}