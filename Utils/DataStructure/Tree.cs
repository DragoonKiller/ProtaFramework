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
    public class BinaryTreeNode<Data> : IBinaryTreeNode<BinaryTreeNode<Data>>, ITreeNode<BinaryTreeNode<Data>>
    {
        BinaryTreeNode<Data> _l;
        BinaryTreeNode<Data> _r;

        public BinaryTreeNode<Data> this[int index]
        {
            get
            {
                if(index < 0 || index > 2) throw new ArgumentException("index out of range");
                if(index == 0) return l; else return r;
            }
        }

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
        
        
        public Data data { get; set; }
        
        public int subCount => throw new NotImplementedException();
        
        // 从父节点撤走, 让自己变成根节点.
        void Detach()
        {
            (f != null).Assert();
            if(this.f.l == this) this.f.l = null;
            if(this.f.r == this) this.f.r = null;
            this.f = null;
        }
        
    }
}