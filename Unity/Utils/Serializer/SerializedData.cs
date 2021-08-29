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
        
        enum TypeId : int
        {
            Float = 1,
            Int = 2,
            Vec = 3,
            Str = 4,
            Obj = 5,
        }
        
        
        [NonSerialized]
        int current;
        
        [SerializeField]
        List<int> indexIndicator = new List<int>();
        
        [SerializeField]
        List<TypeId> typeId = new List<TypeId>();
        
        // ============================================================================================================
        // Float
        // ============================================================================================================
        
        [SerializeField]
        List<float> floats = new List<float>();
        
        public void Push(float v)
        {
            typeId.Add(TypeId.Float);
            indexIndicator.Add(floats.Count);
            floats.Add(v);
        }
        
        public float Float()
        {
            Debug.Assert(typeId[current] == TypeId.Float);
            return floats[indexIndicator[current++]];
        }
        
        // ============================================================================================================
        // Int
        // ============================================================================================================
        
        [SerializeField]
        List<int> ints = new List<int>();
        
        public void Push(int v)
        {
            typeId.Add(TypeId.Int);
            indexIndicator.Add(ints.Count);
            ints.Add(v);
        }
        
        public int Int()
        {
            Debug.Assert(typeId[current] == TypeId.Int);   
            return ints[indexIndicator[current++]];
        }
        
        // ============================================================================================================
        // Vector4
        // ============================================================================================================
        
        
        [SerializeField]
        List<Vector4> vecs = new List<Vector4>();
        
        public void Push(Vector4 v)
        {
            typeId.Add(TypeId.Vec);
            indexIndicator.Add(vecs.Count);
            vecs.Add(v);
        }
        
        public Vector4 Vector4()
        {
            Debug.Assert(typeId[current] == TypeId.Vec);
            return vecs[indexIndicator[current++]];
        }
        public void Push(Vector3 v) => Push((Vector4)v);
        public Vector3 Vector3() => vecs[indexIndicator[current++]];
        public void Push(Vector2 v) => Push((Vector4)v);
        public Vector2 Vector2() => vecs[indexIndicator[current++]];
        public void Push(Color c) => Push(c.ToVec4());
        public Color Color() => vecs[indexIndicator[current++]].ToColor();
        public void Push(Quaternion q) => Push(q.ToVec4());
        public Quaternion Quaternion() => vecs[indexIndicator[current++]].ToQuaternion();
        
        // ============================================================================================================
        // Strings
        // ============================================================================================================
        
        
        [SerializeField]
        List<string> strs = new List<string>();
        
        public void Push(string v)
        {
            typeId.Add(TypeId.Str);
            indexIndicator.Add(strs.Count);
            strs.Add(v);
        }
        
        public string String()
        {
            Debug.Assert(typeId[current] == TypeId.Str);
            return strs[indexIndicator[current++]];
        }
        
        // ============================================================================================================
        // Objects
        // ============================================================================================================
        
        [SerializeField]
        List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
        
        public void Push(UnityEngine.Object v)
        {
            typeId.Add(TypeId.Obj);
            indexIndicator.Add(objs.Count);
            objs.Add(v);
        }
        
        public UnityEngine.Object Object()
        {
            Debug.Assert(typeId[current] == TypeId.Obj);
            return objs[indexIndicator[current++]];
        }
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        
        object ICloneable.Clone() => this.Clone();
        
        public SerializedData Clone()
        {
            var res = new SerializedData();
            res.indexIndicator = new List<int>(indexIndicator);
            res.typeId = new List<TypeId>(typeId);
            res.floats = new List<float>(floats);
            res.ints = new List<int>(ints);
            res.vecs = new List<Vector4>(vecs);
            res.strs = new List<string>(strs);
            res.objs = new List<UnityEngine.Object>(objs);
            res.Reset();
            return res;
        }
        
        public void Clear()
        {
            typeId.Clear();
            indexIndicator.Clear();
            floats.Clear();
            ints.Clear();
            vecs.Clear();
            strs.Clear();
            objs.Clear();
            Reset();
        }
        
        public void Reset()
        {
            current = 0;
        }
    }
}