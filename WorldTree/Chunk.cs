using System.Runtime.InteropServices;
using UnityEngine;
using System;
using Unity.Burst;
using Unity.Mathematics;
using System.Diagnostics;
using System.Collections.Generic;
using Prota.Unity;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace Prota.WorldTree
{
    public delegate Task OnWorldNodeActivate(bool active, WorldNode node, CancellationToken token);
    
    // A chunk is a real state data of world tree at runtime.
    [ExecuteAlways]
    public class Chunk : MonoBehaviour
    {
        [field: NonSerialized]
        public bool inited { get; private set; } = false;
        
        [field: NonSerialized]
        public bool multithreadUpdate { get; set; } = true;
        
        [field: NonSerialized]
        public int framePerTick { get; set; } = 3;
        
        [field: NonSerialized]
        public int updatePerTickLimit { get; set; } = 500;
        
        int _maxDepth = 1;
        public int maxDepth
        {
            get => _maxDepth;
            set
            {
                if(inited) throw new InvalidOperationException("Cannot set maxDepth after initialization.");
                if(value <= 0) throw new InvalidOperationException("Cannot set maxDepth <= 0.");
                _maxDepth = value;
            }
        }
        
        float _rootHalfSize = 1;
        public float rootHalfSize
        {
            get => _rootHalfSize;
            set
            {
                if(inited) throw new InvalidOperationException("Cannot set rootHalfSize after initialization.");
                if(value <= 0) throw new InvalidOperationException("Cannot set rootHalfSize <= 0.");
                _rootHalfSize = value;
            }
        }
        
        public Vector2 rootPosition { get; private set; }
        
        public HashSet<WorldNode>[] activateList { get; private set; }
        
        public HashSet<WorldNode>[] edgeList { get; private set; }
        
        public List<Transform> transformTargetList;
        
        public List<Vector3> pointTargetList;
        
        public List<float> activateDistance;
        
        List<Vector2> allTargetList = new List<Vector2>();
        
        Action<int> updateLevelDelegate;
        
        public event OnWorldNodeActivate onActivate;
        
        void Start()
        {
            (!inited).Assert();
            Init();
        }
        
        void Update()
        {
            if(Application.isPlaying && !(Time.frameCount % framePerTick == framePerTick - 1)) return;
            UpdateAll();
        }
        
        public Chunk Init()
        {
            if(inited) return this;
            inited = true;
            if(updateLevelDelegate == null) updateLevelDelegate = i => UpdateLevel(i);
            activateList = new HashSet<WorldNode>[maxDepth].Fill(i => new HashSet<WorldNode>());
            edgeList = new HashSet<WorldNode>[maxDepth].Fill(i => new HashSet<WorldNode>());
            rootPosition = this.transform.position;
            return this;
        }
        
        public Chunk Reset()
        {
            inited = false;
            activateList = null;
            edgeList = null;
            return this;
        }
        
        public Chunk UpdateAll()
        {
            if(!inited) Init();
            
            allTargetList.Clear();
            if(pointTargetList != null) allTargetList.AddRange(pointTargetList.Select(x => x.ToVec2()));
            if(transformTargetList != null) allTargetList.AddRange(transformTargetList.Select(x => x.position.ToVec2()));
            
            if(multithreadUpdate)
            {
                Parallel.For(0, maxDepth, updateLevelDelegate);
            }
            else
            {
                for(int i = 0; i < maxDepth; i++) UpdateLevel(i);
            }
            return this;
        }
        
        public Chunk UpdateLevel(int level)
        {
            (inited).Assert();
            
            (0 <= level && level < maxDepth).Assert();
            var activates = activateList[level];
            var edges = edgeList[level];
            using(var queueHandle = TempList<WorldNode>.Get())
            using(var usedHandle = TempHashSet<WorldNode>.Get())
            using(var initialNodesHandle = TempList<WorldNode>.Get())
            {
                var initialNodes = initialNodesHandle.value;
                
                // BFS Out.
                var queue = queueHandle.value;
                var used = usedHandle.value;
                foreach(var point in allTargetList)
                {
                    var rpos = WorldNode.RelativePosition(level, rootPosition, new Vector2(rootHalfSize, rootHalfSize), point);
                    var node = new WorldNode(level, new Vector2(rootHalfSize, rootHalfSize), rootPosition, rpos); 
                    TryAdd(queue, used, node);
                }
                
                foreach(var enode in edges)
                {
                    TryAdd(queue, used, enode);
                }
                
                int head = 0;
                while(head < queue.Count)
                {
                    var cur = queue[head];
                    var distance = GetMinDistance(allTargetList, cur);
                    
                    head++;
                }
                
                // BFS In.
                 
                
            }
            
            return this;
        }
        
        static bool TryAdd(List<WorldNode> queue, HashSet<WorldNode> used, WorldNode node)
        {
            if(used.Contains(node)) return false;
            queue.Add(node);
            used.Add(node);
            return true;
        }
        
        static float GetMinDistance(List<Vector2> targetList, WorldNode node)
        {
            var rect = node.rect;
            var distance = float.MaxValue;
            foreach(var p in targetList) distance = distance.Min(rect.EdgeDistanceToPoint(p));
            return distance;
        }
    }
}