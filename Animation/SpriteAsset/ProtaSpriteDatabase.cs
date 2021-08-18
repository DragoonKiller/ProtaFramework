using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Animation
{
    /// <summary>
    /// 存储了当前项目所有 Sprite 资源的 scriptable object.
    /// </summary>
    [Serializable]
    public class ProtaSpriteDatabase : ScriptableObject
    {
        public const string resPath = "ProtaFramework/SpriteDatabase";
        
        static ProtaSpriteDatabase _instance;
        public static ProtaSpriteDatabase instance
        {
            get
            {
                if(_instance == null) _instance = Resources.Load<ProtaSpriteDatabase>(resPath);
                return _instance;
            }
        }
        
        
        [Serializable]
        public struct SpriteCollectionRecord
        {
            [SerializeField]
            public string name;
            
            [SerializeReference]
            public ProtaSpriteCollection collection;
        }
        
        [SerializeField]
        public List<SpriteCollectionRecord> records = new List<SpriteCollectionRecord>();
        
        
        public Dictionary<string, ProtaSpriteCollection> cache;
        
        public ProtaSpriteCollection this[string name]
        {
            get
            {
                // 在编辑器里取图片是实时的.
                if(cache == null || !Application.isPlaying)
                {
                    cache = new Dictionary<string, ProtaSpriteCollection>();
                    foreach(var i in records) cache.Add(i.name, i.collection);
                }
                return cache[name];
            }
        }
        
        public void Add(string name, ProtaSpriteCollection collection)
        {
            if(!EditorCheck()) return;
            records.Add(new SpriteCollectionRecord(){
                name = name,
                collection = collection,
            });
        }
        
        public void Remove(string name)
        {
            if(!EditorCheck()) return;
            records.RemoveAll(x => x.name == name);
        }
        
        public void Remove(ProtaSpriteCollection collection)
        {
            if(!EditorCheck()) return;
            records.RemoveAll(x => x.collection == collection);
        }
        
        public void Clear()
        {
            records.Clear();
            cache = null;
        }
        
        public bool EditorCheck()
        {
            if(Application.isPlaying)
            {
                Debug.LogError("不允许在运行时变更 SpriteDatabase");
                return false;
            }
            return true;
        }
        
    }
}