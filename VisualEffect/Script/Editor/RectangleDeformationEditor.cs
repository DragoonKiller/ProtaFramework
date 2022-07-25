using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prota.VisualEffect;

namespace Prota.Editor
{
    [CustomEditor(typeof(RectangleDeformation))]
    public class RectangleDeformationEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var t = target as RectangleDeformation;
            Undo.RecordObject(t, "SetMesh");
            
            HandleUtility.AddControl(0, 1000);
            
            var sz = t.GetComponent<Renderer>().bounds;
            
            EditorGUI.BeginChangeCheck();
            t.coordBottomLeft = PosHandle(1, t, t.coordBottomLeft, sz, sz.min);
            t.coordBottomRight = PosHandle(2, t, t.coordBottomRight, sz, sz.min);
            t.coordTopLeft = PosHandle(3, t, t.coordTopLeft, sz, sz.min);
            t.coordTopRight = PosHandle(4, t, t.coordTopRight, sz, sz.min);
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
        
        Vector2 PosHandle(int id, RectangleDeformation t, Vector2 curHandle, Bounds b, Vector2 localOffset)
        {
            var p = curHandle * b.size + localOffset;
            
            using(var handleContext = HandleContext.Get())
            {
                Handles.color = new Color(.2f, 1f, .2f, 1);
                var size = HandleUtility.GetHandleSize(p) * 0.1f;
                var snap = Vector3.one * 0.5f;
                var res = Handles.FreeMoveHandle(id, p, size, snap, (controlID, position, rotation, size, eventType) => {
                    Handles.DotHandleCap(controlID, position, rotation, size, eventType);
                });
                res -= (Vector3)(localOffset);
                res = res.Divide(b.size);
                return res;
            }
        }
    }
}
