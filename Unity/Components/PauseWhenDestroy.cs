using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota
{
    public class PauseWhenDestroy : MonoBehaviour
    {
        public bool activeDestroyOnly = false;
        
        void OnActiveDestroy()
        {
            if(activeDestroyOnly) Debug.Break();
        }
        
        void OnDestroy()
        {
            Debug.Break();
        }
    }
}
