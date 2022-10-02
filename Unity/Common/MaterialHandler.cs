using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota
{
    [RequireComponent(typeof(Renderer))]
    public class MaterialHandler : MonoBehaviour
    {
        public Renderer rd => this.GetComponent<Renderer>();
        
        void OnDestroy()
        {
            rd.materials.DestroyAll();
        }
    }
}
