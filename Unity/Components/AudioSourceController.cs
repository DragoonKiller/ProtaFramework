using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Audio;

namespace Prota.Unity
{
    public class AudioSourceController : MonoBehaviour
    {
        static readonly Dictionary<GameObject, AudioSourceController> cache
            = new Dictionary<GameObject, AudioSourceController>();
        
        public static Func<string, AudioClip> getAudioResourceOverride;
        
        public static Func<string, AudioClip> getAudioResource
            = name => {
                if(getAudioResourceOverride != null)
                    return getAudioResourceOverride(name);
                else
                    return ProtaRes.Load<AudioClip>("audio", name);
            };
            
        public static AudioMixer defaultMixer;
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        [Serializable]
        public class Entry
        {
            [field:SerializeField, Readonly] public string name { get; private set; }
            [field:SerializeField, Readonly] public string recordAudioName { get; private set; }
            [field:SerializeField, Readonly] public AudioSource source { get; private set; }
            
            public bool isPlaying => source.isPlaying;
            public bool isPaused => source.isPlaying && source.time > 0f;
            
            public Entry(AudioSource audioSource, string name)
            {
                this.source = audioSource;
                this.name = name;
            }
            
            // ====================================================================================================
            // ====================================================================================================
            
            public Entry Play(float delay = 0f)
            {
                if(delay <= 0f) source.Play();
                else source.PlayDelayed(delay);
                return this;
            }
            
            public Entry Stop()
            {
                source.Stop();
                return this;
            }
            
            public Entry Pause()
            {
                source.Pause();
                return this;
            }
            
            // ====================================================================================================
            // ====================================================================================================
            
            
            public Entry SetVolume(float volume)
            {
                source.volume = volume;
                return this;
            }
            
            public Entry SetLoop(bool loop)
            {
                source.loop = loop;
                return this;
            }
            
            public Entry SetMixer(AudioMixer mixer, string name, int i = 0)
            {
                source.outputAudioMixerGroup = mixer.FindMatchingGroups(name)[i];
                return this;
            }
            
            public Entry SetPlaybackTime(float time)
            {
                source.time = time;
                return this;
            }
            
            public Entry SetAudio(string audioName)
            {
                if(audioName != null && getAudioResource(audioName).PassValue(out var audioClip) != null)
                {
                    source.clip = audioClip;
                }
                else throw new Exception($"Audio clip not found: [{ audioName }]");
                
                return this;
            }
        }
        
        [Readonly] public List<Entry> entries = new List<Entry>();
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        void Awake()
        {
            cache[gameObject] = this;
        }
        
        void OnDestroy()
        {
            cache.Remove(gameObject);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public Entry GetAudioEntry(string name)
        {
            var entry = entries.Find(e => e.name == name);
            if(entry == null)
            {
                var src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                entry = new Entry(src, name);
                entries.Add(entry);
            }
            return entry;
        }
        
        public static AudioSourceController GetOrAdd(GameObject g)
        {
            if(cache.TryGetValue(g, out var controller))
                return controller;
            else
                return g.AddComponent<AudioSourceController>();
        }
    }
    
    public static class UnityMethodExtension
    {
        public static AudioSourceController.Entry Audio(this GameObject g, string audioName)
        {
            return AudioSourceController.GetOrAdd(g).GetAudioEntry("main").SetAudio(audioName);
        }
        
        public static AudioSourceController.Entry Audio(this Component c, string audioName)
        {
            return AudioSourceController.GetOrAdd(c.gameObject).GetAudioEntry("main").SetAudio(audioName);
        }
        
        public static AudioSourceController.Entry Audio(this GameObject g, string name, string audioName)
        {
            return AudioSourceController.GetOrAdd(g).GetAudioEntry(name).SetAudio(audioName);
        }
        
        public static AudioSourceController.Entry Audio(this Component c, string name, string audioName)
        {
            return AudioSourceController.GetOrAdd(c.gameObject).GetAudioEntry(name).SetAudio(audioName);
        }
        
        public static AudioSourceController.Entry AudioByName(this GameObject g, string name)
        {
            return AudioSourceController.GetOrAdd(g).GetAudioEntry(name);
        }
        
        public static AudioSourceController.Entry AudioByName(this Component c, string name)
        {
            return AudioSourceController.GetOrAdd(c.gameObject).GetAudioEntry(name);
        }
    }
    
    
}
