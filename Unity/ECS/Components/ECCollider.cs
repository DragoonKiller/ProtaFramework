using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prota;
using Prota.Unity;
using UnityEngine;

public class ECCollider : EComponent
{
    [SerializeField, Readonly] Collider2D _collider;
    public Collider2D cc => _collider ? _collider : _collider = GetComponent<Collider2D>();
    
    [SerializeField, Readonly] Rigidbody2D _rigidbody;
    public Rigidbody2D rd => _rigidbody ? _rigidbody : _rigidbody = GetComponent<Rigidbody2D>();
}
