using System;
using UnityEngine;
using Prota.Animation;
using System.Collections.Generic;

namespace Prota.Unity
{
    using DataBinding = Prota.Data.DataBinding;
    
    // ProtaFramework 框架的基础脚本. 用于替代 MonoBehavkour.
    public abstract class ProtaScript : MonoBehaviour
    {
        public DataBinding dataBinding => this.GetComponent<DataBinding>();
        
        public ProtaAnimation anim => this.GetComponent<ProtaAnimation>();

        
    }
}