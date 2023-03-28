using UnityEngine;
using UnityEditor;
using System;


namespace Prota.Unity
{
    

    [Serializable]
    public abstract class StaticTrajectory
    {
        [Serializable]
        public abstract class MoveType
        {
            public class Duration : MoveType
            {
                public float duration;
            }
            
            public class Speed : MoveType
            {
                public float speed;
            }
            
        }
        
        [SerializeField, Readonly(hideWhenEditing = true)] public Vector3 startPoint;
        [SerializeField, Readonly(hideWhenEditing = true)] public Vector3 targetPoint;
        [SerializeField, Readonly(hideWhenEditing = true)] public float ratio;
        
        public Vector3 delta => startPoint.To(targetPoint);
        public Vector2 delta2 => delta.XZToXY();
        
        [SerializeReference, ReferenceEditor] public MoveType moveType;
        
        public virtual void Reset() => ratio = 0;
        public abstract void Move(GameObject g);
        public abstract void DebugTrajectory();
        
        public class Linear : StaticTrajectory
        {
            public override void Move(GameObject g)
            {
                var duration = moveType switch {
                    MoveType.Duration d => d.duration,
                    MoveType.Speed s => delta.magnitude / s.speed,
                    _ => throw new NotSupportedException(moveType.ToString())
                };
                
                ratio = (ratio + Time.deltaTime / duration);
                    
                if(moveType is MoveType.Duration)
                {
                    g.transform.position = (startPoint, targetPoint).Lerp(ratio);
                }
                else if(moveType is MoveType.Speed s)
                {
                    g.transform.position = startPoint + startPoint.To(targetPoint).normalized * s.speed * ratio;
                }
                else throw new NotSupportedException(moveType.ToString());
            }

            public override void DebugTrajectory()
            {
                if(moveType is MoveType.Duration) Debug.DrawLine(startPoint, targetPoint, Color.red);
                else Debug.DrawLine(startPoint, startPoint + startPoint.To(targetPoint).normalized * 10, Color.red);
            }
        }
        
        public class Parabolic : StaticTrajectory
        {
            // 最大竖直速度, 竖直速度不能超过这个数.
            public float maxVerticalSpeed = 10000;
            
            float duration => moveType switch {
                MoveType.Duration d => d.duration,
                MoveType.Speed s => delta2.magnitude / s.speed,
                _ => throw new NotSupportedException(moveType.ToString())
            };
            
            Vector2 horizontalVelocity => moveType switch {
                MoveType.Duration d => delta2 / d.duration,
                MoveType.Speed s => delta2.normalized * s.speed,
                _ => throw new NotSupportedException(moveType.ToString())
            };
            
            float deltaH => delta.y;
            float verticalSpeed => ((deltaH - 0.5f * Physics.gravity.y * duration * duration) / duration).Min(maxVerticalSpeed);
            
            public override void Move(GameObject g)
            {
                ratio += Time.deltaTime / duration;
                var t = ratio * duration;
                var move = horizontalVelocity.XYToXZ(verticalSpeed) * t + 0.5f * Physics.gravity * t * t;
                g.transform.position = startPoint + move;
            }
            
            public override void DebugTrajectory()
            {
                const int count = 25;
                for(int i = 0; i < count; i++)
                {
                    var t1 = i / (float)count * duration;
                    var t2 = (i + 1) / (float)count * duration;
                    var p1 = startPoint + horizontalVelocity.XYToXZ(verticalSpeed) * t1 + 0.5f * Physics.gravity * t1 * t1;
                    var p2 = startPoint + horizontalVelocity.XYToXZ(verticalSpeed) * t2 + 0.5f * Physics.gravity * t2 * t2;
                    Debug.DrawLine(p1, p2, Color.red);
                }
            }
        }
        
        public StaticTrajectory Clone() => MemberwiseClone() as StaticTrajectory;
    }
    
    
}
