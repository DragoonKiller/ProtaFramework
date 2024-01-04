using UnityEngine;
using UnityEditor;


using Prota.Procedural;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Prota.Editor
{

    [CustomEditor(typeof(ProceduralMesh))]
    public class ProceduralMeshEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var target = this.target as ProceduralMesh;
            target.RegenMesh();
            
            base.OnInspectorGUI();
        }
    }
}
