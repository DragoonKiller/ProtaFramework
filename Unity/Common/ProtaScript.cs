using System;
using UnityEngine;
using Prota.Animation;

namespace Prota.Unity
{
    using DataBinding = Prota.Data.DataBinding;

    // 用来替代 MonoBEhaviour 的基础脚本类型.
    public class ProtaScript : MonoBehaviour
    {
        public DataBinding dataBinding => this.GetComponent<DataBinding>();
        
        public ProtaAnimation anim => this.GetComponent<ProtaAnimation>();
        
    }
}