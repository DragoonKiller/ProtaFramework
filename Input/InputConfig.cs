using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Prota.Input
{
    
    public class InputConfig : MonoBehaviour
    {
        public enum Mode
        {
            down,
            up,
            pressing,
        }
        
        [Serializable]
        public class Entry
        {
            public string actionName = "";
            
            public KeyCode keyCode;
            
            public Mode mode; 
        }
        
        public InputActionDefinition definition;
        
        [SerializeField]
        public List<Entry> entries = new List<Entry>();
        
        
        Dictionary<string, bool> actionState = new Dictionary<string, bool>();
        Dictionary<string, bool> previousActionState = new Dictionary<string, bool>();
        
        #if UNITY_EDITOR
        public List<string> currentValidActions = new List<string>();
        #endif
        
        void Start()
        {
            AssertKeys();
            RefreshState(actionState);  
        }
        
        void Update()
        {
            (actionState, previousActionState) = (previousActionState, actionState);
            RefreshState(actionState);
            
            #if UNITY_EDITOR
            // UnityEngine.Debug.Log(string.Join("\n", actionState.Select(x => x.Key + " " + x.Value)));
            currentValidActions.Clear();
            currentValidActions.AddRange(actionState.Where(x => x.Value).Select(x => x.Key));
            #endif
            
            foreach(var (key, _) in actionState)
            {
                var exist = actionState.TryGetValue(key, out var activeCurrent);
                activeCurrent = exist && activeCurrent;
                exist = previousActionState.TryGetValue(key, out var activePrevious);
                activePrevious = exist && activePrevious;
                if(activeCurrent && !activePrevious) TriggerAction(key, definition.onActionStart);
                if(activeCurrent && activePrevious) TriggerAction(key, definition.onActionProcess);
                if(!activeCurrent && activePrevious) TriggerAction(key, definition.onActionEnd);
            }
        }
        
        void TriggerAction(string key, Dictionary<string, Action> actions)
        {
            actions.TryGetValue(key, out var action);
            action?.Invoke();
        }
        
        void RefreshState(Dictionary<string, bool> target)
        {
            target.Clear();
            foreach(var entry in entries)
            {
                if(target.TryGetValue(entry.actionName, out var valid) && !valid)
                    continue;
                
                target[entry.actionName] = CheckEntry(entry);
            }
        }
        
        static bool CheckEntry(Entry entry)
        {
            switch(entry.mode)
            {
                case Mode.down: return UnityEngine.Input.GetKeyDown(entry.keyCode);
                case Mode.up: return UnityEngine.Input.GetKeyUp(entry.keyCode);
                case Mode.pressing: return UnityEngine.Input.GetKey(entry.keyCode);
                default:
                return false;
            }
        }
        
        void AssertKeys()
        {
            foreach(var entry in entries)
            {
                bool found = false;
                foreach(var g in definition.actionNames)
                {
                    if(g == entry.actionName)
                    {
                        found = true;
                        break;
                    }
                }
                UnityEngine.Debug.Assert(found, entry.actionName);
            }
        }
        
    }
}
