using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prota;
using Prota.Unity;
using UnityEngine;

public class ECRigid : EComponent
{
    public IEnumerable<ECCollider> colliders => this.ComponentsAside<ECCollider>();
    
    [SerializeField, Readonly] Rigidbody2D _rigidbody2D;
    public Rigidbody2D rd => _rigidbody2D ? _rigidbody2D : _rigidbody2D = GetComponent<Rigidbody2D>();
    
    [SerializeField, Readonly] Rigidbody _rigidbody;
    public Rigidbody rdx => _rigidbody ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();
    
    [SerializeField, Readonly] PhysicsContactRecorder2D _physicsContactRecorder2D;
    public PhysicsContactRecorder2D recorder => _physicsContactRecorder2D ? _physicsContactRecorder2D : _physicsContactRecorder2D = GetComponent<PhysicsContactRecorder2D>();
    
    
    public void SetCollidersEnabled(bool enabled)
    {
        foreach(var collider in colliders)
        {
            collider.gameObject.SetActive(enabled);
        }
    }
}
