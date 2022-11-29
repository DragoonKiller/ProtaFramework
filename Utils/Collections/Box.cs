using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Runtime.Serialization;
using System.Collections;

namespace Prota
{
    // struct 装箱.
    public class Box<T> where T: struct
    {
        public T value;
        
        public static implicit operator T(Box<T> v) => v.value;
    }
    
    public static class BoxExt
    {
        public static Box<T> Box<T>(this T value) where T: struct
        {
            return new Box<T>() { value = value };
        }
    }
}