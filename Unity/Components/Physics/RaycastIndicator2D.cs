using System.Collections;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using System.Linq;
using System;

namespace Prota.Unity
{
    public enum RaycastIndicatorType
    {
        Line = 0,
        Box,
    }
    
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class RaycastIndicator2D : MonoBehaviour
    {
        public Vector2 relativePosition = Vector2.down;
        
        public LayerMask layerMask = -1;
        
        public RaycastIndicatorType type;
        
        public Vector3 size;
        
        public bool Cast(out RaycastHit2D hit)
        {
            switch(type)
            {
                case RaycastIndicatorType.Line:
                    hit = Physics2D.Raycast(transform.position, relativePosition, relativePosition.magnitude, layerMask);
                    return hit.collider != null;
                    
                case RaycastIndicatorType.Box:
                    hit = Physics2D.BoxCast(transform.position, size, 0, relativePosition, relativePosition.magnitude, layerMask);
                    return hit.collider != null;
                    
                default:
                    throw new NotImplementedException($" RaycastIndicator 2D at [{this.GetNamePath()}] :: [{ type }]");
            }
        }
        
        
        #if UNITY_EDITOR
        void Update()
        {
            var myPos = transform.position;
            ProtaDebug.DrawArrow(myPos, myPos + relativePosition.ToVec3(), Color.red);
            if(type == RaycastIndicatorType.Box)
            {
                ProtaDebug.DrawBox2D(myPos, size, 0, Color.yellow);
            }
            
            if(Cast(out var hit))
            {
                var hitPoint = hit.point.ToVec3(myPos.z);
                ProtaDebug.DrawArrow(myPos, hitPoint, Color.green);
                ProtaDebug.DrawArrow(hitPoint, hitPoint + hit.normal.ToVec3() * 0.4f, Color.blue);
                
                if(type == RaycastIndicatorType.Box)
                {
                    var hitPos = this.transform.position + hit.distance * relativePosition.normalized.ToVec3();
                    ProtaDebug.DrawBox2D(hitPos, size, 0, Color.blue);
                }
            }
        }
        #endif
    }
    
}
