using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prota;
using Prota.Unity;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ECRigid2D : EComponent
{
    public IReadOnlyList<Collider2D> colliders;
    
    [SerializeField, Readonly] Rigidbody2D _rigidbody2D;
    public Rigidbody2D rd => _rigidbody2D ? _rigidbody2D : _rigidbody2D = GetComponent<Rigidbody2D>();
    
    [SerializeField, Readonly] PhysicsContactRecorder2D _physicsContactRecorder2D;
    public PhysicsContactRecorder2D recorder => _physicsContactRecorder2D ? _physicsContactRecorder2D : _physicsContactRecorder2D = GetComponent<PhysicsContactRecorder2D>();
    
    
    protected override void Awake()
    {
        base.Awake();
        
        var colliders = new List<Collider2D>();
        this.transform.ForeachTransformRecursively(t => {
            if(t.TryGetComponent<Rigidbody2D>(out _) && this.rd != t.GetComponent<Rigidbody2D>()) return false;
            if(t.TryGetComponent<Collider2D>(out var collider))
                colliders.Add(collider);
            return true;
        });
        this.colliders = colliders;
    }
    
    // ====================================================================================================
    // ====================================================================================================
    
    [field: SerializeField, Readonly]
    public bool isSetToTriggerState { get; private set; }
    
    
    public List<Collider2D> notTriggerOriginally;
    
    public void SetCollidersTrigger(bool isTrigger)
    {
        if(isTrigger)
        {
            foreach(var collider in colliders)
            {
                if(collider.isTrigger) continue;
                notTriggerOriginally = notTriggerOriginally ?? new List<Collider2D>();
                notTriggerOriginally.Add(collider);
                collider.isTrigger = true;
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
    
    // ====================================================================================================
    // ====================================================================================================
    
    
    public void SetCollidersEnabled(bool enabled)
    {
        foreach(var collider in colliders)
        {
            collider.gameObject.SetActive(enabled);
        }
    }
}
