using UnityEngine;
using System;

using Prota.Unity;

namespace Prota.Data
{
    public partial class DataBinding
    {
        [Serializable]
        public class Sprite : DataBinding
        {
            public SpriteRenderer target => source.GetComponent<SpriteRenderer>();
            
            public UnityEngine.Sprite sprite
            {
                get => target ? target.sprite : null;
                set 
                {
                    if(target == null) return;
                    target.sprite = value;
                }
            }

            public override void Deserialize(SerializedData s) { }
            public override void Serialize(SerializedData s) { }
        }
    }
}