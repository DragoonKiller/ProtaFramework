using UnityEngine;
using System;
using System.Text;
using System.Runtime.Serialization;
using Prota.Unity;

namespace Prota.DataBinding
{
    public partial class DataBinding
    {
        [Serializable]
        public class Color : DataBindingBase<UnityEngine.Color>
        {
            public override void Deserialize(SerializedData s)
            {
                value = s.Color();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(value);
            }
        }
    }
}