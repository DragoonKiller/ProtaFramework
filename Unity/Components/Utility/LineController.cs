using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    [RequireComponent(typeof(LineRenderer))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class LineController : MonoBehaviour
    {
        LineRenderer rd => GetComponent<LineRenderer>();
        
        public float circleRadius = 1;
        public int verticesCount = 64;
        public int colliderVerticesCount = 64;
        [SerializeField, EditorButton] bool circle;
        
        public void Update()
        {
            if(circle)
            {
                // Debug.Log("set circle");
                var rd = this.rd;
                circle = false;
                rd.positionCount = verticesCount;
                for(int i = 0; i < rd.positionCount; i++)
                {
                    var angle = 360f * i / rd.positionCount;
                    rd.SetPosition(i, Vector2.one.Rotate(angle).ToVec3() * circleRadius);
                }
                
                if(this.GetComponent<EdgeCollider2D>().PassValue(out var edgeCollider) != null)
                {
                    var points = new Vector2[colliderVerticesCount + 1];
                    for(int i = 0; i < colliderVerticesCount; i++)
                    {
                        var angle = Mathf.PI * 2f * i / colliderVerticesCount;
                        points[i] = Vector2.one.Rotate(angle) * circleRadius;
                    }
                    points[colliderVerticesCount] = points[0];
                    edgeCollider.useAdjacentStartPoint = true;
                    edgeCollider.adjacentStartPoint = points[colliderVerticesCount - 1];
                    edgeCollider.useAdjacentEndPoint = true;
                    edgeCollider.adjacentEndPoint = points[1];
                    edgeCollider.points = points;
                }
            }
        }
    }
    
}
