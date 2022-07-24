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
            t.coordBottomLeft = PosHandle(t, t.coordBottomLeft, sz, sz.min);
            t.coordBottomRight = PosHandle(t, t.coordBottomRight, sz, sz.min);
            t.coordTopLeft = PosHandle(t, t.coordTopLeft, sz, sz.min);
            t.coordTopRight = PosHandle(t, t.coordTopRight, sz, sz.min);
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
        
        Vector2 PosHandle(RectangleDeformation t, Vector2 curHandle, Bounds b, Vector2 localOffset)
        {
            var p = curHandle * b.size + localOffset;
            var res = Handles.PositionHandle(p, Quaternion.identity);
            res -= (Vector3)(localOffset);
            res = res.Divide(b.size);
            return res;
        }
    }
}
