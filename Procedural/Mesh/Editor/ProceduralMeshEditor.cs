using UnityEngine;
using UnityEditor;


using Prota.Procedural;

namespace Prota.Editor
{

    [CustomEditor(typeof(ProceduralMesh))]
    public class ProceduralMeshEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var t = target as ProceduralMesh;
            Undo.RecordObject(t, "SetMesh");
            
            var g = t.meshGenerator;
            
            if(g is QuadGenerator quadGenerator) QuadGenerator(quadGenerator);
            
        }
        
        void QuadGenerator(QuadGenerator g)
        {
            HandleUtility.AddControl(0, 1000);
            
            var modified = false;
            
            var a = g.bottomLeft;
            g.bottomLeft = Handles.PositionHandle(g.bottomLeft, Quaternion.identity);
            if(a != g.bottomLeft) modified = true;
            
            a = g.bottonRight;
            g.bottonRight = Handles.PositionHandle(g.bottonRight, Quaternion.identity);
            if(a != g.bottonRight) modified = true;
            
            a = g.topLeft;
            g.topLeft = Handles.PositionHandle(g.topLeft, Quaternion.identity);
            if(a != g.topLeft) modified = true;
            
            a = g.topRight;
            g.topRight = Handles.PositionHandle(g.topRight, Quaternion.identity);
            if(a != g.topRight) modified = true;
            
            if(modified)
            {
                g.UpdateMesh();
            }
        }
    }
}
