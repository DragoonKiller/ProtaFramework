using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

namespace Prota.CommonResources
{
    
    [InitializeOnLoad]
    public class CommonResource : ScriptableObject
    {
        [MenuItem("Assets/ProtaFramework/资源/创建资源文件夹", priority = 2)]
        public static void Create()
        {
            var x = ScriptableObject.CreateInstance<CommonResource>();
            AssetDatabase.CreateAsset(x, Path.Combine(AssetDatabase.GetAssetPath(Selection.activeInstanceID), "CommonResource.asset"));
        }
        
        
        public string resourcesName;
        
        [Serializable]
        public class Record
        {
            public string name;
            public UnityEngine.Object target;
        }
    
        public List<Record> records = new List<Record>();
        
        public void Clear()
        {
            records.Clear();
        }
        
        public UnityEngine.Object this[string name]
        {
            get
            {
                foreach(var i in records)
                {
                    if(name == i.name) return i.target;
                }
                return null;
            }
        }
        
#if UNITY_EDITOR
        public void Fill()
        {
            Clear();
            
            var path = AssetDatabase.GetAssetPath(this);
            var dirPath = new FileInfo(path).Directory.FullName;
            
            this.resourcesName = Path.GetFileNameWithoutExtension(new FileInfo(path).Directory.FullName);
            
            foreach(var file in Directory.GetFiles(dirPath))
            {
                var ext = Path.GetExtension(file);
                var fileName = Path.GetFileNameWithoutExtension(file);
                var assetObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Prota.Unity.Utils.AssetDatabase.FullPathToAssetPath(file));
                if(assetObject != null && assetObject != this) this.records.Add(new CommonResource.Record{ name = fileName, target = assetObject });
            }
        }
#endif
    }
}
