using System;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        public static Vector2 X(this Vector2 a, float x) => new Vector2(x, a.y);
        public static Vector2 Y(this Vector2 a, float y) => new Vector2(a.x, y);
        
        
        public static Vector3 X(this Vector3 a, float x) => new Vector3(x, a.y, a.z);
        public static Vector3 Y(this Vector3 a, float y) => new Vector3(a.x, y, a.z);
        public static Vector3 Z(this Vector3 a, float z) => new Vector3(a.x, a.y, z);
        
        
        public static Vector4 X(this Vector4 a, float x) => new Vector4(x, a.y, a.z, a.w);
        public static Vector4 Y(this Vector4 a, float y) => new Vector4(a.x, y, a.z, a.w);
        public static Vector4 Z(this Vector4 a, float z) => new Vector4(a.x, a.y, z, a.w);
        public static Vector4 W(this Vector4 a, float w) => new Vector4(a.x, a.y, a.z, w);
        
        
        public static Color R(this Color c, float r) => new Color(r, c.g, c.b, c.a);
        public static Color G(this Color c, float g) => new Color(c.r, g, c.b, c.a);
        public static Color B(this Color c, float b) => new Color(c.r, c.g, b, c.a);
        public static Color A(this Color c, float a) => new Color(c.r, c.g, c.b, a);
        
        public static Vector4 ToVec4(this Color c) => new Vector4(c.r, c.g, c.b, c.a);
        
        public static Vector4 ToVec4(this Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);
        
        public static Quaternion ToQuaternion(this Vector4 q) => new Quaternion(q.x, q.y, q.z, q.w); 
        
        
        public static float Abs(this float x) => Mathf.Abs(x);
        public static float Max(this float a, float b) => Mathf.Max(a, b);
        public static float Min(this float a, float b) => Mathf.Min(a, b);
        
        
        
        
        
        public static float XMap(this float x, float a, float b) => (x - a) / (b - a);
        public static float XMap(this float x, float a, float b, float from, float to) => (x - a) / (b - a) * (to - from) + from;
        
        
        
    }
}