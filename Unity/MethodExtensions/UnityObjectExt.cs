using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        // 用来把非 null 但是已删除的对象转换为 null, 这样可以对齐 C# 语法糖.
        public static UnityEngine.Object CheckNull(this UnityEngine.Object x)
        {
            if(x == null) return null;
            return x;
        }
        
        public static bool IsActualNull(this UnityEngine.Object x)
        {
            return object.ReferenceEquals(x, null);
        }
        
        public static bool IsDestroyed(this UnityEngine.Object x)
        {
            return !x.IsActualNull() && x == null;
        }
        
    }
}
