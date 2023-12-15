using Prota;
using Prota.Unity;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RaycastIndicator2D))]
public class RaycastIndicator2DInspector : Editor
{
    public void OnSceneGUI()
    {
        var raycastIndicator = (RaycastIndicator2D)target;
        var from = raycastIndicator.transform.position.ToVec2();
        var to = from + raycastIndicator.relativePosition;
        
        to = Handles.FreeMoveHandle(
            to.ToVec3(raycastIndicator.transform.position.z),
            0.2f,
            Vector3.zero,
            Handles.SphereHandleCap
        ).ToVec2();
        
        raycastIndicator.relativePosition = to - from;
        
        Undo.RecordObject(raycastIndicator, "RaycastIndicator2D");
        
    }
}
