using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Prota.Unity;

namespace Prota.Unity
{
    [Serializable]
    public sealed class SerializedData : ICloneable<SerializedData>, ICloneable
    {
        [SerializeField]
        public List<int> data = new List<int>();
        
        
        [NonSerialized]
        int current = 0;
        
        object ICloneable.Clone() => this.Clone();
        
        public SerializedData Clone()
        {
            var res = new SerializedData();
            res.data = new List<int>(data);
            return res;
        }
        
        public void Clear()
        {
            data.Clear();
            Reset();
        }
        
        public void Reset()
        {
            current = 0;
        }
        
        
        
        public void Push(int v) => data.Add(v);
        public int Int() =>  data[current++];
        
        
        
        
        public void Push(float v)
        {
            int x = 0;
            unsafe
            {
                int* p = &x;
                *((float*)p) = v;
            }
            data.Add(x);
        }
        public float Float()
        {
            var v = data[current++];
            float d;
            unsafe
            {
                d = *((float*)(&v));
            }
            return d;
        }
        
        
        
        
        
        
        public void Push(Color v)
        {
            Push(v.r);
            Push(v.g);
            Push(v.b);
            Push(v.a);
        }
        
        public Color Color()
        {
            var r = Float();
            var g = Float();
            var b = Float();
            var a = Float();
            return new Color(r, g, b, a);
        }
        
        
        
        
        public void Push(Vector2 v)
        {
            Push(v.x);
            Push(v.y);
        }
        
        public Vector2 Vector2()
        {
            var x = Float();
            var y = Float();
            return new Vector2(x, y);
        }
        
        
        
        
        public void Push(Vector3 v)
        {
            Push(v.x);
            Push(v.y);
            Push(v.z);
        }
        
        public Vector3 Vector3()
        {
            var x = Float();
            var y = Float();
            var z = Float();
            return new Vector3(x, y, z);
        }
        
        
        
        
        public void Push(Vector4 v)
        {
            Push(v.x);
            Push(v.y);
            Push(v.z);
            Push(v.w);
        }
        public Vector4 Vector4()
        {
            var x = Float();
            var y = Float();
            var z = Float();
            var w = Float();
            return new Vector4(x, y, z, w);
        }
        
        
        
        
        public void Push(Quaternion v)
        {
            Push(v.x);
            Push(v.y);
            Push(v.z);
            Push(v.w);
        }
        public Quaternion Quaternion()
        {
            var x = Float();
            var y = Float();
            var z = Float();
            var w = Float();
            return new Quaternion(x, y, z, w);
        }
        
        
        
        public void Push(string s)
        {
            int len = 0;
            var lenPos = data.Count;
            data.Add(0);
            foreach(var c in s)
            {
                data.Add(c);
                len++;
            }
            data[lenPos] = len;
        }
        
        [ThreadStatic] static StringBuilder cachedSb = new StringBuilder();
        public string String()
        {
            cachedSb.Clear();
            var len = data[current++];
            for(int i = 0; i < len; i++)
            {
                var c = data[current++];
                cachedSb.Append((char)c);
            }
            return cachedSb.ToString();
        }
        
        
        
        
    }
}