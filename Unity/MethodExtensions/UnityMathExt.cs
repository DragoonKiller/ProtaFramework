using UnityEngine;
using System;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static float Sqrt(this float x) => Mathf.Sqrt(x);
        public static float Sqrt(this int x) => Mathf.Sqrt(x);
        public static double Sqrt(this double x) => Math.Sqrt(x);
        public static double Sqrt(this long x) => Math.Sqrt(x);
        
        public static float Sin(this float x) => Mathf.Sin(x);
        public static float Cos(this float x) => Mathf.Cos(x);
        public static float Tan(this float x) => Mathf.Tan(x);
        
        public static int NextPowerOfTwo(this int x) => Mathf.NextPowerOfTwo(x);
        
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
        
        public static Vector3 XYToXZ(this Vector2 a, float y = 0) => new Vector3(a.x, y, a.y);
        public static Vector2 XZToXY(this Vector3 a) => new Vector2(a.x, a.z);
        
        public static Vector3 WithLength(this Vector3 a, float len) => a.normalized * len;
        public static Vector2 WithLength(this Vector2 a, float len) => a.normalized * len;
        
        public static Vector2 AddLength(this Vector2 a, float addLen) => a.WithLength(a.magnitude + addLen);
        public static Vector3 AddLength(this Vector3 a, float addLen) => a.WithLength(a.magnitude + addLen);
        
        public static float Angle(this Vector2 a, Vector2 b) => Vector2.SignedAngle(a, b);
        
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
        public static Vector4 Lerp(this (Vector4 from, Vector4 to) p, float x) => p.from + (p.to - p.from) * x;
        public static Color Lerp(this (Color from, Color to) p, float x) => p.from + (p.to - p.from) * x;
        
        public static Vector2 ToVec2(this (float x, float y) a) => new Vector2(a.x, a.y);
        public static Vector3 ToVec3(this (float x, float y, float z) a) => new Vector3(a.x, a.y, a.z);
        
        public static (float x, float y) ToTuple(this Vector2 a) => (a.x, a.y);
        public static (float x, float y, float z) ToTuple(this Vector3 a) => (a.x, a.y, a.z);
        
        public static Vector2 Center(this (Vector2 from, Vector2 to) p) => p.Lerp(0.5f);
        public static Vector3 Center(this (Vector3 from, Vector3 to) p) => p.Lerp(0.5f);
        
        public static Vector2 Vec(this (Vector2 from, Vector2 to) p) => p.to - p.from;
        public static Vector3 Vec(this (Vector3 from, Vector3 to) p) => p.to - p.from;
        
        public static float Length(this (Vector2 from, Vector2 to) p) => p.Vec().magnitude;
        public static float Length(this (Vector3 from, Vector3 to) p) => p.Vec().magnitude;
        
        public static float Area(this Vector2 p) => p.x * p.y;
        
        public static float Volume(this Vector3 p) => p.x * p.y * p.z;
        
        public static Vector2 ToVec2(this Vector3 p) => new Vector2(p.x, p.y);
        public static Vector2 ToVec2(this Vector4 p) => new Vector2(p.x, p.y);
        public static Vector3 ToVec3(this Vector2 p, float z = 0) => new Vector3(p.x, p.y, z);
        public static Vector3 ToVec3(this Vector4 p) => new Vector3(p.x, p.y, p.z);
        public static Vector4 ToVec4(this Vector2 p, float z = 0, float w = 0) => new Vector4(p.x, p.y, z, w);
        public static Vector4 ToVec4(this Vector3 p, float w = 0) => new Vector4(p.x, p.y, p.z, w);
        
        public static int Diff(this (Color32 a, Color32 b) x) => (x.a.r - x.b.r).Abs()
            + (x.a.g - x.b.g).Abs()
            + (x.a.b - x.b.b).Abs()
            + (x.a.a - x.b.a).Abs();
        
        public static Vector2Int FloorToInt(this Vector2 a) => new Vector2Int(a.x.FloorToInt(), a.y.FloorToInt());
        public static Vector3Int FloorToInt(this Vector3 a) => new Vector3Int(a.x.FloorToInt(), a.y.FloorToInt(), a.z.FloorToInt());
        public static Vector2Int CeilToInt(this Vector2 a) => new Vector2Int(a.x.CeilToInt(), a.y.CeilToInt());
        public static Vector3Int CeilToInt(this Vector3 a) => new Vector3Int(a.x.CeilToInt(), a.y.CeilToInt(), a.z.CeilToInt());
        public static Vector2Int RoundToInt(this Vector2 a) => new Vector2Int(a.x.RoundToInt(), a.y.RoundToInt());
        public static Vector3Int RoundToInt(this Vector3 a) => new Vector3Int(a.x.RoundToInt(), a.y.RoundToInt(), a.z.RoundToInt());
        
        public static Vector2 Floor(this Vector2 a) => new Vector2(a.x.Floor(), a.y.Floor());
        public static Vector3 Floor(this Vector3 a) => new Vector3(a.x.Floor(), a.y.Floor(), a.z.Floor());
        public static Vector2 Ceil(this Vector2 a) => new Vector2(a.x.Ceil(), a.y.Ceil());
        public static Vector3 Ceil(this Vector3 a) => new Vector3(a.x.Ceil(), a.y.Ceil(), a.z.Ceil());
        public static Vector2 Round(this Vector2 a) => new Vector2(a.x.Round(), a.y.Round());
        public static Vector3 Round(this Vector3 a) => new Vector3(a.x.Round(), a.y.Round(), a.z.Round());
        
        public static Vector3 Divide(this Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.x, a.z / b.z);
        public static Vector2 Divide(this Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.x);
        
        public static Vector2 Clamp(this Vector2 a, Vector2 min, Vector2 max) => new Vector2(a.x.Clamp(min.x, max.x), a.y.Clamp(min.y, max.y));
        public static Vector3 Clamp(this Vector3 a, Vector3 min, Vector3 max) => new Vector3(a.x.Clamp(min.x, max.x), a.y.Clamp(min.y, max.y), a.z.Clamp(min.z, max.z));
        
        public static Vector2 Rotate(this Vector2 a, float angleInRadian)
        {
            return new Vector2(
                a.x * angleInRadian.Cos() + a.y * angleInRadian.Sin(),
                -a.x * angleInRadian.Sin() + a.y * angleInRadian.Cos()
            );
        }
        
        
        public static Quaternion AsEulerAngle(this (float x, float y, float z) a) => Quaternion.Euler(a.x, a.y, a.z);
        
        
        // Verlet 积分, 用位置相对时间的函数作泰勒展开, 用来精确模拟物理位移/速度/加速度行为
        // https://www.bilibili.com/video/BV1pG411F7b1
        public static float Verlet(this float curPos, float prevPos, float acceleration, float dt)
            => 2 * curPos - prevPos + dt * dt * acceleration;
        public static Vector2 Verlet(this Vector2 curPos, Vector2 prevPos, Vector2 acceleration, float dt)
            => 2 * curPos - prevPos + dt * dt * acceleration;
        public static Vector3 Verlet(this Vector3 curPos, Vector3 prevPos, Vector3 acceleration, float dt)
            => 2 * curPos - prevPos + dt * dt * acceleration;
        
    }
    
}
