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
    // T: node type.
    // E: edge data type.
    // graph edges are single-directional.
    public class Graph<T, E>
    {
        public class EdgeData
        {
            public T from;
            public T to;
            public E data;
        }
        
        public readonly HashSet<T> nodes = new HashSet<T>();
        
        public readonly HashMapSet<T, EdgeData> edges = new HashMapSet<T, EdgeData>();
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public Graph<T, E> AddNode(T node)
        {
            nodes.Add(node);
            return this;
        }
        
        Graph<T, E> AddNode(T from, T to) => AddNode(from).AddNode(to);
        
        public Graph<T, E> AddEdge(T from, T to, bool autoAddNode = true)
        {
            if (autoAddNode) AddNode(from, to);
            else (nodes.Contains(from) && nodes.Contains(to)).Assert();
            var edge = new EdgeData { from = from, to = to, data = default };
            edges.AddElement(from, edge);
            return this;
        }
        
        public Graph<T, E> AddEdge(T from, T to, E data, bool autoAddNode = true)
        {
            if (autoAddNode) AddNode(from, to);
            else (nodes.Contains(from) && nodes.Contains(to)).Assert();
            var edge = new EdgeData { from = from, to = to, data = data };
            edges.AddElement(from, edge);
            return this;
        }
        
        public Graph<T, E> AddEdgeBidirecitonal(T from, T to, bool autoAddNode = true)
        {
            AddEdge(from, to, autoAddNode);
            AddEdge(to, from, autoAddNode);
            return this;
        }
        
        public Graph<T, E> AddEdgeBidirectional(T from, T to, E data, bool autoAddNode = true)
        {
            AddEdge(from, to, data, autoAddNode);
            AddEdge(to, from, data, autoAddNode);
            return this;
        }
        
        public Graph<T, E> AddNodes(IEnumerable<T> nodes)
        {
            foreach (var node in nodes) this.nodes.Add(node);
            return this;
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        Algorithm.DFS<T> DFS(T from)
        {
            var res = new Algorithm.DFS<T>();
            Action<List<T>> setInitialNode = list => list.Add(from);
            Action<T, List<T>> getNextNodes = (node, nextNodes) => {
                foreach (var edge in edges[node]) nextNodes.Add(edge.to);
            };
            res.Init(setInitialNode, getNextNodes);
            return res;            
        }
        
        Algorithm.BFS<T> BFS(T from)
        {
            var res = new Algorithm.BFS<T>();
            Action<List<T>> setInitialNode = list => list.Add(from);
            Action<T, List<T>> getNextNodes = (node, nextNodes) => {
                foreach (var edge in edges[node]) nextNodes.Add(edge.to);
            };
            res.Init(setInitialNode, getNextNodes);
            return res;            
        }
    }
}
