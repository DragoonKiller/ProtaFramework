using UnityEngine;
using System;
using Prota.Unity;

namespace Prota.DataBinding
{
    public partial class DataBinding
    {
        [Serializable]
        public class Vector3 : DataBindingBase<UnityEngine.Vector3>
        {
            public override void Deserialize(SerializedData s)
            {
                value = s.Vector3();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(value);
            }
        }
    }
}