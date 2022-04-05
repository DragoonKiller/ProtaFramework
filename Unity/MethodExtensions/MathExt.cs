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
        public static Color ToColor(this Vector4 x) => new Color(x.x, x.y, x.z, x.w);
        
        public static Quaternion ToQuaternion(this Vector4 q) => new Quaternion(q.x, q.y, q.z, q.w); 
        
        
        public static float Abs(this float x) => Mathf.Abs(x);
        public static float Max(this float a, float b) => Mathf.Max(a, b);
        public static float Min(this float a, float b) => Mathf.Min(a, b);
        
        public static float Sign(this float x) => Mathf.Sign(x);
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
        
        public static bool Within(this int x, int a, int b) => a <= x && x <= b;
        
        public static bool WithinExclusive(this int x, int a, int b) => a < x && x < b;
        
        public static bool Within(this float x, float a, float b) => a <= x && x <= b;
        
        public static bool WithinExclusive(this float x, float a, float b) => a < x && x < b;
        
        public static int FloorToInt(this float x) => Mathf.FloorToInt(x);
        public static int CeilToInt(this float x) => Mathf.CeilToInt(x);
        
        public static float Clamp(this float x, float a, float b) => x < a ? a : x > b ? b : x;
        
        
        public static float XMap(this float x, float a, float b) => (x - a) / (b - a);
        public static float XMap(this float x, float a, float b, float from, float to) => (x - a) / (b - a) * (to - from) + from;
        
        
        
        
    }
}