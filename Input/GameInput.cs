using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

using Prota.Unity;

namespace Prota.Input
{
    [RequireComponent(typeof(PlayerInput))]
    [DisallowMultipleComponent]
    public sealed class GameInput : Singleton<GameInput>
    {
        PlayerInput cache;
        PlayerInput input => cache == null ? cache = UnityEngine.Object.FindObjectOfType<PlayerInput>() : cache;
        
        public static readonly HashMapList<string, Action<InputAction.CallbackContext>> callbacks
            = new HashMapList<string, Action<InputAction.CallbackContext>>();
        
        
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
            callbacks.AddElementNoDuplicate(name, callback);
        }
        

        public void RemoveCallback(string name, Action<InputAction.CallbackContext> callback)
        {
            callbacks.RemoveElement(name, callback);
        }
    }
}