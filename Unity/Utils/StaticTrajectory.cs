using UnityEngine;
using UnityEditor;
using System;


namespace Prota.Unity
{
    
    // 静态轨迹, 使用插值的方式来控制物体移动.
    // 每一种轨迹都有不同的插值参数.
    [Serializable]
    public abstract class StaticTrajectory
    {
        [SerializeField, Readonly(hideWhenEditing = true)] public Vector3 from;
        [SerializeField, Readonly(hideWhenEditing = true)] public Vector3 to;
        
        public Vector3 delta => from.To(to);
        
        public abstract Vector3 Evaluate(float ratio);
        public virtual Vector3 Evaluate(float time, float duration) => Evaluate(time / duration);
        
        public abstract void DebugTrajectory();
        
        public class Linear : StaticTrajectory
        {
            public override void DebugTrajectory()
            {
                Debug.DrawLine(from, to, Color.red);
            }

            public override Vector3 Evaluate(float ratio)
            {
                return Vector3.Lerp(from, to, ratio);
            }
        }
        
        public class Parabolic : StaticTrajectory
        {
            public Vector3 gravity;
            public float duration;

            public Vector3 up => -gravity.normalized;
            public Vector3 verticalDistance => delta.Project(gravity);
            public Vector3 horizontalDistance => delta - verticalDistance;
            public Vector3 horizontalVelocity => horizontalDistance / duration;
            public Vector3 initialVelocity => horizontalVelocity + verticalDistance / duration + 0.5f * gravity * duration;
            
            public Parabolic(Vector3 gravity, float duration)
            {
                this.gravity = gravity;
                this.duration = duration;
            }
            
            public override Vector3 Evaluate(float ratio)
            {
                (duration != 0).Assert(nameof(duration) + " must not be zero.");
                (gravity.magnitude != 0).Assert(nameof(gravity) + " must not be zero.");
                var t = ratio * duration;
                return from + initialVelocity * t + 0.5f * gravity * t * t;
            }
            
            public override void DebugTrajectory()
            {
                const int count = 25;
                for(int i = 0; i < count; i++)
                {
                    var t1 = i / (float)count;
                    var t2 = (i + 1) / (float)count;
                    var p1 = Evaluate(t1);
                    var p2 = Evaluate(t2);
                    Debug.DrawLine(p1, p2, Color.red);
                }
            }
            
        }
        
        public StaticTrajectory Clone() => MemberwiseClone() as StaticTrajectory;
    }
    
    
}
