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
    public sealed class GameInput : Singleton<GameInput>
    {
        PlayerInput cache;
        PlayerInput input => cache == null ? cache = UnityEngine.Object.FindObjectOfType<PlayerInput>() : cache;
        
        public static readonly Dictionary<string, List<Action<InputAction.CallbackContext>>> callbacks
            = new Dictionary<string, List<Action<InputAction.CallbackContext>>>();
        
        
        void Awake()
        {
            input.onActionTriggered += OnTriggered;
        }
        
        void OnDestroy()
        {
            if(input) input.onActionTriggered -= OnTriggered;
        }
        
        void OnTriggered(InputAction.CallbackContext e)
        {
                if(!callbacks.TryGetValue(e.action.name, out var actions)) return;
                foreach(var a in actions) a(e);
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