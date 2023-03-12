using System;
using System.Collections.Generic;

namespace Prota
{
    // 关于为什么不用字符串做 key:
    // 因为传参必须要确定类型...
    
    // 用法:
    // struct EventStruct : IEvent { }
    // Notify<EventStruct>() { }
    // Register<EventStruct>(callback)
    
    internal abstract class CallbackSlotBase
    {
        internal readonly static Dictionary<Type, CallbackSlotBase> instances = new Dictionary<Type, CallbackSlotBase>();
        public abstract bool Register(object x);
        public abstract void Call(object x);
        public abstract void Remove(object x);
        public abstract void Clear();
    }
    
    internal sealed class CallbackSlot<T> : CallbackSlotBase
    {
        // 可以调用一下以确保这个类初始化了.
        internal static Type type => typeof(T);
        
        public static CallbackSlot<T> instance;
        
        public readonly HashSet<Action<T>> actions = new HashSet<Action<T>>();
        
        public CallbackSlot()
        {
            // 我们使用 Activator 凭空创建一个对象.
            // 把自己的全局实例注册进 CallbackSlot 列表里.
            (instance == null).Assert();
            instance = this;
            CallbackSlotBase.instances.Add(typeof(T), this);
        }
        
        public override void Call(object x)
        {
            var v = (T)x;
            foreach(var i in actions) i.Invoke(v);
        }

        // 如果已经在集合里, 返回 false.
        public override bool Register(object x)
        {
            return actions.Add((Action<T>)x);
        }

        public override void Clear()
        {
            actions.Clear();
        }

        public override void Remove(object x)
        {
            actions.Remove((Action<T>)x);
        }
    }
    
    internal enum EventOperationType
    {
        None = 0,
        Notify = 1,
        Register = 2,
        Remove = 3,
        RemoveAll = 4,
        ClearAll = 5,
    }
    
    internal struct EventOperation
    {
        public long id;
        public EventOperationType op;
        public Type targetType;
        public object callback;
        public object arg;
    }
    
    public partial class EventQueue
    {
        static EventQueue _inst;
        public static EventQueue instance => _inst == null ? _inst = new EventQueue() : _inst;
        
        public bool executing { get; private set; }
        
        public int limit = 500000;
        
        public Queue<string> logs = null;
        
        public long id = 0;
        
        public bool log
        {
            get => logs != null;
            set => logs = value ? new Queue<string>() : null;
        }
        
        public int logLimit = 10000;
        
        public string logString => logs == null ? "No log." : string.Join("\n", logs);
        
        internal readonly Queue<EventOperation> queue = new Queue<EventOperation>();
        
        void EnsureCallbackSlotExists(Type type)
        {
            if(CallbackSlotBase.instances.TryGetValue(type, out _)) return;
            Type a = typeof(CallbackSlot<>);
            Type closedType = a.MakeGenericType(type);
            object instance = Activator.CreateInstance(closedType);
            CallbackSlotBase.instances.TryGetValue(type, out _).Assert();
        }
        
        public void AddCallback<T>(Action<T> val) where T: IEvent
        {
            EnsureCallbackSlotExists(typeof(T));
            var opData = new EventOperation() {
                id = ++id,
                op = EventOperationType.Register,
                targetType = typeof(T),
                callback = val,
            };
            Log("add", opData);
            Execute(opData);
        }
        public void RemoveCallback<T>(Action<T> val) where T: IEvent
        {
            EnsureCallbackSlotExists(typeof(T));
            var opData = new EventOperation() {
                id = ++id,
                op = EventOperationType.Remove,
                targetType = typeof(T),
                callback = val,
            };
            Log("remove", opData);
            Execute(opData);
        }
        
        public void ClearCallback<T>() where T: IEvent
        {
            EnsureCallbackSlotExists(typeof(T));
            var opData = new EventOperation() {
                id = ++id,
                op = EventOperationType.RemoveAll,
                targetType = typeof(T),
            };
            Log("clear", opData);
            Execute(opData);
        }
        
        public void ClearAll()
        {
            var opData = new EventOperation() {
                id = ++id,
                op = EventOperationType.ClearAll,
            };
            Log("clear all", opData);
            Execute(opData);
        }
        
        /// <summary>
        /// useInstanceType = true: 使用 val 的实际对象类型作为回调的类型.
        /// useInstanceType = false: 使用类型 T 作为回调的类型.
        /// </summary>
        public void Notify<T>(T val = default, bool useInstanceType = true) where T: IEvent
        {
            var t = useInstanceType ? val.GetType() : typeof(T);
            EnsureCallbackSlotExists(t);
            
            var opData = new EventOperation() {
                id = ++id,
                op = EventOperationType.Notify,
                targetType = t,
                arg = val,
            };
            Log("notify", opData);
            Execute(opData);
        }
        
        // 尝试原地执行事件.
        // 如果后续有事件, 或者队列处于运作状态, 则使用队列继续执行.
        internal void Execute(EventOperation opData)
        {
            queue.Enqueue(opData);
            
            // 启动队列.
            if(!executing)
            {
                executing = true;
                for(int i = 0; i < limit && queue.TryDequeue(out var x); i++)
                {
                    ExecuteStep(x);
                }
                (executing == true).Assert();
                executing = false;
            }
        }
        
        
        // 立即执行该事件.
        internal void ExecuteStep(EventOperation x)
        {
            Log("exec", x);
            
            EnsureCallbackSlotExists(x.targetType);
            
            switch(x.op)
            {
                case EventOperationType.Notify:
                CallbackSlotBase.instances[x.targetType].Call(x.arg);
                break;
                
                case EventOperationType.Register:
                CallbackSlotBase.instances[x.targetType].Register(x.callback);
                break;
                
                case EventOperationType.Remove:
                CallbackSlotBase.instances[x.targetType].Remove(x.callback);
                break;
                
                case EventOperationType.RemoveAll:
                CallbackSlotBase.instances[x.targetType].Clear();
                break;
                
                case EventOperationType.ClearAll:
                foreach(var slot in CallbackSlotBase.instances) slot.Value.Clear();
                break;
                
                default: throw new ArgumentException();
            }
        }
        
        void Log(string desc, EventOperation x)
        {
            if(logs == null) return;
            logs.Enqueue($"##{x.id} ::: {desc}:{ x.op }:{ x.targetType } :: { x.callback } | { x.arg } \n{ Environment.StackTrace.Replace("at ", ">>> ") }");
            if(logs.Count > logLimit) logs.TryDequeue(out _);
        }
    }
    
    
    
    public static class EventQueueExt
    {
        public static IEvent Notify(this IEvent e, bool useInstanceType = true)
        {
            EventQueue.instance.Notify(e, useInstanceType);
            return e;
        }
    }
    
    
}
