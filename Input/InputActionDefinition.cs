using UnityEngine;
using System.Collections.Generic;
using System;

namespace Prota.Input
{
    
    public class InputActionDefinition : MonoBehaviour
    {
        [SerializeField]
        public List<string> actionNames;
        
        
        public Dictionary<string, Action> onActionStart = new Dictionary<string, Action>();
        public Dictionary<string, Action> onActionProcess = new Dictionary<string, Action>();
        public Dictionary<string, Action> onActionEnd = new Dictionary<string, Action>();
        
    }
}
