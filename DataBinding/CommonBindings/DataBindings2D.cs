using UnityEngine;
using UnityEditor;

namespace Prota.Data
{

    public class DataBindings2D : DataBlock
    {
        public new Rigidbody2D rigidbody => this.GetComponent<Rigidbody2D>();
        public SpriteRenderer spriteRenderer => this.GetComponent<SpriteRenderer>();
        public Sprite sprite
        {
            get => spriteRenderer.sprite;
            set => spriteRenderer.sprite = value;
        }
        public new Collider2D collider => this.GetComponent<Collider2D>();
        public BoxCollider2D boxCollider => this.GetComponent<BoxCollider2D>();
        public CapsuleCollider2D capsuleCollider => this.GetComponent<CapsuleCollider2D>();
        public CircleCollider2D circleCollider => this.GetComponent<CircleCollider2D>();
        
    }
    
}