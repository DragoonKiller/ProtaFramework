using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    public delegate void ValueTweeningUpdate(TweenHandle h, float t);
    
    public enum TweenType
    {
        Custom = -1,  // does not counted into duplicate.
        
        MoveX = 1,
        MoveY,
        MoveZ,
        ScaleX,
        ScaleY,
        ScaleZ,
        RotateX,
        RotateY,
        RotateZ,
        ColorR,
        ColorG,
        ColorB,
        Transparency,
        
    }
    
    
}
