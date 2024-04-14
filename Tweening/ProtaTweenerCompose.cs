using UnityEngine;
using Prota.Unity;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;

namespace Prota.Tween
{
    [ExecuteAlways]
    public class ProtaTweenerCompose : MonoBehaviour
    {
        [Serializable]
        public class AnimEntry
        {
            public string name;
            
            // 是否控制 tweener 的进度.
            public bool control;
            
            [Range(0, 1)] public float progress = 0;
            
            // 是否自动播放.
            public bool running = false;
            
            // 播放到尽头后, 是停止还是循环.
            public bool loop = false;
            
            // 指示当前的更新是从 0 到 1 还是反过来.
            public bool reversed = false;
            
            // 循环一次后反转 from/to. false 即一直重复从0到1, true即从0到1再到0再到1循环.
            public bool reverseOnLoop = false;
            
            #if UNITY_EDITOR
            public string matchName;    // regex, 匹配 ProtaTweener.name.
            #endif
            
            public ProtaTweener[] tweeners = Array.Empty<ProtaTweener>();
            
        }
        
        public AnimEntry[] anims = Array.Empty<AnimEntry>();
        
        
        void Update()
        {
            Step(Time.deltaTime);
            UpdateProgress();
        }
        
        public void Step(float dt)
        {
            foreach(var a in anims)
            {
                if(!a.running) continue;
                
                var delta = a.reversed ? -dt : dt;
                a.progress += delta;
                
                if(a.progress >= 1)
                {
                    if(a.loop)
                    {
                        if(a.reverseOnLoop)
                        {
                            a.progress = 2 - a.progress;
                            a.reversed = !a.reversed;
                        }
                        else
                        {
                            a.progress -= 1;
                        }
                    }
                    else
                    {
                        a.progress = 1;
                        a.running = false;
                    }
                }
                
                if(a.progress <= 0)
                {
                    if(a.loop)
                    {
                        if(a.reverseOnLoop)
                        {
                            a.progress = -a.progress;
                            a.reversed = !a.reversed;
                        }
                        else
                        {
                            a.progress += 1;
                        }
                    }
                    else
                    {
                        a.progress = 0;
                        a.running = false;
                    }
                }
            }
        }
        
        public void UpdateProgress()
        {
            foreach(var a in anims)
            {
                if(!a.control) continue;
                foreach(var t in a.tweeners)
                {
                    t.play = true;
                    t.running = false;
                    t.progress = a.progress;
                }
            }
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public void Play()
        {
            foreach(var a in anims)
            {
                a.running = true;
            }
        }
        
        public void Reset()
        {
            foreach(var a in anims)
            {
                a.running = false;
                a.progress = 0;
            }
        }
        
        public void Stop()
        {
            foreach(var a in anims)
            {
                a.running = false;
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        
        
        #if UNITY_EDITOR
        static readonly Dictionary<Regex, AnimEntry> matchDict = new();
        static readonly HashMapList<AnimEntry, ProtaTweener> resultDict = new();
        public void AutoMatch()
        {
            matchDict.Clear();
            foreach(var anim in this.anims)
            {
                matchDict.Add(new Regex(anim.matchName), anim);
            }
            
            resultDict.Clear();
            foreach(var tweener in this.GetComponentsInChildren<ProtaTweener>())
            {
                foreach(var regex in matchDict.Keys)
                {
                    if(regex.IsMatch(tweener.animName))
                    {
                        Debug.LogError($"match [{ regex.ToString() }] => [{ tweener.animName }]");
                        resultDict.AddElement(matchDict[regex], tweener);
                        break;
                    }
                }
            }
            
            foreach(var a in resultDict)
            {
                a.Key.tweeners = a.Value.ToArray();
            }
        }
        
        #endif
        
        
        
        
    }
}
