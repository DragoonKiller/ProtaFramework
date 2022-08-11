using UnityEngine;
using UnityEditor;


using Prota.Procedural;

namespace Prota.Editor
{

    [CustomEditor(typeof(ProceduralMesh))]
    public class ProceduralMeshEditor : UnityEditor.Editor
    {
        ProceduralMesh t => target as ProceduralMesh;
        
        public void OnSceneGUI()
        {
            Undo.RecordObject(t, "SetMesh");
            
            var g = t.meshGenerator;
            
            if(g is QuadGenerator quadGenerator) QuadGenerator(quadGenerator);
            
        }
        
        void QuadGenerator(QuadGenerator g)
        {
            HandleUtility.AddControl(0, 1000);
            
            var modified = false;
            
            var a = g.bottomLeft;
            
            Vector3 Transform(Vector3 v)
            {
                return t.transform.InverseTransformPoint(Handles.PositionHandle(t.transform.TransformPoint(v), Quaternion.identity));
            }
            
            modified |= g.bottomLeft.DiffModify(Transform(g.bottomLeft));
            modified |= g.bottonRight.DiffModify(Transform(g.bottonRight));
            modified |= g.topLeft.DiffModify(Transform(g.topLeft));
            modified |= g.topRight.DiffModify(Transform(g.topRight));
            
            // g.bottomLeft = Transform(g.bottomLeft);
            // if(a != g.bottomLeft) modified = true;
            // 
            // a = g.bottonRight;
            // g.bottonRight = Transform(g.bottonRight);
            // if(a != g.bottonRight) modified = true;
            // 
            // a = g.topLeft;
            // g.topLeft = Transform(g.topLeft);
            // if(a != g.topLeft) modified = true;
            // 
            // a = g.topRight;
            // g.topRight = Transform(g.topRight);
            // if(a != g.topRight) modified = true;
            
            if(modified)
            {
                g.UpdateMesh();
            }
        }
    }
}
