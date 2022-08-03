using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
using System.Collections;

namespace Prota.Tweening
{
    public static class TweeningTests
    {
        public static void TestA()
        {
            var a = GameObject.Find("Test1");
            a.transform.TweenMoveX(4, 2);
            ProtaTweeningManager.Get().StartCoroutine(R(a));
        }
        
        static IEnumerator R(GameObject a)
        {
            yield return new WaitForSeconds(1);
            a.transform.TweenMoveX(-2, 1);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Debug.Break();
            yield break;
        }
        
    }
    
}
