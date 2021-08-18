using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Prota.CommonResources
{
    [CustomEditor(typeof(ResourcesDatabase))]
    public class ResourceDatabaseInspector : UnityEditor.Editor
    {
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var modified = false;
            if(GUILayout.Button("刷新"))
            {
                (serializedObject.targetObject as ResourcesDatabase).Setup();
                modified = true;
            }
            if(GUILayout.Button("刷新所有"))
            {
                ResourcesDatabase.RefreshAll();
                modified = true;
            }
            
            if(modified)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
                AssetDatabase.SaveAssets();
            }
        }
    }
}