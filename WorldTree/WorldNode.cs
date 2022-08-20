using System.Runtime.InteropServices;
using UnityEngine;
using System;
using Unity.Burst;
using Unity.Mathematics;
using System.Diagnostics;

using Prota.Unity;

namespace Prota.WorldTree
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WorldNode : IEquatable<WorldNode>
    {
        // depth of this node.
        public readonly int level;
        
        readonly int2 _relativeCoord;
        
        // relative coordinate to tree root.
        // root has coord (0, 0).
        // level 1 is one of topLeft (-1, 1) topRight (1, 1), bottomLeft (-1, -1), bottomRight (1, -1) 
        // level 2 is extend with scale as topLeft(-2, -2), topRight(2, 2) etc. the inner nodes are located at (-1, -1), (1, -1) and so on.
        public Vector2Int relativeCoord => new Vector2Int(_relativeCoord.x, _relativeCoord.y);
        
        public bool isRoot => level == 0;
        
        public WorldNode parent => isRoot ? this : new WorldNode(level - 1, parentCoord);
        
        public Vector2Int parentCoord => relativeCoord / 2;
        
        public WorldNode bottomLeft => new WorldNode(level + 1, new Vector2Int(Lower(_relativeCoord.x), Lower(relativeCoord.y)));
        
        public WorldNode bottomRight => new WorldNode(level + 1, new Vector2Int(Upper(_relativeCoord.x), Lower(relativeCoord.y)));
        
        public WorldNode topLeft => new WorldNode(level + 1, new Vector2Int(Lower(_relativeCoord.x), Upper(relativeCoord.y)));
        
        public WorldNode topRight => new WorldNode(level + 1, new Vector2Int(Upper(_relativeCoord.x), Upper(relativeCoord.y)));
        
        public WorldNode left => new WorldNode(level, new Vector2Int(relativeCoord.x - 1, relativeCoord.y));
        
        public WorldNode right => new WorldNode(level, new Vector2Int(relativeCoord.x + 1, relativeCoord.y));
        
        public WorldNode down => new WorldNode(level, new Vector2Int(relativeCoord.x, relativeCoord.y - 1));
        
        public WorldNode up => new WorldNode(level, new Vector2Int(relativeCoord.x, relativeCoord.y + 1));
        
        public static WorldNode root => new WorldNode(0, new Vector2Int(0, 0));
        
        public WorldNode(int level, Vector2Int relativeCoord)
        {
            this.level = level;
            this._relativeCoord = relativeCoord.ToSIMD();
        }
        
        static int Lower(int x) => x * 2;
        
        static int Upper(int x) => x * 2 + 1;
        
        public bool Equals(WorldNode other) => level == other.level && math.all(_relativeCoord == other._relativeCoord);
        
        public override bool Equals(object obj)
        {
            if(!(obj is WorldNode n)) return false;
            return this.Equals(n);
        }
        
        public Vector2 Size(Vector2 rootSize) => rootSize / (1 << level);
        
        public Vector2 BasePosition(Vector2 rootPosition, Vector2 rootSize)
        {
            var rpos = (float2)rootPosition;
            var rsize = (float2)rootSize;
            var rcoord = this._relativeCoord;
            BurstAcceleration.GetBasePosition(out var res, level, ref rpos, ref rsize, ref rcoord);
            return res.ToVec();
        }
        
        public Rect Rect(Vector2 rootPosition, Vector2 rootSize) => new Rect(BasePosition(rootPosition, rootSize), Size(rootSize));
        
        public static bool operator==(WorldNode a, WorldNode b) => a.Equals(b);
        
        public static bool operator!=(WorldNode a, WorldNode b) => !a.Equals(b);
        
        public override int GetHashCode() => HashCode.Combine(level, _relativeCoord);

        public override string ToString() => $"WorldNode[{ level } | { relativeCoord.x },{ relativeCoord.y }]";
        
        public static WorldNode GetNodeFromPoint(Vector2 target, int level, Vector2 rootPos, Vector2 rootSize)
        {
            target -= rootPos;
            target /= rootSize;
            target *= 1 << level;
            var coord = target.FloorToInt();
            return new WorldNode(level, coord);
        }
        
        public static void UnitTest()
        {
            var rootPos = new Vector2(-50, -50);
            var rootSize = new Vector2(100, 100);
            var root = WorldNode.root;
            var pointA = root.bottomLeft;
            var pointB = pointA.bottomLeft;
            var pointC = pointB.bottomLeft;
            var pointD = pointB.topRight;
            (pointA.relativeCoord == new Vector2Int(0, 0)).Assert();
            (pointB.relativeCoord == new Vector2Int(0, 0)).Assert();
            (pointC.relativeCoord == new Vector2Int(0, 0)).Assert();
            (pointD.relativeCoord == new Vector2Int(1, 1)).Assert();
            (pointC.right.relativeCoord == new Vector2Int(1, 0)).Assert();
            var pointE = pointC.right.right.up;
            (pointE.relativeCoord == new Vector2Int(2, 1)).Assert();
            (pointE.parent.relativeCoord == new Vector2Int(1, 0)).Assert();
            // 
            // (pointA.GetRelativePositionForPoint(new Vector2(40, 40)) == new Vector2Int(1, 1)).Assert();
            // (pointA.GetRelativePositionForPoint(new Vector2(60, 60)) == new Vector2Int(2, 2)).Assert();
            // (pointA.GetRelativePositionForPoint(new Vector2(-1, -1)) == new Vector2Int(0, 0)).Assert();
            // 
            // (pointB.GetRelativePositionForPoint(new Vector2(20, 20)) == new Vector2Int(2, 2)).Assert();
            // (pointB.GetRelativePositionForPoint(new Vector2(30, 30)) == new Vector2Int(3, 3)).Assert();
            // (pointB.GetRelativePositionForPoint(new Vector2(-20, -20)) == new Vector2Int(1, 1)).Assert();
            // (pointB.GetRelativePositionForPoint(new Vector2(-40, -40)) == new Vector2Int(0, 0)).Assert();
            // (pointB.GetRelativePositionForPoint(new Vector2(30, -20)) == new Vector2Int(3, 1)).Assert();
            // 
            UnityEngine.Debug.Log("Test Complete!");
            
            
            
            const int repeat = 1000000;
            var s = new Stopwatch();
            s.Start();
            for(int i = repeat; i >= 0; i--)
            {
                var g = pointC.parent;
            }
            s.Stop();
            UnityEngine.Debug.Log($"Performance test: get parent: {(double)s.ElapsedMilliseconds / repeat} ms");
            // 0.000337ms for burst enabled.
            // 0.000647ms for burst disabled.
            
            s.Reset();
            s.Start();
            for(int i = repeat; i >= 0; i--)
            {
                var g = pointC.right;
            }
            s.Stop();
            UnityEngine.Debug.Log($"Performance test: get right: {(double)s.ElapsedMilliseconds / repeat} ms");
            // 0.000292ms for burst enabled.
            // 0.000730ms for burst disabled.
        }
        
        
        [BurstCompile]
        struct BurstAcceleration
        {
            [BurstCompile]
            public static unsafe void GetHalfSize(out float2 result, int level, ref float2 rootSize)
            {
                result = math.pow(0.5f, level) * rootSize;
            }
            
            [BurstCompile]
            public static unsafe void GetBasePosition(out float2 result, int level, ref float2 rootPosition, ref float2 rootSize, ref int2 relativeCoord)
            {
                var size = 1 << level;
                var coord = new float2(relativeCoord) / size;  // normalized to [0, 1] relative coordinates.
                result = rootPosition + rootSize * coord;
            }
        }
    }
}