using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

namespace Prota.CommonResources
{
    
    [CreateAssetMenu(fileName = "CommonResources", menuName = "ProtaFramework/CommonResources/Create", order = 1)]
    [InitializeOnLoad]
    public class CommonResource : ScriptableObject, IEnumerable<KeyValuePair<string, UnityEngine.Object>>
    {
        [Serializable]
        public class Record
        {
            public string name;
            public UnityEngine.Object target;
        }
    
        static CommonResource _inst;
        public static CommonResource inst
        {
            get
            {
                if(_inst == null)
                {
                    var inst = AssetDatabase.FindAssets("t:Prota.CommonResources.CommonResource");
                    if(inst.Length == 0) throw new Exception("CommonResource not found");
                    if(inst.Length > 1) Debug.LogError("Multiple CommonResource object detected.");
                    _inst = AssetDatabase.LoadAssetAtPath<CommonResource>(AssetDatabase.GUIDToAssetPath(inst[0]));
                }
                return _inst;
            }
        }
        
        Dictionary<string, UnityEngine.Object> resources = new Dictionary<string, UnityEngine.Object>();
        
        public List<Record> records = new List<Record>();
        
        void InitResources()
        {
            if(resources != null) return;
            resources = new Dictionary<string, UnityEngine.Object>();
            for(int i = 0; i < records.Count; i++) resources[records[i].name] = records[i].target;
        }
        
        public void Add(string name, UnityEngine.Object target)
        {
            if(resources == null) InitResources();
            resources.Add(name, target);
            records.Add(new Record() { name = name, target = target });
        }
        
        public void Clear()
        {
            records.Clear();
            resources.Clear();
        }

        public IEnumerator<KeyValuePair<string, UnityEngine.Object>> GetEnumerator() => resources.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        public UnityEngine.Object this[string name]
        {
            get
            {
                if(resources == null) InitResources();
                return resources[name];
            }
        }
    }
}
