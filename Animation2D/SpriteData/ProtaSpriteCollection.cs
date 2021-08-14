using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Animation
{
    [Serializable]
    public class ProtaSpriteCollection : ScriptableObject
    {
        [Serializable]
        public struct ProtaSpriteInfo
        {
            [SerializeField]
            string _id;
            
            [SerializeField]
            Sprite _sprite;
            
            [SerializeField]
            public string id => _id;
            
            [SerializeField]
            public Sprite sprite => _sprite;
            
            public ProtaSpriteInfo(string id, Sprite sprite) => (this._id, this._sprite) = (id, sprite);
        }
        
        [SerializeField]
        public new string name;
        
        [SerializeField]
        public List<ProtaSpriteInfo> sprites = new List<ProtaSpriteInfo>();
        
        public Dictionary<string, Sprite> cache;
        
        public Sprite this[string name]
        {
            get
            {
                // 在编辑器里取图片是实时的.
                if(cache == null || !Application.isPlaying)
                {
                    cache = new Dictionary<string, Sprite>();
                    foreach(var i in sprites) cache.Add(i.id, i.sprite);
                }
                return cache[name];
            }
        }
        
        
        public void Add(string id, Sprite sprite)
        {
            if(!EditorCheck()) return;
            sprites.Add(new ProtaSpriteInfo(id, sprite));
        }
        
        public void Remove(string id)
        {
            if(!EditorCheck()) return;
            sprites.RemoveAll(x => x.id == id);
        }
        
        public void Remove(Sprite sprite)
        {
            if(!EditorCheck()) return;
            sprites.RemoveAll(x => x.sprite == sprite);
        }
        
        
        public bool EditorCheck()
        {
            if(Application.isPlaying)
            {
                Debug.LogError("不允许在运行时变更 SpriteCollection");
                return false;
            }
            return true;
        }
        
    }
}