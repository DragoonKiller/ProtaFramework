using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using System.Linq;

namespace Prota.CommonResources
{
    
    [CreateAssetMenu(fileName = "ResourcesDatabase", menuName = "", order = 1)]
    [InitializeOnLoad]
    public class ResourcesDatabase : ScriptableObject
    {
        
        [MenuItem("Assets/ProtaFramework/资源/创建数据库", priority = 1)]
        public static void Create()
        {
            var x = ScriptableObject.CreateInstance<ResourcesDatabase>();
            AssetDatabase.CreateAsset(x, Path.Combine(AssetDatabase.GetAssetPath(Selection.activeInstanceID), "ResourceDatabase.asset"));
        }
        
        
        [MenuItem("Assets/ProtaFramework/资源/刷新所有资源", priority = 5)]
        public static void RefreshAll()
        {
            inst.Setup();
            foreach(var i in inst.resources) i.Fill();
        }
        
        static ResourcesDatabase _inst;
        public static ResourcesDatabase inst
        {
            get
            {
                if(_inst == null)
                {
                    var inst = AssetDatabase.FindAssets("t:Prota.CommonResources.ResourcesDatabase");
                    if(inst.Length == 0) throw new Exception("ResourcesDatabase not found");
                    if(inst.Length > 1) Debug.LogError("Multiple ResourcesDatabase object detected.");
                    _inst = AssetDatabase.LoadAssetAtPath<ResourcesDatabase>(AssetDatabase.GUIDToAssetPath(inst[0]));
                }
                return _inst;
            }
        }
        
        
        public List<CommonResource> resources = new List<CommonResource>();
        
        public CommonResource this[string name]
        {
            get
            {
                foreach(var x in resources) if(x.resourcesName == name) return x;
                return null;
            }
        }
        
        public void Setup()
        {
            resources = AssetDatabase.FindAssets("t:CommonResource")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<CommonResource>(x))
                .ToList();
        }
    }
}
