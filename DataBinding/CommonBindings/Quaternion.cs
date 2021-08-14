using UnityEngine;
using System;
using System.Text;
using System.Runtime.Serialization;
using Prota.Unity;

namespace Prota.Data
{
    public partial class DataBinding
    {
        [Serializable]
        public class Quaternion : DataBindingBase<UnityEngine.Quaternion>
        {
            public override void Deserialize(SerializedData s)
            {
                value = s.Quaternion();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(value);
            }
        }
    }
}