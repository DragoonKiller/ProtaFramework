using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Prota.CommonResources
{
    [CustomEditor(typeof(CommonResource))]
    public class CommonResourcesInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var r = serializedObject.targetObject as CommonResource;
            Undo.RecordObject(r, "ProtaFramework.CommonResource.Refresh");
            if(GUILayout.Button("刷新内容"))
            {
                r.Fill();
                EditorUtility.SetDirty(r);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
    }
    
}