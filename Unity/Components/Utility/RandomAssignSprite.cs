using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public class RandomAssignSprite : MonoBehaviour
    {
        public List<Sprite> sprites;
        
        void Start()
        {
            if(this.TryGetComponent<SpriteRenderer>(out var s))
            {
                var sprite = sprites.RandomSelect();
                s.sprite = sprite;
            }
            else if(this.TryGetComponent<ParticleSystem>(out var p))
            {
                var a = p.textureSheetAnimation;
                a.animation = ParticleSystemAnimationType.WholeSheet;
                while(a.spriteCount != 0) a.RemoveSprite(0);
                a.AddSprite(sprites.RandomSelect());
            }
            else
            {
                Debug.LogError($"No Renderer that accept sprite found. [{ this.transform.GetNamePath() }]");
                return;
            }
            
            Destroy(this);
        }
        
    }
    
}
