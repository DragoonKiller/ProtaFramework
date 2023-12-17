using System.Collections;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

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
        
        RaycastHit2D[] hitsCache;
        
        [ShowWhen("BoxSelected")] public BoxCollider2D indicatorBox;
        
        [ShowWhen("BoxSelected")] public Vector2 boxExtend = Vector2.zero;
        public Vector2 boxSize => indicatorBox.size + boxExtend;
        public Vector3 boxPosition
        {
            get
            {
                var pos = transform.TransformPoint(indicatorBox.offset);
                return pos;
            }
        
        }
        public float boxRotation => indicatorBox.transform.rotation.eulerAngles.z;
        
        public bool Cast(out RaycastHit2D hit)
        {
            switch(type)
            {
                case RaycastIndicatorType.Line:
                    hit = Physics2D.Raycast(transform.position, relativePosition, relativePosition.magnitude, layerMask);
                    return hit.collider != null;
                    
                case RaycastIndicatorType.Box:
                    hit = Physics2D.BoxCast(boxPosition, boxSize, boxRotation, relativePosition, relativePosition.magnitude, layerMask);
                    return hit.collider != null;
                    
                default:
                    throw new NotImplementedException($" RaycastIndicator 2D at [{this.GetNamePath()}] :: [{ type }]");
            }
        }
        
        public int CastAll(out RaycastHit2D[] hits)
        {
            if(hitsCache == null) hitsCache = new RaycastHit2D[32];
            return CastAllNonAlloc(hits = hitsCache);
        }
        
        public int CastAllNonAlloc(RaycastHit2D[] hits)
        {
            switch(type)
            {
                case RaycastIndicatorType.Line:
                    return Physics2D.RaycastNonAlloc(transform.position, relativePosition, hits, relativePosition.magnitude, layerMask);
                    
                case RaycastIndicatorType.Box:
                    return Physics2D.BoxCastNonAlloc(boxPosition, boxSize, boxRotation, relativePosition, hits, relativePosition.magnitude, layerMask);
                    
                default:
                    throw new NotImplementedException($" RaycastIndicator 2D at [{this.GetNamePath()}] :: [{ type }]");
            }
        }
        
        
        #if UNITY_EDITOR
        void Update()
        {
            if(!Selection.gameObjects.Contains(this.gameObject)) return;
            
            if(type == RaycastIndicatorType.Line)
            {
                var myPos = transform.position;
                ProtaDebug.DrawArrow(myPos, myPos + relativePosition.ToVec3(), Color.red);
                if(Cast(out var hit))
                {
                    var hitPoint = hit.point.ToVec3(myPos.z);
                    ProtaDebug.DrawArrow(myPos, hitPoint, Color.green.WithA(0.5f));
                    ProtaDebug.DrawArrow(hitPoint, hitPoint + hit.normal.ToVec3() * 0.4f, Color.blue);
                }
            }
            else if(type == RaycastIndicatorType.Box)
            {
                ProtaDebug.DrawArrow(boxPosition, boxPosition + relativePosition.ToVec3(), Color.red);
                ProtaDebug.DrawBox2D(boxPosition, boxSize, boxRotation, Color.red.WithA(0.5f));
                ProtaDebug.DrawBox2D(boxPosition + relativePosition.ToVec3(), boxSize, boxRotation, Color.red.WithA(0.5f));
                
                if(Cast(out var hit))
                {
                    var hitPoint = hit.point.ToVec3(boxPosition.z);
                    ProtaDebug.DrawArrow(boxPosition, hitPoint, Color.green.WithA(0.5f));
                    ProtaDebug.DrawArrow(hitPoint, hitPoint + hit.normal.ToVec3() * 0.4f, Color.blue.WithA(0.5f));
                    
                    var hitPos = boxPosition + hit.distance * relativePosition.normalized.ToVec3();
                    ProtaDebug.DrawBox2D(hitPos, boxSize, 0, Color.blue);
                }
            }
        }
        #endif
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        bool BoxSelected() => type == RaycastIndicatorType.Box;
    }
    
}
