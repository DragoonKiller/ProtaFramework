using UnityEngine;
using Prota.Timer;
using System;
using System.Collections.Generic;

namespace Prota.StateMachine
{
    
    
    public class StateGraph : MonoBehaviour, IDisposable
    {
        StateGraph() { }
        
        public readonly Dictionary<string, StatePoint> points = new Dictionary<string, StatePoint>();
        
        public StatePoint Point(string name)
        {
            if(points.TryGetValue(name, out var res)) return res;
            res = new StatePoint(this, name);
            points.Add(name, res);
            return res;
        }
        
        public StateGraph AddTransferInstant(string from, string to)
        {
            var fromPoint = Point(from);
            var toPoint = Point(to);
            fromPoint.trans.Add(new InstantTransfer(fromPoint, toPoint));
            return this;
        }
        
        public StateGraph AddTransferCondition(string from, string to, Func<bool> check)
        {
            var fromPoint = Point(from);
            var toPoint = Point(to);
            fromPoint.trans.Add(new ConditionalTransfer(fromPoint, toPoint, check));
            return this;
        }
        
        public StateGraph AddOnLeave(string name, Action<StateMachine> onLeave)
        {
            var p = Point(name);
            p.OnLeave += onLeave;
            return this;
        }
        
        public StateGraph AddOnEnter(string name, Action<StateMachine> onEnter)
        {
            var p = Point(name);
            p.OnEnter += onEnter;
            return this;
        }
        
        public StateMachine StartAt(string name, string stateMachineName)
        {
            var s = StateMachine.New(stateMachineName);
            var p = Point(name);
            s.current = p;
            p.StateMachineEnter(s);
            return s;
        }
        
        public bool TryDetach(StateMachine s)
        {
            if(s.current == null) return false;
            if(s.graph != this) return false;
            s.current.StateMachineLeave(s);
            s.current = null;
            return true;
        }
        
        public static StateGraph New(string name)
        {
            var g = new GameObject();
            g.name = "G:" + name;
            g.transform.SetParent(StateMachineManager.instance.transform, false);
            return g.AddComponent<StateGraph>();
        }

        public void Dispose()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
    
    public class StatePoint
    {
        public readonly string name;
        
        public readonly StateGraph graph;
        
        public event Action<StateMachine> OnEnter;
        
        public event Action<StateMachine> OnLeave;
        
        public readonly List<StateTransfer> trans = new List<StateTransfer>();
        
        public readonly HashSet<StateMachine> inPointMachines = new HashSet<StateMachine>(); 
        
        StatePoint() { }
        
        public StatePoint(StateGraph graph, string name)
        {
            this.name = name;
            this.graph = graph;
        }
        
        public void StateMachineEnter(StateMachine s)
        {
            OnEnter?.Invoke(s);
            inPointMachines.Add(s);
        }
        
        public void StateMachineLeave(StateMachine s)
        {
            OnLeave?.Invoke(s);
            inPointMachines.Remove(s);
        }
    }
    
    
    public abstract class StateTransfer
    {
        public readonly StatePoint from;
        public readonly StatePoint to;

        public abstract bool CanTaransfer(StateMachine s);
        public abstract bool TryTransfer(StateMachine m);
        
        private StateTransfer() { }
        
        protected StateTransfer(StatePoint from, StatePoint to)
        {
            this.from = from;
            this.to = to;
        }
        
    }


    public sealed class InstantTransfer : StateTransfer
    {
        public InstantTransfer(StatePoint from, StatePoint to) : base(from, to) { }
        
        public override bool CanTaransfer(StateMachine s) => true;

        public override bool TryTransfer(StateMachine s)
        {
            from.StateMachineEnter(s);
            to.StateMachineEnter(s);
            return true;
        }
    }

    public sealed class ConditionalTransfer : StateTransfer
    {
        public readonly Func<bool> condition;

        public ConditionalTransfer(StatePoint from, StatePoint to, Func<bool> condition) : base(from, to)
        {
            this.condition = condition;
        }

        public override bool CanTaransfer(StateMachine s) => condition();

        public override bool TryTransfer(StateMachine s)
        {
            if(condition())
            {
                from.StateMachineLeave(s);
                to.StateMachineEnter(s);
                return true;
            }
            return false;
        }
    }


    
    public class StateMachine : MonoBehaviour, IDisposable
    {
        public StateGraph graph => current?.graph;
        
        public StatePoint current;
        
        void Update()
        {
            if(null != current) return;
            foreach(var tr in current.trans)
            {
                if(tr.TryTransfer(this)) break;
            }
        }
        
        public static StateMachine New(string name)
        {
            var g = new GameObject();
            g.name = "M:" + name;
            g.transform.SetParent(StateMachineManager.instance.transform, false);
            return g.AddComponent<StateMachine>();
        }

        public void Dispose()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}