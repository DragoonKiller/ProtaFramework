using System;
using UnityEngine;

namespace Prota
{
    public static partial class MethodExtensions
    {
        #if UNITY_2017_1_OR_NEWER
        
        public static Vector2 To(this Vector2 a, Vector2 b) => b - a;
        public static Vector3 To(this Vector3 a, Vector3 b) => b - a;
        public static Vector4 To(this Vector4 a, Vector4 b) => b - a;
        
        public static Vector2 WithX(this Vector2 a, float x) => new Vector2(x, a.y);
        public static Vector2 WithY(this Vector2 a, float y) => new Vector2(a.x, y);
        
        
        public static Vector3 WithX(this Vector3 a, float x) => new Vector3(x, a.y, a.z);
        public static Vector3 WithY(this Vector3 a, float y) => new Vector3(a.x, y, a.z);
        public static Vector3 WithZ(this Vector3 a, float z) => new Vector3(a.x, a.y, z);
        
        
        public static Vector4 WithX(this Vector4 a, float x) => new Vector4(x, a.y, a.z, a.w);
        public static Vector4 WithY(this Vector4 a, float y) => new Vector4(a.x, y, a.z, a.w);
        public static Vector4 WithZ(this Vector4 a, float z) => new Vector4(a.x, a.y, z, a.w);
        public static Vector4 WithW(this Vector4 a, float w) => new Vector4(a.x, a.y, a.z, w);
        
        
        public static ref Vector2 SetX(this ref Vector2 a, float x) { a.x = x; return ref a; }
        public static ref Vector2 SetY(this ref Vector2 a, float y) { a.y = y; return ref a; }
        
        public static ref Vector3 SetX(this ref Vector3 a, float x) { a.x = x; return ref a; }
        public static ref Vector3 SetY(this ref Vector3 a, float y) { a.y = y; return ref a; }
        public static ref Vector3 SetZ(this ref Vector3 a, float z) { a.z = z; return ref a; }
        
        
        public static ref Vector4 SetX(this ref Vector4 a, float x) { a.x = x; return ref a; }
        public static ref Vector4 SetY(this ref Vector4 a, float y) { a.y = y; return ref a; }
        public static ref Vector4 SetZ(this ref Vector4 a, float z) { a.z = z; return ref a; }
        public static ref Vector4 SetW(this ref Vector4 a, float w) { a.w = w; return ref a; }
        
        
        public static float Dot(this Vector2 a, Vector2 b) => Vector2.Dot(a, b);
        public static float Dot(this Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        public static float Dot(this Vector4 a, Vector4 b) => Vector4.Dot(a, b);
        
        
        public static float Cross(this Vector2 a, Vector2 b) => Vector3.Cross((Vector3)a, (Vector3)b).z;
        public static Vector3 Cross(this Vector3 a, Vector3 b) => Vector3.Cross(a, b);
        
        
        public static Color WithR(this Color c, float r) => new Color(r, c.g, c.b, c.a);
        public static Color WithG(this Color c, float g) => new Color(c.r, g, c.b, c.a);
        public static Color WithB(this Color c, float b) => new Color(c.r, c.g, b, c.a);
        public static Color WithA(this Color c, float a) => new Color(c.r, c.g, c.b, a);
        
        public static ref Color SetR(this ref Color c, float r) { c.r = r; return ref c; }
        public static ref Color SetG(this ref Color c, float g) { c.g = g; return ref c; }
        public static ref Color SetB(this ref Color c, float b) { c.b = b; return ref c; }
        public static ref Color SetA(this ref Color c, float a) { c.a = a; return ref c; }
        
        public static Vector4 ToVec4(this Color c) => new Vector4(c.r, c.g, c.b, c.a);
        
        public static Vector4 ToVec4(this Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);
        public static Color ToColor(this Vector4 x) => new Color(x.x, x.y, x.z, x.w);
        
        public static Quaternion ToQuaternion(this Vector4 q) => new Quaternion(q.x, q.y, q.z, q.w); 
        
        public static Vector2 Len(this Vector2 v, float len) => v.normalized * len;
        public static Vector3 Len(this Vector3 v, float len) => v.normalized * len;
        public static Vector4 Len(this Vector4 v, float len) => v.normalized * len;
        
        
        public static Vector2 Lerp(this (Vector2 from, Vector2 to) p, float x) => p.from + (p.to - p.from) * x;
        public static Vector3 Lerp(this (Vector3 from, Vector3 to) p, float x) => p.from + (p.to - p.from) * x;
        
        
        public static Vector2 Center(this (Vector2 from, Vector2 to) p) => p.Lerp(0.5f);
        public static Vector3 Center(this (Vector3 from, Vector3 to) p) => p.Lerp(0.5f);
        
        public static Vector2 Vec(this (Vector2 from, Vector2 to) p) => p.to - p.from;
        public static Vector3 Vec(this (Vector3 from, Vector3 to) p) => p.to - p.from;
        
        public static float Length(this (Vector2 from, Vector2 to) p) => p.Vec().magnitude;
        public static float Length(this (Vector3 from, Vector3 to) p) => p.Vec().magnitude;
        
        
        public static Vector2 ToVec2(this Vector3 p) => new Vector2(p.x, p.y);
        public static Vector2 ToVec2(this Vector4 p) => new Vector2(p.x, p.y);
        public static Vector3 ToVec3(this Vector2 p, float z = 0) => new Vector3(p.x, p.y, z);
        public static Vector3 ToVec3(this Vector4 p) => new Vector3(p.x, p.y, p.z);
        public static Vector3 ToVec4(this Vector2 p, float z = 0, float w = 0) => new Vector4(p.x, p.y, z, w);
        public static Vector3 ToVec4(this Vector3 p, float w = 0) => new Vector4(p.x, p.y, p.z, w);
        
        #endif
        
        
        
        
        
        
        
        
        public static float Abs(this float x) => Mathf.Abs(x);
        public static float Max(this float a, float b) => Mathf.Max(a, b);
        public static float Min(this float a, float b) => Mathf.Min(a, b);
        
        public static int Sign(this float x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        public static float Exp(this float x) => Mathf.Exp(x);
        public static float Pow(this float x, float y) => Mathf.Pow(x, y);
        public static float Floor(this float x) => Mathf.Floor(x);
        public static float Ceil(this float x) => Mathf.Ceil(x);
        
        public static float Mod(this float x, float y) => x - Mathf.Floor(x / y) * y;
        
        public static int Mod(this int x, int y) => x - x / y * y;
        
        public static float Div(this float x, float y)
        {
            return (x - x.Mod(y)) / y;
        }
        
        public static int Div(this int x, int y)
        {
            return (x - x.Mod(y)) / y;
        }
        
        public static bool In(this int x, int a, int b) => a <= x && x <= b;
        
        public static bool InExclusive(this int x, int a, int b) => a < x && x < b;
        
        public static bool In(this float x, float a, float b) => a <= x && x <= b;
        
        public static bool InExclusive(this float x, float a, float b) => a < x && x < b;
        
        public static int FloorToInt(this float x) => Mathf.FloorToInt(x);
        public static int CeilToInt(this float x) => Mathf.CeilToInt(x);
        
        public static float Clamp(this float x, float a, float b) => x < a ? a : x > b ? b : x;
        
        
        public static float XMap(this float x, float a, float b) => (x - a) / (b - a);
        public static float XMap(this float x, float a, float b, float from, float to) => (x - a) / (b - a) * (to - from) + from;
        
        
        
    }
}