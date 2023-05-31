using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prota;
using Prota.Unity;
using UnityEditor.Experimental.GraphView;
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
    
    [SerializeField, Readonly] PhysicsContactRecorder _physicsContactRecorder;
    public PhysicsContactRecorder rdxRecorder => _physicsContactRecorder ? _physicsContactRecorder : _physicsContactRecorder = GetComponent<PhysicsContactRecorder>();
    
    public List<Collider2D> notTriggerOriginally;
    
    public bool isSetToTriggerState { get; private set; }
    
    public void SetCollidersTrigger(bool isTrigger)
    {
        if(isTrigger)
        {
            foreach(var collider in colliders)
            {
                if(collider.cc.isTrigger) continue;
                notTriggerOriginally = notTriggerOriginally ?? new List<Collider2D>();
                notTriggerOriginally.Add(collider.cc);
                collider.cc.isTrigger = true;
            }
            isSetToTriggerState = true;
        }
        else
        {
            if(notTriggerOriginally != null)
                foreach(var collider in notTriggerOriginally)
                    collider.isTrigger = false;
            notTriggerOriginally = null;
            isSetToTriggerState = false;
        }
    }
    
    public void SetCollidersEnabled(bool enabled)
    {
        foreach(var collider in colliders)
        {
            collider.gameObject.SetActive(enabled);
        }
    }
}
