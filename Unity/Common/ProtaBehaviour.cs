using UnityEngine;
using System;
using System.Collections.Generic;
using Prota;

namespace Prota.Unity
{
    public class ProtaBehaviour : MonoBehaviour
    {
        
        protected List<Action> onDestroy;
        
        void OnDestroy()
        {
            onDestroy?.InvokeAll();
        }
        
    }
}