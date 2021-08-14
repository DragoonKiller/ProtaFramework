using System;
using Prota.Unity;
using UnityEngine;
using Prota.Animation;

namespace Prota.Data
{
    public partial class DataBinding
    {
        public class Animation : DataBinding
        {
            public float time;
            
            public ProtaAnimation anim => source.GetComponent<ProtaAnimation>();
            
            public override void Deserialize(SerializedData s)
            {
                time = s.Float();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(time);
                
            }
        }
    }
}