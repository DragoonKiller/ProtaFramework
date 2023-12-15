using System.Collections;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using System.Linq;
using System;

namespace Prota.Unity
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class RaycastIndicator2D : MonoBehaviour
    {
        public Vector2 relativePosition = Vector2.down;
        
        public LayerMask layerMask = -1;
        
        public bool Cast(out RaycastHit2D hit)
        {
            hit = Physics2D.Raycast(transform.position, relativePosition, relativePosition.magnitude, layerMask);
            return hit.collider != null;
        }
        
        
        #if UNITY_EDITOR
        void Update()
        {
            var myPos = transform.position;
            ProtaDebug.DrawArrow(myPos, myPos + relativePosition.ToVec3(), Color.red);
            if(Cast(out var hit))
            {
                var hitPoint = hit.point.ToVec3(myPos.z);
                ProtaDebug.DrawArrow(myPos, hitPoint, Color.green);
                ProtaDebug.DrawArrow(hitPoint, hitPoint + hit.normal.ToVec3() * 0.4f, Color.blue);
            }
        }
        #endif
    }
    
}
