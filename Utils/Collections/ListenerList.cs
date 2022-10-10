using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Runtime.Serialization;
using System.Collections;

namespace Prota
{
    public class CallbackList<T>
    {
        public readonly HashSet<Action<T>> list = new HashSet<Action<T>>();
        
        public CallbackList<T> Add(Action<T> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T> Remove(Action<T> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T v1)
        {
            foreach(var c in list) c(v1);
        }
    }
    
    public class CallbackList<T1, T2>
    {
        public readonly HashSet<Action<T1, T2>> list = new HashSet<Action<T1, T2>>();
        
        public CallbackList<T1, T2> Add(Action<T1, T2> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2> Remove(Action<T1, T2> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2)
        {
            foreach(var c in list) c(v1, v2);
        }
    }
    
    
    public class CallbackList<T1, T2, T3>
    {
        public readonly HashSet<Action<T1, T2, T3>> list = new HashSet<Action<T1, T2, T3>>();
        
        public CallbackList<T1, T2, T3> Add(Action<T1, T2, T3> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2, T3> Remove(Action<T1, T2, T3> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2, T3 v3)
        {
            foreach(var c in list) c(v1, v2, v3);
        }
    }
    
    
    public class CallbackList<T1, T2, T3, T4>
    {
        public readonly HashSet<Action<T1, T2, T3, T4>> list = new HashSet<Action<T1, T2, T3, T4>>();
        
        public CallbackList<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2, T3, T4> Remove(Action<T1, T2, T3, T4> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2, T3 v3, T4 v4)
        {
            foreach(var c in list) c(v1, v2, v3, v4);
        }
    }
    
    
    public class CallbackList<T1, T2, T3, T4, T5>
    {
        public readonly HashSet<Action<T1, T2, T3, T4, T5>> list = new HashSet<Action<T1, T2, T3, T4, T5>>();
        
        public CallbackList<T1, T2, T3, T4, T5> Add(Action<T1, T2, T3, T4, T5> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2, T3, T4, T5> Remove(Action<T1, T2, T3, T4, T5> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
        {
            foreach(var c in list) c(v1, v2, v3, v4, v5);
        }
    }
    
    
    public class CallbackList<T1, T2, T3, T4, T5, T6>
    {
        public readonly HashSet<Action<T1, T2, T3, T4, T5, T6>> list = new HashSet<Action<T1, T2, T3, T4, T5, T6>>();
        
        public CallbackList<T1, T2, T3, T4, T5, T6> Add(Action<T1, T2, T3, T4, T5, T6> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2, T3, T4, T5, T6> Remove(Action<T1, T2, T3, T4, T5, T6> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
        {
            foreach(var c in list) c(v1, v2, v3, v4, v5, v6);
        }
    }
    
    
    public class CallbackList<T1, T2, T3, T4, T5, T6, T7>
    {
        public readonly HashSet<Action<T1, T2, T3, T4, T5, T6, T7>> list = new HashSet<Action<T1, T2, T3, T4, T5, T6, T7>>();
        
        public CallbackList<T1, T2, T3, T4, T5, T6, T7> Add(Action<T1, T2, T3, T4, T5, T6, T7> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2, T3, T4, T5, T6, T7> Remove(Action<T1, T2, T3, T4, T5, T6, T7> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
        {
            foreach(var c in list) c(v1, v2, v3, v4, v5, v6,  v7);
        }
    }
    
    
    public class CallbackList<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public readonly HashSet<Action<T1, T2, T3, T4, T5, T6, T7, T8>> list = new HashSet<Action<T1, T2, T3, T4, T5, T6, T7, T8>>();
        
        public CallbackList<T1, T2, T3, T4, T5, T6, T7, T8> Add(Action<T1, T2, T3, T4, T5, T6, T7, T8> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2, T3, T4, T5, T6, T7, T8> Remove(Action<T1, T2, T3, T4, T5, T6, T7, T8> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8)
        {
            foreach(var c in list) c(v1, v2, v3, v4, v5, v6,  v7, v8);
        }
    }
    
    public class CallbackList<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public readonly HashSet<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> list = new HashSet<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>();
        
        public CallbackList<T1, T2, T3, T4, T5, T6, T7, T8, T9> Add(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2, T3, T4, T5, T6, T7, T8, T9> Remove(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9)
        {
            foreach(var c in list) c(v1, v2, v3, v4, v5, v6,  v7, v8, v9);
        }
    }
    
    public class CallbackList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        public readonly HashSet<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> list = new HashSet<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>();
        
        public CallbackList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Add(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> val)
        {
            list.Add(val);
            return this;
        }
        
        public CallbackList<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Remove(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> val)
        {
            list.Remove(val);
            return this;
        }
        
        public void Invoke(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10)
        {
            foreach(var c in list) c(v1, v2, v3, v4, v5, v6,  v7, v8, v9, v10);
        }
    }
    
}