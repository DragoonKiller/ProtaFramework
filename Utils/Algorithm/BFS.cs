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
    public static class Algorithm
    {
        // T: node type.
        public class  BFS<T>
        {
            public Action<List<T>> SetInitialNodes;
            
            public Action<T, List<T>> GetNextNodes;
            
            public Action<T> OnNodeAdd;
            
            public Action<T> OnNodeExtend;
            
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
                this.valid = true;
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
                
                OnNodeExtend?.Invoke(cur);
                
                next.Clear();
                GetNextNodes(cur, next);
                foreach(var node in next)
                {
                    queue.Add(node);
                    reached.Add(node);
                    OnNodeAdd?.Invoke(node);
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