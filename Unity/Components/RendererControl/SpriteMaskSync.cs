using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota.Unity
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteMask))]
    [ExecuteAlways]
    public class SpriteMaskSync : MonoBehaviour
    {
        SpriteRenderer rd => this.GetComponent<SpriteRenderer>();
        SpriteMask mask => this.GetComponent<SpriteMask>();
        
        void Update()
        {
            mask.sprite = rd.sprite;
        }
        
        void LateUpdate()
        {
            mask.sprite = rd.sprite;
        }
    }
}
