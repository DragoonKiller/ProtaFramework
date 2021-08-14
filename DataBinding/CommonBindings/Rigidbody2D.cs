using UnityEngine;
using System;
using Prota.Unity;

namespace Prota.Data
{
    public partial class DataBinding
    {
        [Serializable]
        public class Rigidbody2D : DataBinding
        {
            public UnityEngine.Rigidbody2D target => source.GetComponent<UnityEngine.Rigidbody2D>();
            
            public UnityEngine.Vector3 velocity
            {
                get => target.velocity;
                set => target.velocity = value;
            }
            
            public override void Deserialize(SerializedData s) { }
            public override void Serialize(SerializedData s) { }
        }
    }
}