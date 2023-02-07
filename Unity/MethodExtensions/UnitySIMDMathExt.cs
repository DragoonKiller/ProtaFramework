using UnityEngine;
using System;
using Unity.Mathematics;

namespace Prota.Unity
{
    public static partial class UnitySIMDMethodExtensions
    {
        public static float2 To(this float2 a, float2 b) => b - a;
        public static float3 To(this float3 a, float3 b) => b - a;
        public static float4 To(this float4 a, float4 b) => b - a;
        
        public static float2 WithX(this float2 a, float x) => new float2(x, a.y);
        public static float2 WithY(this float2 a, float y) => new float2(a.x, y);
        
        
        public static float3 WithX(this float3 a, float x) => new float3(x, a.y, a.z);
        public static float3 WithY(this float3 a, float y) => new float3(a.x, y, a.z);
        public static float3 WithZ(this float3 a, float z) => new float3(a.x, a.y, z);
        
        
        public static float4 WithX(this float4 a, float x) => new float4(x, a.y, a.z, a.w);
        public static float4 WithY(this float4 a, float y) => new float4(a.x, y, a.z, a.w);
        public static float4 WithZ(this float4 a, float z) => new float4(a.x, a.y, z, a.w);
        public static float4 WithW(this float4 a, float w) => new float4(a.x, a.y, a.z, w);
        
        
        public static ref float2 SetX(this ref float2 a, float x) { a.x = x; return ref a; }
        public static ref float2 SetY(this ref float2 a, float y) { a.y = y; return ref a; }
        
        public static ref float3 SetX(this ref float3 a, float x) { a.x = x; return ref a; }
        public static ref float3 SetY(this ref float3 a, float y) { a.y = y; return ref a; }
        public static ref float3 SetZ(this ref float3 a, float z) { a.z = z; return ref a; }
        
        
        public static ref float4 SetX(this ref float4 a, float x) { a.x = x; return ref a; }
        public static ref float4 SetY(this ref float4 a, float y) { a.y = y; return ref a; }
        public static ref float4 SetZ(this ref float4 a, float z) { a.z = z; return ref a; }
        public static ref float4 SetW(this ref float4 a, float w) { a.w = w; return ref a; }
        
        
        public static float Dot(this float2 a, float2 b) => math.dot(a, b);
        public static float Dot(this float3 a, float3 b) => math.dot(a, b);
        public static float Dot(this float4 a, float4 b) => math.dot(a, b);
        
        
        public static float Cross(this float2 a, float2 b) => math.cross(a.ToFloat3(), b.ToFloat3()).z;
        public static float3 Cross(this float3 a, float3 b) => math.cross(a, b);
        
        public static float2 Len(this float2 v, float len) => math.normalize(v) * len;
        public static float3 Len(this float3 v, float len) => math.normalize(v) * len;
        public static float4 Len(this float4 v, float len) => math.normalize(v) * len;
        
        
        public static float2 Lerp(this (float2 from, float2 to) p, float x) => p.from + (p.to - p.from) * x;
        public static float3 Lerp(this (float3 from, float3 to) p, float x) => p.from + (p.to - p.from) * x;
        
        public static float2 Center(this (float2 from, float2 to) p) => p.Lerp(0.5f);
        public static float3 Center(this (float3 from, float3 to) p) => p.Lerp(0.5f);
        
        public static float2 Vec(this (float2 from, float2 to) p) => p.to - p.from;
        public static float3 Vec(this (float3 from, float3 to) p) => p.to - p.from;
        
        public static float Length(this (float2 from, float2 to) p) => math.length(p.Vec());
        public static float Length(this (float3 from, float3 to) p) => math.length(p.Vec());
        
        
        public static float Area(this float2 p) => p.x * p.y;
        
        public static float Volume(this float3 p) => p.x * p.y * p.z;
        
        public static float2 ToFloat2(this float3 p) => new float2(p.x, p.y);
        public static float2 ToFloat2(this float4 p) => new float2(p.x, p.y);
        public static float3 ToFloat3(this float2 p, float z = 0) => new float3(p.x, p.y, z);
        public static float3 ToFloat3(this float4 p) => new float3(p.x, p.y, p.z);
        public static float4 ToFloat4(this float2 p, float z = 0, float w = 0) => new float4(p.x, p.y, z, w);
        public static float4 ToFloat4(this float3 p, float w = 0) => new float4(p.x, p.y, p.z, w);
        
        public static Vector2 ToVec(this float2 p) => (Vector2)p;
        public static Vector3 ToVec(this float3 p) => (Vector3)p;
        public static Vector4 ToVec(this float4 p) => (Vector4)p;
        
        public static Vector2Int ToVec(this int2 p) => new Vector2Int(p.x, p.y);
        public static Vector3Int ToVec(this int3 p) => new Vector3Int(p.x, p.y, p.z);
        public static int2 ToSIMD(this Vector2Int p) => new int2(p.x, p.y);
        public static int3 ToSIMD(this Vector3Int p) => new int3(p.x, p.y, p.z);
        
    }
}