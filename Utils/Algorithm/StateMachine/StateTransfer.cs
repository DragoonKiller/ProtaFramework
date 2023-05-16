using System;

namespace Prota.Unity
{
    [Serializable]
    public abstract class StateTransfer
    {
        
        [Serializable]
        public sealed class Instant : StateTransfer
        {
            public Instant(StateNode from, StateNode to) : base(from, to) { }

            public override bool CanTransfer(StateMachine stm) => true;

            public override string ToString() => ToString("Instant");
        }

        [Serializable]
        public sealed class NodeFinish : StateTransfer
        {
            public NodeFinish(StateNode from, StateNode to) : base(from, to) { }

            public override bool CanTransfer(StateMachine stm) => stm.currentTime >= stm.node.loopTime;
            
            public override string ToString() => ToString("Finish");
        }

        [Serializable]
        public sealed class Condition : StateTransfer
        {
            public Func<bool> condition { get; private set; }
            
            public override bool CanTransfer(StateMachine stm) => condition();
            
            public Condition(StateNode from, StateNode to, Func<bool> condition) : base(from, to)
                => this.condition = condition;
                
            public override string ToString() => ToString("Condition");
        }
        
        public readonly StateNode from;
        public readonly StateNode to;
        event Action<StateMachine> onTransfer;
        
        StateTransfer(StateNode from, StateNode to)
        {
            this.from = from;
            this.to = to;
        }
        
        public StateTransfer OnTransfer(Action<StateMachine> a)
        {
            onTransfer += a;
            return this;
        }
        
        public StateTransfer NotifyNodeMoveAlongside(StateMachine stm)
        {
            onTransfer?.Invoke(stm);
            return this;
        }
        
        public abstract bool CanTransfer(StateMachine stm);
        
        StateTransfer(StateNode from, StateNode to, Action<StateMachine> onTransfer) : this(from, to)
            => this.onTransfer = onTransfer;
        
        public static StateTransfer NewInstnat(StateNode from, StateNode to, Action<StateMachine> onTransfer = null)
            => new Instant(from, to).OnTransfer(onTransfer);
            
        public static StateTransfer NewNodeFinish(StateNode from, StateNode to, Action<StateMachine> onTransfer = null)
            => new NodeFinish(from, to).OnTransfer(onTransfer);    
            
        public static StateTransfer NewCondition(StateNode from, StateNode to, Func<bool> condition, Action<StateMachine> onTransfer = null)
            => new Condition(from, to, condition).OnTransfer(onTransfer);

        public override string ToString() => $"StateTransfer[{from.name} -> {to.name}]";
        string ToString(string type) => $"StateTransfer{type}[{from.name} -> {to.name}]";
    }
}
