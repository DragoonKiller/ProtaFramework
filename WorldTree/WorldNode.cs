using System.Runtime.InteropServices;
using UnityEngine;
using System;
using Unity.Burst;
using Unity.Mathematics;
using System.Diagnostics;

namespace Prota.WorldTree
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WorldNode : IEquatable<WorldNode>
    {
        readonly int _level;
        
        readonly float2 _rootSize;
        
        readonly float2 _rootPosition;
        
        readonly int2 _relativeCoord;
        
        public readonly float2 basePosition;
        
        public readonly float2 size;
        
        // depth of this node.
        public int level => _level;
        
        // What's the area the tree control?
        public Vector2 rootSize => _rootSize;
        
        // Where the tree is?
        public Vector2 rootPosition => _rootPosition;
        
        // relative coordinate to tree root.
        // root has coord (0, 0).
        // level 1 is one of topLeft (-1, 1) topRight (1, 1), bottomLeft (-1, -1), bottomRight (1, -1) 
        // level 2 is extend with scale as topLeft(-2, -2), topRight(2, 2) etc. the inner nodes are located at (-1, -1), (1, -1) and so on.
        public Vector2Int relativeCoord => new Vector2Int(_relativeCoord.x, _relativeCoord.y);
        
        // in world position.
        public Rect rect => new Rect(basePosition, size);
        
        public bool isRoot => level == 0;
        
        public WorldNode parent => isRoot ? this
            : new WorldNode(
                level - 1,
                rootSize,
                rootPosition,
                parentCoord
            );
        
        public Vector2Int parentCoord => relativeCoord / 2;
        
        public WorldNode bottomLeft => new WorldNode(level + 1, rootSize, rootPosition, new Vector2Int(Lower(_relativeCoord.x), Lower(relativeCoord.y)));
        
        public WorldNode bottomRight => new WorldNode(level + 1, rootSize, rootPosition, new Vector2Int(Upper(_relativeCoord.x), Lower(relativeCoord.y)));
        
        public WorldNode topLeft => new WorldNode(level + 1, rootSize, rootPosition, new Vector2Int(Lower(_relativeCoord.x), Upper(relativeCoord.y)));
        
        public WorldNode topRight => new WorldNode(level + 1, rootSize, rootPosition, new Vector2Int(Upper(_relativeCoord.x), Upper(relativeCoord.y)));
        
        public WorldNode left => new WorldNode(level, rootSize, rootPosition, new Vector2Int(relativeCoord.x - 1, relativeCoord.y));
        
        public WorldNode right => new WorldNode(level, rootSize, rootPosition, new Vector2Int(relativeCoord.x + 1, relativeCoord.y));
        
        public WorldNode down => new WorldNode(level, rootSize, rootPosition, new Vector2Int(relativeCoord.x, relativeCoord.y - 1));
        
        public WorldNode up => new WorldNode(level, rootSize, rootPosition, new Vector2Int(relativeCoord.x, relativeCoord.y + 1));
        
        public WorldNode(int level, Vector2 rootSize, Vector2 rootPosition, Vector2Int relativeCoord)
        {
            this._level = level;
            this._rootSize = rootSize;
            this._rootPosition = rootPosition;
            this._relativeCoord = relativeCoord.ToSIMD();
            
            BurstAcceleration.GetHalfSize(out var size, level, ref this._rootSize);
            this.size = size;
            
            BurstAcceleration.GetBasePosition(out this.basePosition, level, ref this._rootPosition, ref this._rootSize, ref _relativeCoord);
        }
        
        public Vector2Int GetRelativePositionForPoint(Vector2 point)
        {
            return RelativePosition(level, rootPosition, rootSize, point);
        }
        
        public static Vector2Int RelativePosition(int level, Vector2 rootPosition, Vector2 rootSize, Vector2 point)
        {
            if(level == 0) return new Vector2Int(0, 0);
            point -= rootPosition;
            point = point / rootSize * (1 << level);
            return point.FloorToInt();
        }
        
        public static WorldNode Root(Vector2 centerPosition, Vector2 halfSize)
            => new WorldNode(0, halfSize, centerPosition, Vector2Int.zero);
        
        static int Lower(int x) => x * 2;
        
        static int Upper(int x) => x * 2 + 1;
        
        public bool Equals(WorldNode other)
            => _level == other._level
            && math.all(_rootSize == other._rootSize)
            && math.all(_rootPosition == other._rootPosition)
            && math.all(_relativeCoord == other._relativeCoord);
        
        public override bool Equals(object obj)
        {
            if(!(obj is WorldNode n)) return false;
            return this.Equals(n);
        }
        
        public static bool operator==(WorldNode a, WorldNode b) => a.Equals(b);
        
        public static bool operator!=(WorldNode a, WorldNode b) => !a.Equals(b);
        
        public override int GetHashCode() => HashCode.Combine(_level, _rootSize, _rootPosition, _relativeCoord);

        public override string ToString() => $"WorldNode[{ level }:{ relativeCoord.x },{ relativeCoord.y }|{basePosition}]";
        
        public static void UnitTest()
        {
            var root = WorldNode.Root(new Vector2(-50, -50), new Vector2(100, 100));
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
            
            (pointA.GetRelativePositionForPoint(new Vector2(40, 40)) == new Vector2Int(1, 1)).Assert();
            (pointA.GetRelativePositionForPoint(new Vector2(60, 60)) == new Vector2Int(2, 2)).Assert();
            (pointA.GetRelativePositionForPoint(new Vector2(-1, -1)) == new Vector2Int(0, 0)).Assert();
            
            (pointB.GetRelativePositionForPoint(new Vector2(20, 20)) == new Vector2Int(2, 2)).Assert();
            (pointB.GetRelativePositionForPoint(new Vector2(30, 30)) == new Vector2Int(3, 3)).Assert();
            (pointB.GetRelativePositionForPoint(new Vector2(-20, -20)) == new Vector2Int(1, 1)).Assert();
            (pointB.GetRelativePositionForPoint(new Vector2(-40, -40)) == new Vector2Int(0, 0)).Assert();
            (pointB.GetRelativePositionForPoint(new Vector2(30, -20)) == new Vector2Int(3, 1)).Assert();
            
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