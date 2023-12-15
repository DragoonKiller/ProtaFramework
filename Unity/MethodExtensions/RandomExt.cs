using UnityEngine;
using System;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        
        
        public static int SegmentRandom(this float x)
        {
            (x >= 0).Assert();
            var b = x.FloorToInt();
            x -= b;
            return b + ((0f, 1f).Random() <= x ? 1 : 0);
        }
        
        public static int Random(this (int a, int b) v)
        {
            return UnityEngine.Random.Range(v.a, v.b);
        }
        
        public static float Random(this (float a, float b) v)
        {
            return UnityEngine.Random.Range(v.a, v.b);
        }
        
        public static Vector2Int Random(this (Vector2Int a, Vector2Int b) v)
        {
            return new Vector2Int(
                UnityEngine.Random.Range(v.a.x, v.b.x),
                UnityEngine.Random.Range(v.a.y, v.b.y)
            );
        }
        
        public static Vector3Int Random(this (Vector3Int a, Vector3Int b) v)
        {
            return new Vector3Int(
                UnityEngine.Random.Range(v.a.x, v.b.x),
                UnityEngine.Random.Range(v.a.y, v.b.y),
                UnityEngine.Random.Range(v.a.z, v.b.z)
            );
        }
        
        public static Vector2 Random(this (Vector2 a, Vector2 b) v)
        {
            return new Vector2(
                UnityEngine.Random.Range(v.a.x, v.b.x),
                UnityEngine.Random.Range(v.a.y, v.b.y)
            );
        }
        
        public static Vector3 Random(this (Vector3 a, Vector3 b) v)
        {
            return new Vector3(
                UnityEngine.Random.Range(v.a.x, v.b.x),
                UnityEngine.Random.Range(v.a.y, v.b.y),
                UnityEngine.Random.Range(v.a.z, v.b.z)
            );
        }
        
        public static Vector4 Random(this (Vector4 a, Vector4 b) v)
        {
            return new Vector4(
                UnityEngine.Random.Range(v.a.x, v.b.x),
                UnityEngine.Random.Range(v.a.y, v.b.y),
                UnityEngine.Random.Range(v.a.z, v.b.z),
                UnityEngine.Random.Range(v.a.w, v.b.w)
            );
        }
        
        public static Color Random(this (Color a, Color b) v)
        {
            return new Color(
                UnityEngine.Random.Range(v.a.r, v.b.r),
                UnityEngine.Random.Range(v.a.g, v.b.g),
                UnityEngine.Random.Range(v.a.b, v.b.b),
                UnityEngine.Random.Range(v.a.a, v.b.a)
            );
        }
        
        public static Vector2 RandomCircle(this Vector2 a, float radius) => (a, radius).RandomCircle();
        public static Vector2 RandomCircle(this (Vector2 a, float radius) v)
        {
            var angle = UnityEngine.Random.Range(90, 360f) * Mathf.Rad2Deg;
            return Vector2.one.Rotate(angle) * UnityEngine.Random.Range(0f, v.radius).Sqrt() + v.a;
        }
        
        public static Vector2 RandomCircleEdge(this Vector2 a, float radius) => (a, radius).RandomCircleEdge();
        public static Vector2 RandomCircleEdge(this (Vector2 a, float radius) v)
        {
            var angle = UnityEngine.Random.Range(90, 360f) * Mathf.Rad2Deg;
            return Vector2.one.Rotate(angle) * v.radius + v.a;
        }
        
        public static Vector3 RandomSphere(this Vector3 a, float dist) => (a, dist).RandomSphere();
        public static Vector3 RandomSphere(this (Vector3 a, float dist) v)
        {
            var q = Quaternion.Euler(
                UnityEngine.Random.Range(0, 360f),
                UnityEngine.Random.Range(0, 360f),
                UnityEngine.Random.Range(0, 360f)
            );
            return q * Vector3.one * UnityEngine.Random.Range(0f, v.dist).Pow(1/3f) + v.a;
        }
        
        public static Vector3 RandomSphereSurface(this Vector3 a, float dist) => (a, dist).RandomSphereSurface();
        public static Vector3 RandomSphereSurface(this (Vector3 a, float dist) v)
        {
            var q = Quaternion.Euler(
                UnityEngine.Random.Range(0, 360f),
                UnityEngine.Random.Range(0, 360f),
                UnityEngine.Random.Range(0, 360f)
            );
            return q * Vector3.one * v.dist + v.a;
        }
    }
}
