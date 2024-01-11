using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    [DefaultExecutionOrder(2500)]
    public class OrderedExecution : Singleton<OrderedExecution>
    {
        public static SortedList<int, HashSet<Action>> updateExecutionList = new();
        
        public static SortedList<int, HashSet<Action>> lateUpdateExecutionList = new();
        
        public static Dictionary<Action, int> priorityMap = new();
        
        void Update()
        {
            foreach (var (priority, actions) in updateExecutionList)
            {
                foreach (var action in actions)
                {
                    action();
                }
            }
        }
        
        void LateUpdate()
        {
            foreach (var (priority, actions) in lateUpdateExecutionList)
            {
                foreach (var action in actions)
                {
                    action();
                }
            }
        }
        
        public static void RegisterUpdate(Action f, int priority)
        {
            OrderedExecution.EnsureExists();
            var p = updateExecutionList.GetOrCreate(priority, () => new HashSet<Action>());
            p.Add(f);
            priorityMap[f] = priority;
        }
        
        public static void RegisterLateUpdate(Action f, int priority)
        {
            OrderedExecution.EnsureExists();
            var p = lateUpdateExecutionList.GetOrCreate(priority, () => new HashSet<Action>());
            p.Add(f);
            priorityMap[f] = priority;
        }
        
        public static void DeregisterUpdate(Action f)
        {
            OrderedExecution.EnsureExists();
            var priority = priorityMap[f];
            var p = updateExecutionList.GetOrCreate(priority, () => new HashSet<Action>());
            p.Remove(f);
            priorityMap.Remove(f);
            if(p.Count == 0) updateExecutionList.Remove(priority);
        }
        
        public static void DeregisterLateUpdate(Action f)
        {
            OrderedExecution.EnsureExists();
            var priority = priorityMap[f];
            var p = lateUpdateExecutionList.GetOrCreate(priority, () => new HashSet<Action>());
            p.Remove(f);
            priorityMap.Remove(f);
            if(p.Count == 0) lateUpdateExecutionList.Remove(priority);
        }
        
        
    }
    
}

