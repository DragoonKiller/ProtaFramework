using UnityEngine;
using System;
using Prota.Unity;

namespace Prota.DataBinding
{
    public partial class DataBinding
    {
        [Serializable]
        public class Vector4 : DataBindingBase<UnityEngine.Vector4>
        {
            public override void Deserialize(SerializedData s)
            {
                value = s.Vector4();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(value);
            }
        }
    }
}