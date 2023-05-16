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
        public class DFS<T>
        {
            public class StackFrame
            {
                public bool isUsing = false;
                public T node;
                public int currentSub = 0;
                public List<T> subNodes = new List<T>();
                public void Init()
                {
                    isUsing = false;
                    node = default;
                    currentSub = 0;
                    subNodes.Clear();
                }
                public void Take(T node)
                {
                    isUsing = true;
                    this.node = node;
                    currentSub = 0;
                    subNodes.Clear();
                }
                public void Release()
                {
                    isUsing = false;
                    subNodes.Clear();
                }
            }
            
            Action<List<T>> setInitialNodes;
            
            Action<T, List<T>> getNextNodes;
            
            // 当一个点被加入到栈中时调用. 第一个参数是添加它的起点, 第二个参数是被添加到栈上的点.
            Action<T, T> onNodeExtend;
            
            Dictionary<T, bool> reached = new Dictionary<T, bool>();
            
            public bool Reached(T node) => reached[node];
            
            public bool started { get; private set; }
            
            // 处理了总共多少个点.
            public int processed { get; private set; }
            
            // 正在处理的栈帧.
            public int current { get; private set; }
            
            // 正在处理的状态. 栈帧会被复用.
            public readonly List<StackFrame> stack = new List<StackFrame>();
            
            
            
            public DFS()
            {
                Reset();
            }
            
            public void Reset()
            {
                current = -1;
                processed = 0;
                started = false;
                stack.Clear();
            }
            
            public DFS<T> Init(Action<List<T>> setInitialNodes, Action<T, List<T>> getNextNodes)
            {
                this.setInitialNodes = setInitialNodes;
                this.getNextNodes = getNextNodes;
                Reset();
                return this;
            }
            
            public DFS<T> Init(Action<List<T>> setInitialNodes, Action<T, List<T>> getNextNodes, Action<T, T> onNodeExtend)
            {
                this.setInitialNodes = setInitialNodes;
                this.getNextNodes = getNextNodes;
                this.onNodeExtend = onNodeExtend;
                Reset();
                return this;
            }
            
            DFS<T> Execute()
            {
                if(started) return this;
                started = true;
                processed = 0;
                var initialNodes = new List<T>();
                setInitialNodes(initialNodes);
                
                foreach(var node in initialNodes)
                {
                    if(reached.ContainsKey(node)) continue;
                    
                    // initialize stack.
                    stack.Add(new StackFrame());
                    stack[0].Take(node);
                    getNextNodes(node, stack[0].subNodes);
                    reached[node] = true;
                    onNodeExtend?.Invoke(default, node);
                    
                    current = 0;
                    
                    // non-recursive DFS.
                    while(current >= 0)
                    {
                        var frame = stack[current];
                        processed++;
                        
                        // no more sub nodes.
                        if(frame.currentSub >= frame.subNodes.Count)
                        {
                            frame.Release();
                            current--;
                            continue;
                        }
                        
                        var subNode = frame.subNodes[frame.currentSub];
                        frame.currentSub++;
                        
                        if(reached.ContainsKey(subNode)) continue;
                        
                        // add to stack.
                        current++;
                        if(current >= stack.Count) stack.Add(new StackFrame());
                        
                        var newFrame = stack[current];
                        newFrame.Take(subNode);
                        getNextNodes(subNode, newFrame.subNodes);
                        reached[subNode] = true;
                        onNodeExtend?.Invoke(subNode, node);
                        break;
                    }
                    
                    (current == -1).Assert();
                }
                
                return this;
            }
            
        }
        
    }
}
