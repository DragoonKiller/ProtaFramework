using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.VisualEffect
{
    [ExecuteAlways]
    public class RectangleDeformation : MonoBehaviour
    {
        public Vector2 coordBottomLeft = new Vector2(0, 0);
        public Vector2 coordBottomRight = new Vector2(1, 0);
        public Vector2 coordTopLeft = new Vector2(0, 1);
        public Vector2 coordTopRight = new Vector2(1, 1);
        
        Material mat;
        
        public Renderer rd => this.GetComponent<Renderer>();
        
        void Update()
        {
            if(rd == null) return;
            
            if(mat == null || mat != rd.sharedMaterial)
            {
                mat = new Material(rd.sharedMaterial);
                rd.material = mat;
            }
            
            mat.SetVector("_CoordBottomLeft", coordBottomLeft);
            mat.SetVector("_CoordBottomRight", coordBottomRight);
            mat.SetVector("_CoordTopLeft", coordTopLeft);
            mat.SetVector("_CoordTopRight", coordTopRight);
        }
        
        void OnDestroy()
        {
            if(rd.material == mat)
            {
                DestroyImmediate(mat);
            }
        }
        
        
    }
}
