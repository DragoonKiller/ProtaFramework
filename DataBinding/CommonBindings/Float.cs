using UnityEngine;
using System;
using Prota.Unity;

namespace Prota.DataBinding
{
    public partial class DataBinding
    {
        [Serializable]
        public class Float : DataBindingBase<float>
        {
            public override void Deserialize(SerializedData s)
            {
                value = s.Float();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(value);
            }
        }
    }
}