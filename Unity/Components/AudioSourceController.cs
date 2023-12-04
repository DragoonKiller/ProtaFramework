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
            [Readonly] public string name;
            [Readonly] public AudioSource source;
            
            public bool isPlaying => source.isPlaying;
            public bool isPaused => source.isPlaying && source.time > 0f;
            
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
            
            public Entry Play(float delay = 0f)
            {
                if(delay <= 0f) source.Play();
                else source.PlayDelayed(delay);
                return this;
            }
            
            public Entry SetMixer(AudioMixer mixer, string name, int i = 0)
            {
                source.outputAudioMixerGroup = mixer.FindMatchingGroups(name)[i];
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
            
            public Entry SetPlaybackTime(float time)
            {
                source.time = time;
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
        
        public Entry GetAudio(string name, string audioName)
        {
            var entry = entries.Find(e => e.name == name);
            if(entry == null)
            {
                entry = new Entry();
                entry.name = name;
                var src = entry.source = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                entry.source.clip = getAudioResource(audioName);
                if(entry.source.clip == null)
                    throw new Exception($"Audio clip not found: [{ audioName }]");
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
            return AudioSourceController.GetOrAdd(g).GetAudio(audioName, audioName);
        }
        
        public static AudioSourceController.Entry Audio(this Component c, string audioName)
        {
            return AudioSourceController.GetOrAdd(c.gameObject).GetAudio(audioName, audioName);
        }
        
        public static AudioSourceController.Entry Audio(this GameObject g, string name, string audioName)
        {
            return AudioSourceController.GetOrAdd(g).GetAudio(name, audioName);
        }
        
        public static AudioSourceController.Entry Audio(this Component c, string name, string audioName)
        {
            return AudioSourceController.GetOrAdd(c.gameObject).GetAudio(name, audioName);
        }
        
    }
    
    
}
