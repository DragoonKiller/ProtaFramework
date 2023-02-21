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
            // nothing will happend in edit mode, though using materials are not allowed.
            if(!Application.isPlaying) return;
            
            // recently, if materals are not copied, copy it, and remove.
            //otherwise it will be removed directly.
            // *there's no way to findout weather the material is copied or not*
            rd.materials.DestroyAll();
        }
    }
}
