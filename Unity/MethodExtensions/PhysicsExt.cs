using System;
using System.Collections.Generic;
using UnityEngine;


namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        const float collisionSizeReduction = 1f / 256;
        
        static RaycastHit2D[] rayCastBuffer = new RaycastHit2D[128];
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity)
        {
            return MoveAndCollide(c, velocity, Time.fixedDeltaTime, (LayerMask)(-1));
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, LayerMask layer)
        {
            return MoveAndCollide(c, velocity, Time.fixedDeltaTime, layer);
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, float deltaTime)
        {
            return MoveAndCollide(c, velocity, deltaTime, (LayerMask)(-1));
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, float deltaTime, LayerMask layer)
        {
            var move = velocity * deltaTime;
            move = move.Len(move.magnitude + collisionSizeReduction);
            
            switch(c)
            {
                default:
                case BoxCollider2D box:
                {
                    var n = Physics2D.BoxCastNonAlloc(c.bounds.center, c.bounds.size, c.transform.rotation.z, move, rayCastBuffer, move.magnitude + 1e-4f);
                    move = ClosetCollide(n, c, move);
                }
                break;
                
                case CircleCollider2D circ:
                {
                    var n = Physics2D.CircleCastNonAlloc(circ.bounds.center, circ.radius, move, rayCastBuffer, move.magnitude + 1e-4f);
                    move = ClosetCollide(n, c, move);
                }
                break;
                
                case CapsuleCollider2D cap:
                {
                    var n = Physics2D.CapsuleCastNonAlloc(cap.bounds.center, cap.size, cap.direction, c.transform.rotation.z, move, rayCastBuffer, move.magnitude + 1e-4f);
                    move = ClosetCollide(n, c, move);
                }
                break;
            }
            
            move = move.Len(move.magnitude - collisionSizeReduction);
            return move;
        }

        class RaycastHit2DDistanceComparer : IComparer<RaycastHit2D>
        {
            public int Compare(RaycastHit2D x, RaycastHit2D y)
            {
                return x.distance.CompareTo(y.distance);
            }
        }
        
        static readonly RaycastHit2DDistanceComparer raycastHit2DDistanceComparer = new RaycastHit2DDistanceComparer();

        static Vector2 ClosetCollide(int n, Collider2D c, Vector2 move)
        {
            if(n == 0) return move;
            
            Array.Sort(rayCastBuffer, 0, n, raycastHit2DDistanceComparer);
            for(int i = 0; i < n; i++)
            {
                if(rayCastBuffer[i].collider != c) return move.Len(rayCastBuffer[i].distance);
            }
            
            return move;
        }
        
        
        
        static Collider2D[] collidersCache = new Collider2D[64];
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity, float deltaTime, LayerMask layer)
        {
            var n = rd.GetAttachedColliders(collidersCache);
            var res = velocity * deltaTime;
            for(int i = 0; i < n; i++)
            {
                var r = collidersCache[i].MoveAndCollide(velocity, deltaTime, layer);
                if(r.sqrMagnitude < res.sqrMagnitude) res = r; 
            }
            return res;
        }
        
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity, float deltaTime)
        {
            return rd.MoveAndCollide(velocity, deltaTime, (LayerMask)(-1));
        }
        
        
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity, LayerMask layer)
        {
            return rd.MoveAndCollide(velocity, Time.fixedDeltaTime, layer);
        }
        
        
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity)
        {
            return rd.MoveAndCollide(velocity, Time.fixedDeltaTime, (LayerMask)(-1));
        }
        
        public static Rigidbody2D MoveRelative(this Rigidbody2D rd, Vector2 move)
        {
            rd.MovePosition(rd.position + move);
            return rd;
        }
        
        public static Rigidbody2D RotateRelative(this Rigidbody2D rd, float move)
        {
            rd.MoveRotation(rd.rotation + move);
            return rd;
        }
    }
}