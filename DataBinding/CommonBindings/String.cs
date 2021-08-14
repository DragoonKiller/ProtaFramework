using UnityEngine;
using System;
using Prota.Unity;

namespace Prota.DataBinding
{
    public partial class DataBinding
    {
        [Serializable]
        public class String : DataBindingBase<string>
        {
            public String() => value = "";
            
            public override void Deserialize(SerializedData s)
            {
                value = s.String();
            }

            public override void Serialize(SerializedData s)
            {
                s.Push(value);
            }
        }
    }
}