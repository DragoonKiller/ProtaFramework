using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        // 4向分离, 从0开始, 逆时针.
        // 返回值为0, 1, 2, 3, 分别表示右, 上, 左, 下.
        public static int DirectionPartition4(this Vector2 dir)
        {
            var angle = Vector2.SignedAngle(Vector2.right, dir);
            if(angle < 0) angle += 360;
            if(angle < 45f) return 0;
            if(angle < 135f) return 1;
            if(angle < 225f) return 2;
            if(angle < 315f) return 3;
            return 0;
        }
        
        // 4向分离.
        public static Vector2Int DirectionNormalize4(this Vector2 dir)
        {
            var partition = dir.DirectionPartition4();
            switch (partition)
            {
                case 0: return Vector2Int.right;
                case 1: return Vector2Int.up;
                case 2: return Vector2Int.left;
                case 3: return Vector2Int.down;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        // 8向分离, 从0开始, 逆时针.
        // 返回值为0, 1, 2, 3, 4, 5, 6, 7, 分别表示右, 右上, 上, 左上, 左, 左下, 下, 右下.
        public static int DirectionPartition8(this Vector2 dir)
        {
            var angle = Vector2.SignedAngle(Vector2.right, dir);
            if(angle < 0) angle += 360;
            if(angle < 22.5f) return 0;
            if(angle < 67.5f) return 1;
            if(angle < 112.5f) return 2;
            if(angle < 157.5f) return 3;
            if(angle < 202.5f) return 4;
            if(angle < 247.5f) return 5;
            if(angle < 292.5f) return 6;
            if(angle < 337.5f) return 7;
            return 0;
        }
        
        // 8向分离.
        public static Vector2Int DirectionNormalize8(this Vector2 dir)
        {
            var partition = dir.DirectionPartition8();
            switch (partition)
            {
                case 0: return Vector2Int.right;
                case 1: return new Vector2Int(1, 1);
                case 2: return Vector2Int.up;
                case 3: return new Vector2Int(-1, 1);
                case 4: return Vector2Int.left;
                case 5: return new Vector2Int(-1, -1);
                case 6: return Vector2Int.down;
                case 7: return new Vector2Int(1, -1);
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static string DirectionPartitionName4(this Vector2 dir)
        {
            var partition = dir.DirectionPartition4();
            switch (partition)
            {
                case 0: return "Right";
                case 1: return "Up";
                case 2: return "Left";
                case 3: return "Down";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static string DirectionPartitionName8(this Vector2 dir)
        {
            var partition = dir.DirectionPartition8();
            switch (partition)
            {
                case 0: return "Right";
                case 1: return "UpRight";
                case 2: return "Up";
                case 3: return "UpLeft";
                case 4: return "Left";
                case 5: return "DownLeft";
                case 6: return "Down";
                case 7: return "DownRight";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static string DirectionPartitionToName8(this int dir)
        {
            switch (dir)
            {
                case 0: return "Right";
                case 1: return "UpRight";
                case 2: return "Up";
                case 3: return "UpLeft";
                case 4: return "Left";
                case 5: return "DownLeft";
                case 6: return "Down";
                case 7: return "DownRight";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static string DirectionPartitionToName4(this int dir)
        {
            switch (dir)
            {
                case 0: return "Right";
                case 1: return "Up";
                case 2: return "Left";
                case 3: return "Down";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static int DirectionNameToPartition4(this string dir)
        {
            switch (dir)
            {
                case "Right": return 0;
                case "Up": return 1;
                case "Left": return 2;
                case "Down": return 3;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static int DirectionNameToPartition8(this string dir)
        {
            switch (dir)
            {
                case "Right": return 0;
                case "UpRight": return 1;
                case "Up": return 2;
                case "UpLeft": return 3;
                case "Left": return 4;
                case "DownLeft": return 5;
                case "Down": return 6;
                case "DownRight": return 7;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        
        public static bool PointInTriangle(this Vector2 x, Vector2 a, Vector2 b, Vector2 c)
        {
            var areaABC = Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2f);
            var areaPBC = Mathf.Abs((x.x * (b.y - c.y) + b.x * (c.y - x.y) + c.x * (x.y - b.y)) / 2f);
            var areaPCA = Mathf.Abs((a.x * (x.y - c.y) + x.x * (c.y - a.y) + c.x * (a.y - x.y)) / 2f);
            var areaPAB = Mathf.Abs((a.x * (b.y - x.y) + b.x * (x.y - a.y) + x.x * (a.y - b.y)) / 2f);
            return Mathf.Approximately(areaABC, areaPBC + areaPCA + areaPAB);
        }
        
        public static Vector2Int StepLeft(this Vector2Int a) => new Vector2Int(a.x - 1, a.y);
        public static Vector2Int StepRight(this Vector2Int a) => new Vector2Int(a.x + 1, a.y);
        public static Vector2Int StepUp(this Vector2Int a) => new Vector2Int(a.x, a.y + 1);
        public static Vector2Int StepDown(this Vector2Int a) => new Vector2Int(a.x, a.y - 1);
        public static Vector2Int StepUpLeft(this Vector2Int a) => new Vector2Int(a.x - 1, a.y + 1);
        public static Vector2Int StepUpRight(this Vector2Int a) => new Vector2Int(a.x + 1, a.y + 1);
        public static Vector2Int StepDownLeft(this Vector2Int a) => new Vector2Int(a.x - 1, a.y - 1);
        public static Vector2Int StepDownRight(this Vector2Int a) => new Vector2Int(a.x + 1, a.y - 1);
        
        public static Vector3Int StepLeft(this Vector3Int a) => new Vector3Int(a.x - 1, a.y, a.z);
        public static Vector3Int StepRight(this Vector3Int a) => new Vector3Int(a.x + 1, a.y, a.z);
        public static Vector3Int StepUp(this Vector3Int a) => new Vector3Int(a.x, a.y + 1, a.z);
        public static Vector3Int StepDown(this Vector3Int a) => new Vector3Int(a.x, a.y - 1, a.z);
        public static Vector3Int StepForward(this Vector3Int a) => new Vector3Int(a.x, a.y, a.z + 1);
        public static Vector3Int StepBack(this Vector3Int a) => new Vector3Int(a.x, a.y, a.z - 1);
        
        
        public static float ManhattanLength(this Vector2 a) => a.x + a.y;
        public static float ManhattanLength(this Vector3 a) => a.x + a.y + a.z;
        
        public static float ManhattanDistance(this Vector2 a, Vector2 b) => (a - b).ManhattanLength();
        public static float ManhattanDistance(this Vector3 a, Vector3 b) => (a - b).ManhattanLength();
        
        // 从a到b的两条曼哈顿路径的转折点.
        public static (Vector2 a, Vector2 b) ManhattanCorner(this Vector2 a, Vector2 b)
            => (new Vector2(a.x, b.y), new Vector2(b.x, a.y));
        
        public static float Sqrt(this float x) => Mathf.Sqrt(x);
        public static float Sqrt(this int x) => Mathf.Sqrt(x);
        public static double Sqrt(this double x) => Math.Sqrt(x);
        public static double Sqrt(this long x) => Math.Sqrt(x);
        
        public static float Sin(this float x) => Mathf.Sin(x);
        public static float Cos(this float x) => Mathf.Cos(x);
        public static float Tan(this float x) => Mathf.Tan(x);
        
        public static float PingPong(this float x, float a) => Mathf.PingPong(x, a);
        
        public static bool IsZero(this Vector2 a) => a == Vector2.zero;
        public static bool IsZero(this Vector3 a) => a == Vector3.zero;
        public static bool IsZero(this Vector4 a) => a == Vector4.zero;
        
        public static Vector2 Abs(this Vector2 a) => new Vector2(Mathf.Abs(a.x), Mathf.Abs(a.y));
        public static Vector3 Abs(this Vector3 a) => new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
        
        public static Vector2 To(this Vector2 a, Vector2 b) => b - a;
        public static Vector3 To(this Vector3 a, Vector3 b) => b - a;
        public static Vector4 To(this Vector4 a, Vector4 b) => b - a;
        
        
        public static Vector2 WithX(this Vector2 a, float x) => new Vector2(x, a.y);
        public static Vector2 WithY(this Vector2 a, float y) => new Vector2(a.x, y);
        
        
        public static Vector3 WithX(this Vector3 a, float x) => new Vector3(x, a.y, a.z);
        public static Vector3 WithY(this Vector3 a, float y) => new Vector3(a.x, y, a.z);
        public static Vector3 WithZ(this Vector3 a, float z) => new Vector3(a.x, a.y, z);
        
        public static Vector3 WithXY(this Vector3 a, Vector2 xy) => new Vector3(xy.x, xy.y, a.z);
        
        
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
        
        public static Vector2 WithMaxLength(this Vector2 a, float maxLen) => a.normalized * a.magnitude.Max(maxLen);
        public static Vector3 WithMaxLength(this Vector3 a, float maxLen) => a.normalized * a.magnitude.Max(maxLen);
        
        public static Vector2 WithMinLength(this Vector2 a, float maxLen) => a.normalized * a.magnitude.Min(maxLen);
        public static Vector3 WithMinLength(this Vector3 a, float maxLen) => a.normalized * a.magnitude.Min(maxLen);
        
        public static Vector2 AddLength(this Vector2 a, float addLen) => a.WithLength(a.magnitude + addLen);
        public static Vector3 AddLength(this Vector3 a, float addLen) => a.WithLength(a.magnitude + addLen);
        
        public static Vector2 MoveTowards(this Vector2 a, Vector2 b, float maxDistanceDelta)
            => Vector2.MoveTowards(a, b, maxDistanceDelta);
            
        public static Vector3 MoveTowards(this Vector3 a, Vector3 b, float maxDistanceDelta)
            => Vector3.MoveTowards(a, b, maxDistanceDelta);
        
        public static float Angle(this Vector2 a, Vector2 b) => Vector2.SignedAngle(a, b);
        
        public static float Dot(this Vector2 a, Vector2 b) => Vector2.Dot(a, b);
        public static float Dot(this Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        public static float Dot(this Vector4 a, Vector4 b) => Vector4.Dot(a, b);
        
        public static float Cross(this Vector2 a, Vector2 b) => Vector3.Cross((Vector3)a, (Vector3)b).z;
        public static Vector3 Cross(this Vector3 a, Vector3 b) => Vector3.Cross(a, b);
        
        public static Color Add(this Color a, Color b) => new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        
        public static Color Sub(this Color p, Color q) => new Color(p.r - q.r, p.g - q.g, p.b - q.b, p.a - q.a);
        
        public static Color WithRGB(this Color c, Color r) => new Color(r.r, r.g, r.b, c.a);
        
        public static Color WithR(this Color c, float r) => new Color(r, c.g, c.b, c.a);
        public static Color WithG(this Color c, float g) => new Color(c.r, g, c.b, c.a);
        public static Color WithB(this Color c, float b) => new Color(c.r, c.g, b, c.a);
        public static Color WithA(this Color c, float a) => new Color(c.r, c.g, c.b, a);
        
        public static ref Color SetR(this ref Color c, float r) { c.r = r; return ref c; }
        public static ref Color SetG(this ref Color c, float g) { c.g = g; return ref c; }
        public static ref Color SetB(this ref Color c, float b) { c.b = b; return ref c; }
        public static ref Color SetA(this ref Color c, float a) { c.a = a; return ref c; }
        
        public static string ToWebString(this Color c) => ColorUtility.ToHtmlStringRGBA(c);
        public static Color ToColor(this string str) => ColorUtility.TryParseHtmlString(str, out Color c) ? c : Color.clear;
        
        public static Vector4 ToVec4(this Color c) => new Vector4(c.r, c.g, c.b, c.a);
        
        public static Vector4 ToVec4(this Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);
        public static Color ToColor(this Vector4 x) => new Color(x.x, x.y, x.z, x.w);
        
        public static Quaternion ToQuaternion(this Vector4 q) => new Quaternion(q.x, q.y, q.z, q.w); 
        
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
        
        public static float Perimiter(this Vector2 p) => (p.x + p.y) * 2;
        
        public static float Volume(this Vector3 p) => p.x * p.y * p.z;
        
        public static float SurfaceArea(this Vector3 p) => (p.x * p.y + p.x * p.z + p.y * p.z) * 2;
        
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
        
        public static Vector2 Rotate(this Vector2 a, float angleInDegree)
        {
            var rad = angleInDegree * Mathf.Deg2Rad;
            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);
            return new Vector2(a.x * cos - a.y * sin, a.x * sin + a.y * cos);
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
        
        
        public static Vector3 ProjectToPerpendicularPlane(this Vector3 a, Vector3 normal)
            => a - Vector3.Project(a, normal);
        
        public static Vector3 Project(this Vector3 a, Vector3 normal)
            => Vector3.Project(a, normal);
        
        public static Vector2 ProjectToPerpendicularLine(this Vector2 a, Vector2 normal)
            => a - a.Project(normal);
        
        public static Vector2 Project(this Vector2 a, Vector2 normal)
            => a.Dot(normal.normalized) * normal.normalized;
        
        
        
        public static bool IsParallel(this Vector2 a, Vector2 b)
            => a.Cross(b).ApproximatelyEqual(0);
        
        public static bool IsParallel(this Vector3 a, Vector3 b)
            => a.Cross(b).IsZero();
        
        public static bool IsPerpendicular(this Vector2 a, Vector2 b)
            => a.Dot(b).ApproximatelyEqual(0);
        
        public static bool IsPerpendicular(this Vector3 a, Vector3 b)
            => a.Dot(b).ApproximatelyEqual(0);
        
        
        public static IEnumerable<Vector2> AverageDivide(this (Vector2 from, Vector2 to) v, int count)
        {
            (count > 0).Assert();
            for (var i = 0; i < count; i++) yield return v.from + (v.to - v.from) * (i + 1) / (count + 1);
        }
        
        public static IEnumerable<Vector3> AverageDivide(this (Vector3 from, Vector3 to) v, int count)
        {
            (count > 0).Assert();
            for (var i = 0; i < count; i++) yield return v.from + (v.to - v.from) * (i + 1) / (count + 1);
        }
        
        
        public static float SmoothStep(this float x) => x * x * (3 - 2 * x);
        
        public static float ToDegree(this float x) => x * Mathf.Rad2Deg;
        
        public static float ToRadian(this float x) => x * Mathf.Deg2Rad;
        
        public static Vector2Int Abs(this Vector2Int a) => new Vector2Int(a.x.Abs(), a.y.Abs());
        
        public static Vector3Int Abs(this Vector3Int a) => new Vector3Int(a.x.Abs(), a.y.Abs(), a.z.Abs());
        
        public static Vector2Int Min(this Vector2Int a, Vector2Int b) => new Vector2Int(a.x.Min(b.x), a.y.Min(b.y));
        
        public static Vector3Int Min(this Vector3Int a, Vector3Int b) => new Vector3Int(a.x.Min(b.x), a.y.Min(b.y), a.z.Min(b.z));
        
        public static Vector2Int Max(this Vector2Int a, Vector2Int b) => new Vector2Int(a.x.Max(b.x), a.y.Max(b.y));
        
        public static Vector3Int Max(this Vector3Int a, Vector3Int b) => new Vector3Int(a.x.Max(b.x), a.y.Max(b.y), a.z.Max(b.z));
        
        public static float MaxComponent(this Vector2 a) => a.x.Max(a.y);
        
        public static float MaxComponent(this Vector3 a) => a.x.Max(a.y).Max(a.z);
        
        public static float MinComponent(this Vector2 a) => a.x.Min(a.y);
        
        public static float MinComponent(this Vector3 a) => a.x.Min(a.y).Min(a.z);
        
        public static int MaxComponent(this Vector2Int a) => a.x.Max(a.y);
        
        public static int MaxComponent(this Vector3Int a) => a.x.Max(a.y).Max(a.z);
        
        public static int MinComponent(this Vector2Int a) => a.x.Min(a.y);
        
        public static int MinComponent(this Vector3Int a) => a.x.Min(a.y).Min(a.z);
        
        public static float SumAllComponents(this Vector2 a) => a.x + a.y;
        
        public static float SumAllComponents(this Vector3 a) => a.x + a.y + a.z;
        
        public static int SumAllComponents(this Vector2Int a) => a.x + a.y;
        
        public static int SumAllComponents(this Vector3Int a) => a.x + a.y + a.z;
        
        
        
        // 切比雪夫距离
        public static int DistanceChebyshev(this Vector2Int a, Vector2Int b)
            => (a - b).Abs().MaxComponent();
        
        public static int DistanceChebyshev(this Vector3Int a, Vector3Int b)
            => (a - b).Abs().MaxComponent();
            
        public static float DistanceChebyshev(this Vector2 a, Vector2 b)
            => (a - b).Abs().MaxComponent();
            
        public static float DistanceChebyshev(this Vector3 a, Vector3 b)
            => (a - b).Abs().MaxComponent();
            
        public static int DistanceManhattan(this Vector2Int a, Vector2Int b)
            => (a - b).Abs().SumAllComponents();
            
        public static int DistanceManhattan(this Vector3Int a, Vector3Int b)
            => (a - b).Abs().SumAllComponents();
            
        public static float DistanceManhattan(this Vector2 a, Vector2 b)
            => (a - b).Abs().SumAllComponents();
            
        public static float DistanceManhattan(this Vector3 a, Vector3 b)
            => (a - b).Abs().SumAllComponents();
        
        public static float Distance(this Vector2 a, Vector2 b)
            => (a - b).magnitude;
        
        public static float Distance(this Vector3 a, Vector3 b)
            => (a - b).magnitude;
            
        public static float Distance(this Vector2Int a, Vector2Int b)
            => (a - b).magnitude;
        
        public static float Distance(this Vector3Int a, Vector3Int b)
            => (a - b).magnitude;
            
        
        public static Vector2Int ToVec2Int(this Vector3Int a) => new Vector2Int(a.x, a.y);
        
        public static Vector3Int ToVec3Int(this Vector2Int a, int z = 0) => new Vector3Int(a.x, a.y, z);
        
        public static Vector2 HitXYPlane(this Ray a, float z = 0)
        {
            var t = -(a.origin.z - z) / a.direction.z;
            return a.origin + a.direction * t;
        }
        
        
        public static Vector2 SnapTo(this Vector2 p, Vector2 snap)
        {
            return p.SnapTo(snap, Vector2.zero);
        }
        
        public static Vector2 SnapTo(this Vector2 p, Vector2 snap, Vector2 origin)
        {
            if(snap.x <= 1e-12f || snap.y <= 1e-12f) return p;
            var delta = p - origin;
            var x = Mathf.Round(delta.x / snap.x) * snap.x;
            var y = Mathf.Round(delta.y / snap.y) * snap.y;
            return origin + new Vector2(x, y);
        }
    }
    
}
