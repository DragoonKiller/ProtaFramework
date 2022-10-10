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
    public class BinaryTreeNode<Data>
    {
        BinaryTreeNode<Data> _l;
        BinaryTreeNode<Data> _r;
        
        public BinaryTreeNode<Data> l
        {
            get => _l;
            set
            {
                if(this._l != null) this._l.Detach();
                this._l = value;
                if(value.f != null) value.Detach();
                value.f = this._l;
            }
        }
        
        public BinaryTreeNode<Data> r
        {
            get => _r;
            set
            {
                if(this._r != null) this._r.Detach();
                this._r = value;
                if(value.f != null) value.Detach();
                value.f = this._r;
            }
        }
        
        public BinaryTreeNode<Data> f { get; private set; }
        
        public BinaryTreeNode<Data> leftMost
        {
            get
            {
                var x = this;
                while(x.l != null) x = x.l;
                return x;
            }
        }
        
        public BinaryTreeNode<Data> rightMost
        {
            get
            {
                var x = this;
                while(x.r != null) x = x.r;
                return x;
            }
        }
        
        public BinaryTreeNode<Data> prev
        {
            get
            {
                var x = this.l;
                if(x == null) return x.f.r == x ? x.f : null;
                while(x.r != null) x = x.r;
                return x;
            }
        }
        
        public BinaryTreeNode<Data> next
        {
            get
            {
                var x = this.r;
                if(x == null) return x.f.l == x ? x.f : null;
                while(x.l != null) x = x.l;
                return x;
            }
        }
        
        public Data data { get; set; }
        
        
        // how many nodes are ancients.
        public int Depth(Dictionary<BinaryTreeNode<Data>, int> cache = null)
        {
            if(this.f == null) return 0;
            if(cache != null && cache.TryGetValue(this, out var res)) return res;
            res = this.f.Depth(cache) + 1;
            if(cache != null) cache[this] = res;
            return res;
        }
        
        
        void Detach()
        {
            (f != null).Assert();
            if(this.f.l == this) this.f.l = null;
            if(this.f.r == this) this.f.r = null;
            this.f = null;
        }
        
    }
}