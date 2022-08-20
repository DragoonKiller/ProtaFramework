using System.Runtime.InteropServices;
using UnityEngine;
using System;
using Unity.Burst;
using Unity.Mathematics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Prota.Unity;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

using BFS = Prota.Algorithm.BFS<Prota.WorldTree.WorldNode>;

namespace Prota.WorldTree
{
    public delegate Task OnWorldNodeActivate(bool active, WorldNode node, CancellationToken token);
    
    public class ChunkLayer
    {
        public enum State
        {
            None = 0,
            Computing = 1,
            Complete = 2,
        }
        
        public readonly int level;
        
        public Vector2 rootPosition;
        
        public Vector2 rootSize;
        
        readonly HashSet<WorldNode> activated = new HashSet<WorldNode>();
        
        readonly HashSet<WorldNode> edges = new HashSet<WorldNode>();
        
        readonly HashSet<WorldNode> toBeActivate = new HashSet<WorldNode>();
        
        readonly HashSet<WorldNode> toBeDeactivate = new HashSet<WorldNode>();
        
        readonly HashSet<WorldNode> toBeRemoved = new HashSet<WorldNode>();
        
        readonly HashSet<WorldNode> targets = new HashSet<WorldNode>();
        
        readonly List<Vector2> targetPoints = new List<Vector2>();
        
        
        public IReadOnlyCollection<WorldNode> activeNodes => activated;
        
        public IReadOnlyCollection<WorldNode> edgeNodes => edges;
        
        public IReadOnlyCollection<WorldNode> toBeActiveNodes => toBeActivate;
        
        public IReadOnlyCollection<WorldNode> toBeDeactiveNodes => toBeDeactivate;
        
        public IReadOnlyCollection<WorldNode> toBeRemovedNodes => toBeRemoved;
        
        public IReadOnlyCollection<WorldNode> targetNodes => targets;
        
        public float activeDistance;
        
        public int cacheCount;
        
        readonly BFS bfsExtend = new BFS();
        
        readonly BFS bfsShrink = new BFS();
        
        public IReadOnlyList<WorldNode> processingExtends => bfsExtend.queue;
        
        public int processedExtendCount => bfsExtend.head;
        
        public IReadOnlyList<WorldNode> processingShrinks => bfsShrink.queue;
        
        public int processedShrinkCount => bfsShrink.head;
        
        
        public State state { get; private set; }
        
        public ChunkLayer(int level, Vector2 rootPosition, Vector2 rootHalfSize, float activeDistance)
        {
            this.level = level;
            this.rootPosition = rootPosition;
            this.rootSize = rootHalfSize;
            this.activeDistance = activeDistance;
            bfsExtend.SetInitialNodes = SetInitialNodesExtend;
            bfsExtend.GetNextNodes = GetNextNodesExtend;
            bfsShrink.SetInitialNodes = SetInitialNodesShrink;
            bfsShrink.GetNextNodes = GetNextNodesShrink;
        }
        
        // ====================================================================================================
        // 数据更新和 BFS
        // ====================================================================================================
        
        
        void SetInitialNodesExtend(List<WorldNode> list)
        {
            list.AddRange(targets);
            foreach(var enode in edges)
            {
                WorldNode left = enode.left, right = enode.right, up = enode.up, down = enode.down;
                if(activated.Contains(left)) list.Add(left);
                if(activated.Contains(right)) list.Add(right);
                if(activated.Contains(up)) list.Add(up);
                if(activated.Contains(down)) list.Add(down);
            }
        }
        
        void GetNextNodesExtend(WorldNode node, List<WorldNode> list)
        {
            TryPutNextNodeExtend(list, node.left);
            TryPutNextNodeExtend(list, node.right);
            TryPutNextNodeExtend(list, node.up);
            TryPutNextNodeExtend(list, node.down);
        }
        
        void TryPutNextNodeExtend(List<WorldNode> list, WorldNode node)
        {
            if(bfsExtend.reached.Contains(node)) return;
            if(activated.Contains(node)) return;
            var distance = GetMinDistance(targetPoints, node);
            if(distance <= activeDistance) list.Add(node);
        }
        
        void SetInitialNodesShrink(List<WorldNode> list)
        {
            list.AddRange(edges);
        }
        
        void GetNextNodesShrink(WorldNode node, List<WorldNode> list)
        {
            TryPutNextNodeShrink(list, node.left);
            TryPutNextNodeShrink(list, node.right);
            TryPutNextNodeShrink(list, node.up);
            TryPutNextNodeShrink(list, node.down);
        }
        
        void TryPutNextNodeShrink(List<WorldNode> list, WorldNode node)
        {
            if(bfsShrink.reached.Contains(node)) return;
            if(!activated.Contains(node)) return;
            var distance = GetMinDistance(targetPoints, node);
            if(distance > activeDistance) list.Add(node);
        }
        
        
        void StartCompute()
        {
            (state == State.None).Assert();
            toBeActivate.Clear();
            toBeDeactivate.Clear();
            bfsExtend.Start();
            bfsShrink.Start();
            state = State.Computing;
        }    
        
        void SetToComplete()
        {
            toBeActivate.Clear();
            toBeDeactivate.Clear();
            foreach(var newNode in bfsExtend.queue) if(!activated.Contains(newNode)) toBeActivate.Add(newNode);
            foreach(var newNode in bfsShrink.queue) toBeDeactivate.Add(newNode);
            bfsExtend.Reset();
            bfsShrink.Reset();
            state = State.Complete;
        }
        
        public void ComputeActivateList()
        {
            if(state == State.Complete) return;
            if(state == State.None) StartCompute();
            
            bfsShrink.Execute();
            bfsExtend.Execute();
            
            SetToComplete();
        }
        
        public void ComputeActivateListStep(int stepIteration)
        {
            if(state == State.Complete) return;
            if(state == State.None) StartCompute();
            
            bool extendComplete = bfsExtend.ExecuteStep(stepIteration / 2 + 1);
            bool shrinkComplete = bfsShrink.ExecuteStep(stepIteration / 2 + 1);
            
            if(extendComplete) bfsShrink.ExecuteStep(stepIteration / 2);
            if(shrinkComplete) bfsExtend.ExecuteStep(stepIteration / 2);
            
            if(extendComplete && shrinkComplete) SetToComplete();
        }
        
        
        void ApplyActivateList(OnWorldNodeActivate f = null)
        {
            (state == State.Complete).Assert();
            
            // activating nodes that are to be deleted. resume the node.
            toBeRemoved.RemoveRange(toBeActivate);
            
            // new nodes to be deactive.
            foreach(var enode in toBeDeactivate) if(activated.Contains(enode)) toBeRemoved.Add(enode);
            
            // update activate nodes.
            activated.AddRange(toBeActivate);
            
            toBeDeactivate.RemoveRange(toBeActivate);
            activated.RemoveRange(toBeDeactivate);
            
            edges.RemoveRange(toBeActivate);
            
            // new edge nodes.
            foreach(var node in toBeActivate)
            {
                WorldNode left = node.left, right = node.right, up = node.up, down = node.down;
                if(!activated.Contains(left)) edges.Add(left);
                if(!activated.Contains(right)) edges.Add(right);
                if(!activated.Contains(up)) edges.Add(up);
                if(!activated.Contains(down)) edges.Add(down);
            }
            
            foreach(var node in toBeDeactivate)
            {
                if(!activated.Contains(node) && (activated.Contains(node.left)
                    || activated.Contains(node.right)
                    || activated.Contains(node.up)
                    || activated.Contains(node.down))
                )
                {
                    edges.Add(node);
                }
                else
                {
                    edges.Remove(node);
                }
            }
            
            foreach(var node in toBeActivate) f?.Invoke(true, node, CancellationToken.None);
            
            toBeDeactivate.Clear();
            toBeActivate.Clear();
            
            state = State.None;
        }
        
        public void ApplyRemove(int maxIteration = 200, OnWorldNodeActivate f = null)
        {
            int i = 0;
            while(i++ < maxIteration && toBeRemoved.Count > cacheCount)
            {
                var e = toBeRemoved.FirstElement();
                toBeRemoved.Remove(e);
                f?.Invoke(false, e, CancellationToken.None);
            }
        }
        
        public IEnumerator<ChunkLayer> CreateStepHandle(int stepIteration, Action<ChunkLayer> getTargets)
        {
            while(true)
            {
                (state == State.None).Assert();
                getTargets(this);
                // Console.WriteLine($"Get Targets. count { targets.Count }");
                yield return this;
                
                (state == State.None).Assert();
                getTargets(this);
                StartCompute();
                // Console.WriteLine($"Start compute.");
                yield return this;
                
                (state == State.Computing).Assert();
                int addition = 0;
                while(state != State.Complete)
                {
                    getTargets(this);
                    ComputeActivateListStep(stepIteration + addition);
                    addition += 5;
                    // Console.WriteLine($"Computing [{ bfsExtend.processed } rest { bfsExtend.processing }] [{ bfsShrink.processed } rest { bfsShrink.processing }]");
                    yield return this;
                }
                
                // Console.WriteLine($"Done to be added [{ toBeActivate.Count }] to be removed [{ toBeDeactivate.Count }]");
                
                getTargets(this);
                
                ApplyActivateList();
                // Console.WriteLine($"Done [{ activated.Count }]");
                yield return this;
            }
        }
        
        public void Clear()
        {
            activated.Clear();
            edges.Clear();
            toBeActivate.Clear();
            toBeDeactivate.Clear();
            toBeRemoved.Clear();
            targets.Clear();
            bfsExtend.Reset();
            bfsShrink.Reset();
            
        }
        
        // ====================================================================================================
        // Target inforamtion
        // ====================================================================================================
        
        public void SetTargetPoints(IEnumerable<Vector3> targetPoints) => SetTargetPoints(targetPoints.Select(x => x.ToVec2()));
        
        public void SetTargetPoints(IEnumerable<Vector2> newTargetPoints)
        {
            targetPoints.Clear();
            targetPoints.AddRange(newTargetPoints);
            
            targets.Clear();
            
            foreach(var point in targetPoints)
            {
                var node = WorldNode.GetNodeFromPoint(point, level, rootPosition, rootSize); 
                targets.Add(node);
            }
        }
        
        // ====================================================================================================
        // Utils
        // ====================================================================================================
        
        
        float GetMinDistance(List<Vector2> targetList, WorldNode node)
        {
            var rect = node.Rect(rootPosition, rootSize);
            var distance = float.MaxValue;
            foreach(var p in targetList) distance = distance.Min(rect.EdgeDistanceToPoint(p));
            return distance;
        }
    }
}