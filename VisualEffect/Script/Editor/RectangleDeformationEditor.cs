using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prota.VisualEffect;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;

namespace Prota.Editor
{
    [CustomEditor(typeof(RectangleDeformation))]
    public class RectangleDeformationInspector : UpdateInspector
    {
        RectangleDeformation t => target as RectangleDeformation;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            root.AddChild(new PropertyField(serializedObject.FindProperty("coordBottomLeft")));
            root.AddChild(new PropertyField(serializedObject.FindProperty("coordBottomRight")));
            root.AddChild(new PropertyField(serializedObject.FindProperty("coordTopLeft")));
            root.AddChild(new PropertyField(serializedObject.FindProperty("coordTopRight")));
            
            root.AddChild(new Button(() => {
                t.coordBottomLeft = Vector2.zero;
                t.coordBottomRight = Vector2.right;
                t.coordTopLeft = Vector2.up;
                t.coordTopRight = Vector2.one;
            }){ text = "Set default" });
            
            return root;
        }

        public void OnSceneGUI()
        {
            Undo.RecordObject(t, "SetMesh");
            
            HandleUtility.AddControl(0, 1000);
            
            var sz = t.GetComponent<Renderer>().bounds;
            
            if(sz.size.ToVec2().Area() <= 1e6f) return;
            
            EditorGUI.BeginChangeCheck();
            // TODO: 编辑器的API在2023和2021不一样, 得先给它干掉.
            // t.coordBottomLeft = PosHandle(1, t, t.coordBottomLeft, sz, sz.min);
            // t.coordBottomRight = PosHandle(2, t, t.coordBottomRight, sz, sz.min);
            // t.coordTopLeft = PosHandle(3, t, t.coordTopLeft, sz, sz.min);
            // t.coordTopRight = PosHandle(4, t, t.coordTopRight, sz, sz.min);
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        protected override void Update()
        {
            
        }

        // Vector2 PosHandle(int id, RectangleDeformation t, Vector2 curHandle, Bounds b, Vector2 localOffset)
        // {
        //     var p = curHandle * b.size + localOffset;
        //     
        //     using(var handleContext = HandleContext.Get())
        //     {
        //         Handles.color = new Color(.2f, 1f, .2f, 1);
        //         var size = HandleUtility.GetHandleSize(p) * 0.05f;
        //         var snap = Vector3.one * 0.5f;
        //         var res = Handles.FreeMoveHandle(id, p, size, snap, (controlID, position, rotation, size, eventType) => {
        //             Handles.DotHandleCap(controlID, position, rotation, size, eventType);
        //         });
        //         res -= (Vector3)(localOffset);
        //         res = res.Divide(b.size);
        //         return res;
        //     }
        // }
    }
}
