using System;
using Prota.Unity;
using UnityEngine;
using Prota.Animation;

namespace Prota.Data
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Animation : DataBlock
    {
        public SpriteRenderer spriteRenderer => this.GetComponent<SpriteRenderer>();
        
        public Sprite sprite
        {
            get => spriteRenderer.sprite;
            set => spriteRenderer.sprite = value;
        }
    }
}