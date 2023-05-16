using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace Prota
{
    public static partial class Algorithm
    {
        // T: node type.
        public class BFS<T>
        {
            Action<List<T>> SetInitialNodes;
            
            Action<T, List<T>> GetNextNodes;
            
            // 节点添加到队列时触发.
            event Action<T> onNodeAdd;
            
            // 节点从队列中取出时触发.
            event Action<T> onNodeExtend;
            
            public bool started { get; private set; }
            
            public int head { get; private set; } 
            
            public bool valid { get; private set; }
            
            readonly List<T> _queue = new List<T>();
            public List<T> queue => valid ? _queue : null;
            
            readonly List<T> _next = new List<T>();
            List<T> next => valid ? _next : null;
            
            readonly HashSet<T> _reached = new HashSet<T>();
            public HashSet<T> reached => valid ? _reached : null;
            
            public int processed => !started ? 0 : head;
            
            public int processing => !started ? 0 : queue.Count - head;
            
            public BFS()
            {
                valid = true;
            }
            
            public BFS<T> Init(Action<List<T>> setInitialNodes, Action<T, List<T>> getNextNodes)
            {
                SetInitialNodes = setInitialNodes;
                GetNextNodes = getNextNodes;
                Reset();
                return this;
            }
            
            public BFS<T> Init(Action<List<T>> setInitialNodes, Action<T, List<T>> getNextNodes, Action<T> onNodeAdd, Action<T> onNodeExtend)
            {
                SetInitialNodes = setInitialNodes;
                GetNextNodes = getNextNodes;
                this.onNodeAdd = onNodeAdd;
                this.onNodeExtend = onNodeExtend;
                Reset();
                return this;
            }
            
            public void Reset()
            {
                head = 0;
                started = false;
                queue.Clear();
                next.Clear();
                reached.Clear();
            }
            
            public void Start()
            {
                Reset();
                StartExecute();
            }
            
            void StartExecute()
            {
                valid.Assert();
                SetInitialNodes.AssertNotNull();
                GetNextNodes.AssertNotNull();
                head = 0;
                SetInitialNodes(queue);
                foreach(var node in queue) reached.Add(node);
                started = true;
            }
            
            // return: completed?
            public bool ExecuteStep(int iteraiton = 500)
            {
                (iteraiton >= 1).Assert();
                for(int i = 0; i < iteraiton - 1 && !Step(); i++);
                return Step();
            }
            
            // return: completed?
            bool Step()
            {
                if(!started) StartExecute();
                
                if(head >= queue.Count) return true;
                
                var cur = queue[head];
                head++;
                
                onNodeExtend?.Invoke(cur);
                
                next.Clear();
                GetNextNodes(cur, next);
                foreach(var node in next)
                {
                    queue.Add(node);
                    reached.Add(node);
                    onNodeAdd?.Invoke(node);
                }
                
                return false;
            }
            
            public void Execute(int maxIteration = 1000000)
            {
                int i = 0;
                for(; i < maxIteration && !Step(); i++);
                if(maxIteration == i) throw new Exception("BFS: iterated too many!");
            }
            
        }
    }
}
