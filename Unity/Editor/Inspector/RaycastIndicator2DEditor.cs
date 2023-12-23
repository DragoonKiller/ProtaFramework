using Prota;
using Prota.Unity;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RaycastIndicator2D))]
[CanEditMultipleObjects]
public class RaycastIndicator2DInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Label("Debug");
        var raycastIndicator = (RaycastIndicator2D)target;
        raycastIndicator.Cast(out var hit);
        
        GUI.enabled = false;
        EditorGUILayout.ObjectField("hit", hit.collider, typeof(Collider2D), true);
        EditorGUILayout.FloatField("distance", hit.distance);
        EditorGUILayout.Vector2Field("position", hit.point);
        EditorGUILayout.Vector2Field("normal", hit.normal);
        GUI.enabled = true;
    }
    
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
