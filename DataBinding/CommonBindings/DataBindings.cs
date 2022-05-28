using UnityEngine;
using UnityEditor;

namespace Prota.Data
{

    public class DataBindings : DataBlock
    {
        public new Rigidbody rigidbody => this.GetComponent<Rigidbody>();
        public new Rigidbody2D rigidbody2D => this.GetComponent<Rigidbody2D>();
        public MeshFilter mesh => this.GetComponent<MeshFilter>();
        public MeshRenderer meshRenderer => this.GetComponent<MeshRenderer>();
        public new Collider collider => this.GetComponent<Collider>();
        public MeshCollider meshCollider => this.GetComponent<MeshCollider>();
        public BoxCollider boxCollider => this.GetComponent<BoxCollider>();
        public CapsuleCollider capsuleCollider => this.GetComponent<CapsuleCollider>();
        public SphereCollider sphereCollider => this.GetComponent<SphereCollider>();
        
    }
    
}