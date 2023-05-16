using System;

namespace Prota.Unity
{
    // 状态机节点.
    [Serializable]
    public sealed class StateNode
    {
        public string name;
        
        public bool isUpdateState;
        
        public float loopTime;

        public event Action<StateMachine> onEnter;
        
        public event Action<StateMachine> onLeave;
        
        public event Action<StateMachine> onUpdate;
        
        StateNode() { }
        
        public static StateNode New(string name, bool isUpdateState = false, float loopTime = 0)
        {
            if(isUpdateState) (loopTime > 0).Assert();
            
            return new StateNode {
                name = name,
                isUpdateState = isUpdateState,
                loopTime = loopTime
            };
        }
        
        internal void NotifyStateMachineEnter(StateMachine stm) => onEnter?.Invoke(stm);
        internal void NotifyStateMachineLeave(StateMachine stm) => onLeave?.Invoke(stm);
        internal void NotifyStateMachineUpdate(StateMachine stm) => onUpdate?.Invoke(stm);

        public override string ToString() => $"StateNode[{name}]";
    }
}
