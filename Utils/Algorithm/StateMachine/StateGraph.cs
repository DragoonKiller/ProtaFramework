using System;
using System.Linq;
using System.Collections.Generic;


namespace Prota.Unity
{
    // 状态图.
    [Serializable]
    public sealed class StateGraph
    {
        public string name;
        
        public readonly HashMapList<StateNode, StateTransfer> transfers = new HashMapList<StateNode, StateTransfer>();
        
        public readonly PairDictionary<string, StateNode> nodes = new PairDictionary<string, StateNode>();
        
        public readonly List<StateMachine> runningStates = new List<StateMachine>();
        
        public StateGraph AddNode(StateNode node)
        {
            // 重名检查.
            (!nodes.ContainsKey(node.name)).Assert();
            // 重复添加检查.
            (!nodes.ContainsValue(node)).Assert();
            
            nodes.Add(node.name, node);
            return this;
        }
        
        public StateGraph AddNode(string name, bool isUpdateState = false, float loopTime = 0)
            => AddNode(StateNode.New(name, isUpdateState, loopTime));
        
        public StateGraph AddTransfer(StateTransfer transfer)
        {
            if(!nodes.ContainsValue(transfer.from)) AddNode(transfer.from);
            if(!nodes.ContainsValue(transfer.to)) AddNode(transfer.to);
            transfers.AddElement(transfer.from, transfer);
            return this;
        }
        
        public StateGraph AddTransferInstant(StateNode from, StateNode to, Action<StateMachine> onTransfer = null)
            => AddTransfer(StateTransfer.NewInstnat(from, to, onTransfer));
        
        public StateGraph AddTransferCondition(StateNode from, StateNode to, Func<bool> condition, Action<StateMachine> onTransfer = null)
            => AddTransfer(StateTransfer.NewCondition(from, to, condition, onTransfer));
        
        public StateGraph AddTransferFinish(StateNode from, StateNode to, Action<StateMachine> onTransfer = null)
            => AddTransfer(StateTransfer.NewNodeFinish(from, to, onTransfer));
        
        
        public StateMachine StartAt(StateNode node, string name = "UnnamedStateMachine")
        {
            var stm = new StateMachine(name, this, node);
            node.NotifyStateMachineEnter(stm);
            return stm;
        }
        
        public StateMachine StartAt(string nodeName, string stmName = "UnnamedStateMachine")
        {
            if(!nodes.TryGetValue(nodeName, out var node)) throw new Exception($"StateGraph {this} has no node named {nodeName}.");
            return StartAt(node, stmName);
        }
        
        public StateGraph UpdateAllStateMachine(float deltaTime = 0)
        {
            foreach(var stm in runningStates) stm.Update(deltaTime);
            return this;
        }
        
        public bool IsStateMachineOnGraph(StateMachine stm)
            => stm.graph == this && nodes.Contains(stm.node);
        
        public bool RemoveStateMachine(StateMachine stm)
        {
            if(!IsStateMachineOnGraph(stm)) return false;
            stm.node.NotifyStateMachineLeave(stm);
            runningStates.Remove(stm);
            return true;
        }
        
        public Graph<StateNode, StateTransfer> graph
        {
            get
            {
                var g = new Graph<StateNode, StateTransfer>();
                
                foreach(var node in nodes.Values) g.AddNode(node);
                
                foreach(var node in nodes.Values)
                foreach(var tr in transfers[node])
                {
                    g.AddEdge(tr.from, tr.to, tr, false);
                }
                
                return g;
            }
        }

        public override string ToString() => $"StateGraph[{name}]";
    }
}
