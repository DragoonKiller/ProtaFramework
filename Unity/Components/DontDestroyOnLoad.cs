using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    
    [DisallowMultipleComponent]
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
}   
