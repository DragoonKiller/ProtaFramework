using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    public static class TweeningTests
    {
        public static void TestA()
        {
            var a = GameObject.Find("Test1");
            a.transform.TweenMove(new Vector3(2, 2, 0), 1);
            
        }
        
        
    }
    
}
