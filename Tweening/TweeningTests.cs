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
            var tt = a.transform.TweenMoveX(4, 2);
            tt.onInterrupted = t => {
                Debug.Log("interrupt!!");
            };
            tt.onRemove = t => {
                Debug.Log("remove 1 !!!");
            };
            ProtaTweeningManager.Get().StartCoroutine(R(a));
        }
        
        static IEnumerator R(GameObject a)
        {
            yield return new WaitForSeconds(1);
            var tt = a.transform.TweenMoveX(-2, 1);
            tt.onFinish = t => {
                Debug.Log("finish!!!");
            };
            tt.onRemove = t => {
                Debug.Log("remove 2 !!!");
            };
            
            // yield return new WaitForEndOfFrame();
            // yield return new WaitForEndOfFrame();
            // Debug.Break();
            
            yield break;
        }
        
    }
    
}
