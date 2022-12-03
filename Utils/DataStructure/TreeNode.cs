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
    public interface CommonTreeNode<T> where T: CommonTreeNode<T>
    {
        public T f { get; }
    }
    
    public interface IBinaryTreeNode<T> : CommonTreeNode<T> where T: IBinaryTreeNode<T>
    {
        public T l { get; }
        
        public T r { get; }
    }
    
    public interface ITreeNode<T> : CommonTreeNode<T> where T: ITreeNode<T>
    {
        // 获取子节点数量.
        public int subCount { get; }
        
        // 获取第 i 个子节点.
        public T this[int index] { get; }
    }
    
    public static class TreeNodeExt
    {
        // 如果不是子节点, 返回 -1.
        public static int SubIndexOf<T>(this T root, T otherNode) where T: ITreeNode<T>
        {
            if(((T)default).Equals(otherNode)) return -1;
            if(((T)default).Equals(root)) return -1;
            for(int i = 0; i < otherNode.subCount; i++) if(otherNode[i].Equals(root)) return i;
            return -1;
        }
        
        public static int SubIndexOfParent<T>(this T root) where T: ITreeNode<T>
        {
            return SubIndexOf(root, root.f);
        }
        
        public static T NextNode<T>(this T root) where T: IBinaryTreeNode<T>
        {
            var x = root.r;
            if(x == null) return x.f.l.Equals(x) ? x.f : default;
            while(x.l != null) x = x.l;
            return x;
        }
        
        public static T PrevNode<T>(this T root) where T: IBinaryTreeNode<T>
        {
            var x = root.l;
            if(x == null) return x.f.r.Equals(x) ? x.f : default;
            while(x.r != null) x = x.r;
            return x;
        }
        
        public static T AssertParent<T>(this T root, T parent) where T: ITreeNode<T>
        {
            if(!root.f.Equals(parent)) throw new Exception("not the parent");
            var i = root.SubIndexOf(parent);
            if(i < 0) throw new Exception("not the parent");
            return root;
        }
        
        public static int Depth<T>(T root) where T: ITreeNode<T>
        {
            for(int i = 0; i < 10000000; i++) 
            {
                if(((T)default).Equals(root)) return i;
                root = root.f;
            }
            throw new InvalidOperationException("depth too large, may have a loop.");
        }
        
        public static int Depth<T>(T root, Dictionary<T, int> cache) where T: ITreeNode<T>
        {
            for(int i = 0; i < 10000000; i++) 
            {
                if(cache.ContainsKey(root)) return cache[root];
                if(((T)default).Equals(root)) return cache[root] = i;
                root = root.f;
            }
            throw new InvalidOperationException("depth too large, may have a loop.");
        }
    }
    
}