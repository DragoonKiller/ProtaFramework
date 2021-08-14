using UnityEngine;
using System;
using Prota.Unity;

namespace Prota.Data
{
    public partial class DataBinding
    {
        [Serializable]
        public class Int : DataBindingBase<int>
        {
            public override void Deserialize(SerializedData s)
            {
                value = s.Int();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(value);
            }
        }
    }
}