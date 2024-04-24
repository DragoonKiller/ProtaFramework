using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public static class SpriteUVCache
    {
        static Dictionary<Sprite, Vector2[]> cache = new();
        
        public static Vector2[] Get(Sprite sprite)
        {
            if(!sprite) throw new Exception("SpriteUVCache.Get: Sprite is null");
            lock(cache)
            {
                if(cache.TryGetValue(sprite, out var uv)) return uv;
                throw new Exception($"SpriteUVCache.Get: Entry of [{ sprite }] not found");
            }
        }
        
        public static Vector2[] GetOrCreate(Sprite sprite)
        {
            if(!sprite) throw new Exception("SpriteUVCache.GetOrCreate: Sprite is null");
            lock(cache)
            {
                if(cache.TryGetValue(sprite, out var uv)) return uv;
                cache.Add(sprite, sprite.uv);
                return sprite.uv;
            }
        }
        
        public static void Prepare(Sprite sprite)
        {
            if(!sprite) return;
            lock(cache)
            {
                if(cache.ContainsKey(sprite)) return;
                cache.Add(sprite, sprite.uv);
            }
        }
        
        
    }
}
