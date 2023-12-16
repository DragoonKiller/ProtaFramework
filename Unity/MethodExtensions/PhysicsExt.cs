using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        // ============================================================================================================
        // Consts
        // ============================================================================================================
        
        const float collisionSizeReduction = 1f / 256;
        
        static RaycastHit2D[] rayCastBuffer = new RaycastHit2D[128];
        
        static readonly LayerMask maskOfAll = (LayerMask)(-1);
        
        // ============================================================================================================
        // MoveAndCollide
        // ============================================================================================================
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity)
        {
            return MoveAndCollide(c, velocity, Time.fixedDeltaTime, maskOfAll);
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, LayerMask layer)
        {
            return MoveAndCollide(c, velocity, Time.fixedDeltaTime, layer);
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, float deltaTime)
        {
            return MoveAndCollide(c, velocity, deltaTime, maskOfAll);
        }
        
        public static Vector2 MoveAndCollide(this Collider2D c, Vector2 velocity, float deltaTime, LayerMask layer)
        {
            // 速度太慢, 当做没移动.
            if(velocity.sqrMagnitude < 1e-8f) return Vector2.zero;
            
            var move = velocity * deltaTime;
            move = move.WithLength(move.magnitude);
            
            switch(c)
            {
                default:
                case BoxCollider2D box:
                {
                    var n = Physics2D.BoxCast(c.bounds.center, c.bounds.size, c.transform.rotation.z, move, new ContactFilter2D(), rayCastBuffer, move.magnitude);
                    
                    #if UNITY_EDITOR
                    var color = Color.yellow;
                    var min = c.bounds.min + (Vector3)move;
                    var max = c.bounds.max + (Vector3)move;
                    Debug.DrawLine(min, min.WithX(max.x), color);
                    Debug.DrawLine(min, min.WithY(max.y), color);
                    Debug.DrawLine(min.WithX(max.x), max, color);
                    Debug.DrawLine(min.WithY(max.y), max, color);
                    #endif
                    
                    move = ClosetCollide(n, c, move);
                }
                break;
                
                case CircleCollider2D circ:
                {
                    var n = Physics2D.CircleCast(circ.bounds.center, circ.radius, move, new ContactFilter2D(), rayCastBuffer, move.magnitude);
                    move = ClosetCollide(n, c, move);
                }
                break;
                
                case CapsuleCollider2D cap:
                {
                    var n = Physics2D.CapsuleCast(cap.bounds.center, cap.size, cap.direction, c.transform.rotation.z, move, new ContactFilter2D(), rayCastBuffer, move.magnitude);
                    move = ClosetCollide(n, c, move);
                }
                break;
            }
            
            move = move.WithLength(move.magnitude);
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
                // 自己不算.
                if(rayCastBuffer[i].collider == c) continue;
                // 法线和移动方向相同, 不属于碰撞.
                if(Vector2.Dot(rayCastBuffer[i].normal, move) >= 0) continue;
                
                return move.WithLength(rayCastBuffer[i].distance);
            }
            
            // 啥都没碰到.
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
            return rd.MoveAndCollide(velocity, deltaTime, maskOfAll);
        }
        
        
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity, LayerMask layer)
        {
            return rd.MoveAndCollide(velocity, Time.fixedDeltaTime, layer);
        }
        
        
        public static Vector2 MoveAndCollide(this Rigidbody2D rd, Vector2 velocity)
        {
            return rd.MoveAndCollide(velocity, Time.fixedDeltaTime, maskOfAll);
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
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static Rigidbody2D SetVx(this Rigidbody2D rd, float x)
        {
            rd.velocity = rd.velocity.WithX(x);
            return rd;
        }
        
        public static Rigidbody2D SetVy(this Rigidbody2D rd, float y)
        {
            rd.velocity = rd.velocity.WithY(y);
            return rd;
        }
        
        public static Rigidbody2D GetVx(this Rigidbody2D rd, out float x)
        {
            x = rd.velocity.x;
            return rd;
        }
        
        public static Rigidbody2D GetVy(this Rigidbody2D rd, out float y)
        {
            y = rd.velocity.y;
            return rd;
        }
    }
}
