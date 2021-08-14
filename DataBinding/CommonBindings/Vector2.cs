using UnityEngine;
using System;
using Prota.Unity;

namespace Prota.Data
{
    public partial class DataBinding
    {
        [Serializable]
        public class Vector2 : DataBindingBase<UnityEngine.Vector2>
        {
            public override void Deserialize(SerializedData s)
            {
                value = s.Vector2();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(value);
            }
        }
    }
}