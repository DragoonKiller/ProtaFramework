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
        
        class Lock : IDisposable
        {
            public int count;
            
            public Lock Get()
            {
                var newValue = Interlocked.Increment(ref count);
                if(newValue != 1)
                {
                    Interlocked.Decrement(ref count);
                    throw new InvalidOperationException("Thread safety broken!");
                }
                
                return this;
            }
            
            void Release() 
            {
                var v = Interlocked.Decrement(ref count);
                (v == 0).Assert();
            }
            
            public void Dispose() => Release();
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
        
        public int maxIterationOnRemove = 200;
        
        public int stepIteration = 200;
        
        public OnWorldNodeActivate onActiveDeactive;
        
        public State state { get; private set; }
        
        readonly Lock lockobj = new Lock(); 
        
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
            lock(targets) list.AddRange(targets);
            
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
            using(lockobj.Get())
            {
                if(state == State.Complete) return;
                if(state == State.None) StartCompute();
                
                bfsShrink.Execute();
                bfsExtend.Execute();
                
                SetToComplete();
            }
        }
        
        public void ComputeStep(int? iteraiton = null)
        {
            using(lockobj.Get())
            {
                ComputeActivateListStep(iteraiton);
            }
        }
        
        void ComputeActivateListStep(int? stepIteration = null)
        {
            var iteration = stepIteration ?? this.stepIteration;
            
            if(state == State.Complete) return;
            if(state == State.None) StartCompute();
            
            bool extendComplete = bfsExtend.ExecuteStep(iteration / 2 + 1);
            bool shrinkComplete = bfsShrink.ExecuteStep(iteration / 2 + 1);
            
            if(extendComplete) bfsShrink.ExecuteStep(iteration / 2);
            if(shrinkComplete) bfsExtend.ExecuteStep(iteration / 2);
            
            if(extendComplete && shrinkComplete) SetToComplete();
        }
        
        
        void ApplyActivateList()
        {
            (state == State.Complete).Assert();
            
            ApplyActivates();
            ApplyEdges();
            
            foreach(var node in toBeActivate) onActiveDeactive?.Invoke(true, node, CancellationToken.None);
            
            toBeDeactivate.Clear();
            toBeActivate.Clear();
            
            state = State.None;
        }
        
        void ApplyActivates()
        {
            // notice: toBeDeactivate collection contains edges that might be activated soon.
            // this inforamtion are kept for edge removing.
            
            // activating nodes that are to be deleted. resume the node.
            toBeRemoved.RemoveRange(toBeActivate);
            
            // new nodes to be deactive.
            foreach(var enode in toBeDeactivate) if(activated.Contains(enode)) toBeRemoved.Add(enode);
            
            // update activate nodes.
            activated.AddRange(toBeActivate);
            
            toBeDeactivate.RemoveRange(toBeActivate);
            activated.RemoveRange(toBeDeactivate);
            
        }
        
        void ApplyEdges()
        {
            edges.RemoveRange(toBeActivate);
            
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
            
        }
        
        void ApplyRemove()
        {
            int i = 0;
            while(i++ < maxIterationOnRemove && toBeRemoved.Count > cacheCount)
            {
                var e = toBeRemoved.FirstElement();
                toBeRemoved.Remove(e);
                onActiveDeactive?.Invoke(false, e, CancellationToken.None);
            }
        }
        
        public async Task ComputeAsync()
        {
            using(lockobj.Get())
            {
                await new SwitchToWorkerThread();
                StartCompute();
                
                var extend = ProtaTask.Run(() => bfsExtend.Execute());
                var shrink = ProtaTask.Run(() => bfsShrink.Execute());
                await Task.WhenAll(extend, shrink);
                
                SetToComplete();
                ApplyActivates();
                ApplyEdges();
                
                await new SwitchToMainThread();
                foreach(var node in toBeActivate) onActiveDeactive?.Invoke(true, node, CancellationToken.None);
                
                await new SwitchToWorkerThread();
                toBeDeactivate.Clear();
                toBeActivate.Clear();
                
                await new SwitchToMainThread();
                ApplyRemove();
                await new SwitchToWorkerThread();
                
                state = State.None;
            }
        }
        
        public IEnumerator<ChunkLayer> CreateStepHandle(int stepIteration, Action<ChunkLayer> getTargets)
        {
            while(true)
            {
                getTargets(this);
                
                using(lockobj.Get())
                {
                    (state == State.None).Assert();
                    StartCompute();
                }
                
                yield return this;
                
                int addition = 0;
                while(true)
                {
                    bool completed = state == State.Complete;
                    if(completed) break;
                    using(lockobj.Get())
                    {
                        (state == State.Computing).Assert();
                        ComputeActivateListStep(stepIteration + addition);
                        addition += 5;
                    }
                    
                    yield return this;
                }
                
                yield return this;
                
                using(lockobj.Get())
                {
                    ApplyActivateList();
                }
                yield return this;
                
                using(lockobj.Get())
                {
                    ApplyRemove();
                }
                
                yield return this;
            }
        }
        
        public void Clear()
        {
            using(lockobj.Get())
            {
                activated.Clear();
                edges.Clear();
                toBeActivate.Clear();
                toBeDeactivate.Clear();
                toBeRemoved.Clear();
                lock(targets) targets.Clear();
                bfsExtend.Reset();
                bfsShrink.Reset();
            }
        }
        
        // ====================================================================================================
        // Target inforamtion
        // ====================================================================================================
        
        public void SetTargetPoints(IEnumerable<Vector3> targetPoints) => SetTargetPoints(targetPoints.Select(x => x.ToVec2()));
        
        public void SetTargetPoints(IEnumerable<Vector2> newTargetPoints)
        {
            lock(targets)
            lock(targetPoints)
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
        }
        
        // ====================================================================================================
        // Utils
        // ====================================================================================================
        
        
        float GetMinDistance(List<Vector2> targetList, WorldNode node)
        {
            lock(targetList)
            {
                var rect = node.Rect(rootPosition, rootSize);
                var distance = float.MaxValue;
                foreach(var p in targetList) distance = distance.Min(rect.EdgeDistanceToPoint(p));
                return distance;
            }
        }
    }
}