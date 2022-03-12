using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Collections;
using Prota.Unity;

namespace Prota.Input
{
    [RequireComponent(typeof(PlayerInput))]
    [DisallowMultipleComponent]
    public sealed class GameInput : MonoBehaviour
    {
        public static GameInput instance;
        
        PlayerInput input => GetComponent<PlayerInput>();
        
        public readonly Dictionary<string, List<Action<InputAction.CallbackContext>>> callbacks
            = new Dictionary<string, List<Action<InputAction.CallbackContext>>>();
        
        
        void Awake()
        {
            input.onActionTriggered += e => {
                if(!callbacks.TryGetValue(e.action.name, out var actions)) return;
                foreach(var a in actions) a(e); 
            };
        }
        
        void OnDestroy()
        {
            
        }
        
        public void AddCallback(string name, Action<InputAction.CallbackContext> callback)
        {
            callbacks.GetOrCreate(name, out var list);
            if(!list.Contains(callback))
            {
                list.Add(callback);
            }
        }
        

        public void RemoveCallback(string name, Action<InputAction.CallbackContext> callback)
        {
            callbacks.GetOrCreate(name, out var list);
            list.Remove(callback);
        }
    }
}