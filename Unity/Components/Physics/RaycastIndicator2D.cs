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
        
        [ShowWhen("BoxSelected")] public BoxCollider2D indicatorBox;
        public Vector2 boxSize => indicatorBox.size;
        public Vector3 boxPosition
        {
            get
            {
                var pos = transform.TransformPoint(indicatorBox.offset);
                return pos;
            }
        
        }
        public float boxRotation => indicatorBox.transform.rotation.eulerAngles.z;
        
        
        #if UNITY_EDITOR
        [Header("Debug")]
        public Collider2D colliderHit; 
        #endif
        
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
        
        
        #if UNITY_EDITOR
        void Update()
        {
            if(type == RaycastIndicatorType.Line)
            {
                var myPos = transform.position;
                ProtaDebug.DrawArrow(myPos, myPos + relativePosition.ToVec3(), Color.red);
                if(Cast(out var hit))
                {
                    var hitPoint = hit.point.ToVec3(myPos.z);
                    ProtaDebug.DrawArrow(myPos, hitPoint, Color.green);
                    ProtaDebug.DrawArrow(hitPoint, hitPoint + hit.normal.ToVec3() * 0.4f, Color.blue);
                    colliderHit = hit.collider;
                }
            }
            else if(type == RaycastIndicatorType.Box)
            {
                ProtaDebug.DrawArrow(boxPosition, boxPosition + relativePosition.ToVec3(), Color.red);
                ProtaDebug.DrawBox2D(boxPosition, boxSize, boxRotation, Color.yellow);
                if(Cast(out var hit))
                {
                    var hitPoint = hit.point.ToVec3(boxPosition.z);
                    ProtaDebug.DrawArrow(boxPosition, hitPoint, Color.green);
                    ProtaDebug.DrawArrow(hitPoint, hitPoint + hit.normal.ToVec3() * 0.4f, Color.blue);
                    
                    var hitPos = boxPosition + hit.distance * relativePosition.normalized.ToVec3();
                    ProtaDebug.DrawBox2D(hitPos, boxSize, 0, Color.blue);
                    
                    colliderHit = hit.collider;
                }
            }
        }
        #endif
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        bool BoxSelected() => type == RaycastIndicatorType.Box;
    }
    
}
