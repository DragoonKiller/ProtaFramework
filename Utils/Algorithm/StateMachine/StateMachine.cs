using System;

namespace Prota.Unity
{
    // 在状态节点(SateNode)和状态转移(StateTransfer)构建的状态图(StateGraph)上游走的状态机.
    [Serializable]
    public sealed class StateMachine
    {
        public string name;
        
        // 当前所在状态图.
        public StateGraph graph;
        
        // 当前所在节点.
        public StateNode node;
        
        // 在当前节点上的标记时间.
        public float currentTime
            => node.isUpdateState && node.loopTime > 0 ? totalTime.Repeat(node.loopTime) : totalTime;
        
        // 在当前节点上经过多久时间了...
        public float totalTime;
        
        public StateMachine(string name, StateGraph graph, StateNode node)
        {
            this.name = name;
            this.graph = graph;
            this.node = node;
            totalTime = 0;
        }
        
        public StateMachine Update(float deltaTime = 0)
        {
            totalTime += deltaTime;
            
            if (node.isUpdateState) node.NotifyStateMachineUpdate(this);
            
            bool nextTransfer = true;
            for(int _ = 0; _ < 10000 && nextTransfer; _++)
            {
                StateTransfer transfer = null;
                foreach(var tr in graph.transfers[node])
                {
                    if (!tr.CanTransfer(this)) continue;
                    transfer = tr;
                    break;
                }
                
                nextTransfer = false;
                if(transfer != null)
                {
                    MoveAlongside(transfer);
                    nextTransfer = true;
                }
                
                if(_ == 9999) throw new Exception("StateMachine.Update() too many transfers in one Update.");
            }
            
            return this;
        }
        
        
        public void MoveAlongside(StateTransfer tr)
        {
            (node == tr.from).Assert();
            tr.from.NotifyStateMachineLeave(this);
            tr.NotifyNodeMoveAlongside(this);
            totalTime = 0;
            tr.to.NotifyStateMachineEnter(this);
        }
        
        public void RemoveFromGraph()
        {
            graph.RemoveStateMachine(this);
            graph = null;
            node = null;
        }
        
        public override string ToString() => $"StateMachine[{name} at {node.name} of {graph.name}]";
        
        public static void UnitTest()
        {
            throw new NotImplementedException(); // TODO
        }
        
    }
}
