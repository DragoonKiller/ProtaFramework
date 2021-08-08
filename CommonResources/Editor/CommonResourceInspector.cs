using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Prota.CommonResources
{
    [CustomEditor(typeof(CommonResource))]
    public class CommonResourcesInspector : UnityEditor.Editor
    {
        static readonly HashSet<string> validExt = new HashSet<string> {
            ".png",
            ".jpg",
            ".gif",
            ".txt",
        };
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var r = serializedObject.targetObject as CommonResource;
            Undo.RecordObject(r, "ProtaFramework.CommonResource.Refresh");
            if(GUILayout.Button("刷新内容"))
            {
                FillAll(r);
                EditorUtility.SetDirty(r);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
        }
        
        public static void FillAll(CommonResource c)
        {
            c.Clear();
            
            var path = AssetDatabase.GetAssetPath(c);
            var dirPath = new FileInfo(path).Directory.FullName;
            
            foreach(var file in Directory.GetFiles(dirPath))
            {
                var ext = Path.GetExtension(file);
                if(validExt.Contains(ext))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    c.Add(fileName, AssetDatabase.LoadAssetAtPath<Object>(Prota.Unity.Utils.AssetDatabase.FullPathToAssetPath(file)));
                }
            }
            
        }
        
    }
    
}