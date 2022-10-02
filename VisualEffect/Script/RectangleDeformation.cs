using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Prota.Unity;

namespace Prota.VisualEffect
{
    [RequireComponent(typeof(Renderer))]
    public class RectangleDeformation : MonoBehaviour
    {
        public Vector2 coordBottomLeft = new Vector2(0, 0);
        public Vector2 coordBottomRight = new Vector2(1, 0);
        public Vector2 coordTopLeft = new Vector2(0, 1);
        public Vector2 coordTopRight = new Vector2(1, 1);
        
        void Update()
        {
            var mat = this.GetMaterialInstance();
            if(mat == null) return;
            mat.SetVector("_CoordBottomLeft", coordBottomLeft);
            mat.SetVector("_CoordBottomRight", coordBottomRight);
            mat.SetVector("_CoordTopLeft", coordTopLeft);
            mat.SetVector("_CoordTopRight", coordTopRight);
        }
        
    }
}
