using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Prota.Unity
{
    public class GenerateAfter : MonoBehaviour
    {
        [Serializable]
        public enum GenerateMode
        {
            World,
            LocalToThis,
            LocalToSpecific,
        }
        
        [Serializable]
        public enum GenerateAfterEvent
        {
            Start,
            
            ActiveDestroy,
            
            Manually,
        }
        
        [Serializable]
        public enum SelectMode
        {
            Random,
            
            All,
        }
        
        public GameObject[] targetToGenerate;
        
        public SelectMode selectMode = SelectMode.Random;
        
        public GenerateMode generateMode = GenerateMode.World;
        
        public Transform referenceTransform;
        
        public Vector3 position = Vector2.zero;
        
        public Vector3 localRotation = Vector2.zero;
        
        public GenerateAfterEvent generateAfterEvent = GenerateAfterEvent.Start;
        
        public bool loop;
        
        public float delay = 1f;
        
        public float loopDelay = 1f;
        
        // message 从这里读取刚生成的 GameObject.
        public GameObject g;
        
        public bool generateStarted = false;
        
        void Start()
        {
            if(!generateStarted && generateAfterEvent == GenerateAfterEvent.Start)
            {
                generateStarted = true;
                this.gameObject.NewTimer(delay, Generate);
            }
        }
        
        void OnActiveDestroy()
        {
            if(!generateStarted && generateAfterEvent == GenerateAfterEvent.ActiveDestroy)
            {
                generateStarted = true;
                this.gameObject.NewTimer(delay, Generate);
            }
        }
        
        public void Trigger()
        {
            if(generateStarted) return;
            generateStarted = true;
            this.gameObject.NewTimer(delay, Generate);
        }
        
        void Generate()
        {
            var selection = GetSelection();
            
            foreach(var targetToGenerate in selection)
            {
                switch(generateMode)
                {
                    case GenerateMode.World:
                        Instantiate(targetToGenerate, position, Quaternion.Euler(localRotation));
                        break;
                        
                    case GenerateMode.LocalToThis:
                        g = Instantiate(targetToGenerate, this.transform);
                        g.transform.localPosition = position;
                        g.transform.localRotation = Quaternion.Euler(localRotation);
                        break;
                        
                    case GenerateMode.LocalToSpecific:
                        g = Instantiate(targetToGenerate, referenceTransform);
                        g.transform.localPosition = position;
                        g.transform.localRotation = Quaternion.Euler(localRotation);
                        break;
                        
                    default:
                        throw new ArgumentOutOfRangeException(nameof(generateMode), generateMode, null);
                }
                
                g.SendMessage("OnGenerate", this, SendMessageOptions.DontRequireReceiver);
            }
            
            
            if(loop) this.gameObject.NewTimer(loopDelay, Generate);
        }
        
        IEnumerable<GameObject> GetSelection()
        {
            if(targetToGenerate == null || targetToGenerate.Length == 0) yield break;
            
            if(selectMode == SelectMode.Random)
            {
                yield return targetToGenerate[UnityEngine.Random.Range(0, targetToGenerate.Length)];
            }
            else
            {
                foreach(var g in targetToGenerate)
                {
                    yield return g;
                }
            }
        }
    }
}
