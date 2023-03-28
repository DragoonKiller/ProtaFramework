using System;
using UnityEngine;
using Unity.Collections;
namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public struct NativeArrayElementReference<T> where T: struct
        {
            public NativeArrayElementReference(NativeArray<T> list, int index)
            {
                this.list = list;
                this.index = index;
            }

            public readonly NativeArray<T> list;
            public readonly int index;
            
            public bool exist => index >= 0 && index < list.Length;
            
            public T value
            {
                get => list[index];
                set
                {
                    var a = list;
                    a[index] = value;
                }
            }
            
        }
        
        public static NativeArrayElementReference<T> ElementReference<T>(this NativeArray<T> list, int index)
            where T: struct
            => new NativeArrayElementReference<T>(list, index);
        
        
        // w * h array, with lines first and column second.
        // point (a, b) will point to a * w + b.
        public struct NativeArrayView2D<T> where T: struct
        {
            public readonly NativeArray<T> list;
            public readonly int w, h;

            public NativeArrayView2D(NativeArray<T> list, int w, int h)
            {
                this.list = list;
                this.w = w;
                this.h = h;
            }
            
            public bool Valid(int l, int c) => 0 <= l && l < h && 0 <= c && c < w;
            
            public T this[int l, int c]
            {
                get => list[l * w + c];
                set
                {
                    var a = list;
                    a[l * w + c] = value;
                }
            }
        }
        
        public static NativeArrayView2D<T> View2D<T>(this NativeArray<T> list, int w, int h)
            where T: struct
            => new NativeArrayView2D<T>(list, w, h);
            
    }
}
