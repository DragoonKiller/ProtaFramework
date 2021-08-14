using UnityEngine;
using System;

using Vec3 = UnityEngine.Vector3;
using Prota.Unity;

namespace Prota.DataBinding
{
    public partial class DataBinding
    {
        [Serializable]
        public class Transform : DataBinding
        {
            public Vec3 position {
                get => source.transform.position;
                set => source.transform.position = value;
            }
            
            public Vec3 localPosition {
                get => source.transform.localPosition;
                set => source.transform.localPosition = value;
            }
            
            public UnityEngine.Quaternion rotation {
                get => source.transform.rotation;
                set => source.transform.rotation = value;
            }
            public Vec3 localScale {
                get => source.transform.localScale;
                set => source.transform.localScale = value;
            }
            
            public Vec3 lossyScale {
                get => source.transform.lossyScale;
            }
            
            public Matrix4x4 local2world {
                get => source.transform.localToWorldMatrix;
            }
            
            
            public Matrix4x4 worldToLocal {
                get => source.transform.worldToLocalMatrix;
            }
            
            public Vec3 TransformPoint(Vec3 r)
                => source.transform.TransformPoint(r);
                
            public Vec3 TransformVector(Vec3 r)
                => source.transform.TransformVector(r);
                
            public Vec3 TransformDirection(Vec3 r)
                => source.transform.TransformDirection(r);
                

            public Vec3 InverseTransformPoint(Vec3 r)
                => source.transform.InverseTransformPoint(r);
                
            public Vec3 InverseTransformVector(Vec3 r)
                => source.transform.InverseTransformVector(r);
                
            public Vec3 InverseTransformDirection(Vec3 r)
                => source.transform.InverseTransformDirection(r);

            public override void Deserialize(SerializedData s) { }

            public override void Serialize(SerializedData s) { }
        }
    }
}