using UnityEngine;
using UnityEditor;
using Prota.Editor;
using System;

namespace Prota.Editor
{
    public class EditorPrefEntryInt
    {
        public string key;
        
        public int cachedValue;
        
        public int value
        {
            get => cachedValue;
            set
            {
                cachedValue = value;
                EditorPrefs.SetInt(key, value);
            }
        }
        
        public EditorPrefEntryInt(string key, int defaultValue = default)
        {
            this.key = key;
            cachedValue = EditorPrefs.GetInt(key, default);
        }
    }
    
    public class EditorPrefEntryFloat
    {
        public string key;
        
        public float cachedValue;
        
        public float value
        {
            get => cachedValue;
            set
            {
                cachedValue = value;
                EditorPrefs.SetFloat(key, value);
            }
        }
        
        public EditorPrefEntryFloat(string key, float defaultValue = default)
        {
            this.key = key;
            cachedValue = EditorPrefs.GetFloat(key, defaultValue);
        }
    }
    
    public class EditorPrefEntryString
    {
        public string key;
        
        public string cachedValue;
        
        public string value
        {
            get => cachedValue;
            set
            {
                cachedValue = value;
                EditorPrefs.SetString(key, value);
            }
        }
        
        public EditorPrefEntryString(string key, string defaultValue = default)
        {
            this.key = key;
            cachedValue = EditorPrefs.GetString(key, defaultValue);
        }
    }
    
    public class EditorPrefEntryString<T>
    {
        public Func<string, T> getter;
        public Func<T, string> setter;
        
        public string key;
        
        public string cachedRawValue;
        
        public string rawValue
        {
            get => cachedRawValue;
            set
            {
                cachedRawValue = value;
                EditorPrefs.SetString(key, value);
            }
        }
        
        public T value
        {
            get => getter(cachedRawValue);
            set => rawValue = setter(value);
        }
        
        public EditorPrefEntryString(string key, Func<string, T> getter, Func<T, string> setter, string defaultValue = default)
        {
            this.getter = getter;
            this.setter = setter;
            this.key = key;
            cachedRawValue = EditorPrefs.GetString(key, defaultValue);
        }
    }
    
    public class EditorPrefEntryVec
    {
        public readonly string key;
        
        public readonly string keyX;
        public readonly string keyY;
        public readonly string keyZ;
        public readonly string keyW;
        
        public Vector4 cachedValue;
        
        public Vector4 vec4
        {
            get => cachedValue;
            set
            {
                cachedValue = value;
                EditorPrefs.SetFloat(keyX, value.x);
                EditorPrefs.SetFloat(keyY, value.y);
                EditorPrefs.SetFloat(keyZ, value.z);
                EditorPrefs.SetFloat(keyW, value.w);
            }
        }
        
        public Vector3 vec3
        {
            get => new Vector3(cachedValue.x, cachedValue.y, cachedValue.z);
            set
            {
                cachedValue = new Vector4(value.x, value.y, value.z, cachedValue.w);
                EditorPrefs.SetFloat(keyX, value.x);
                EditorPrefs.SetFloat(keyY, value.y);
                EditorPrefs.SetFloat(keyZ, value.z);
            }
        }
        
        public Vector2 vec2
        {
            get => new Vector2(cachedValue.x, cachedValue.y);
            set
            {
                cachedValue = new Vector4(value.x, value.y, cachedValue.z, cachedValue.w);
                EditorPrefs.SetFloat(keyX, value.x);
                EditorPrefs.SetFloat(keyY, value.y);
            }
        }
        
        public EditorPrefEntryVec(string key, Vector4 defaultValue = default)
        {
            this.key = key;
            this.keyX = key + "_x";
            this.keyY = key + "_y";
            this.keyZ = key + "_z";
            this.keyW = key + "_w";
            cachedValue = new Vector4(
                EditorPrefs.GetFloat(keyX, defaultValue.x),
                EditorPrefs.GetFloat(keyY, defaultValue.y),
                EditorPrefs.GetFloat(keyZ, defaultValue.z),
                EditorPrefs.GetFloat(keyW, defaultValue.w)
            );
        }
    }
    
    public class EditorPrefEntryBool
    {
        public string key;
        
        public bool cachedValue;
        
        public bool value
        {
            get => cachedValue;
            set
            {
                cachedValue = value;
                EditorPrefs.SetBool(key, value);
            }
        }
        
        public EditorPrefEntryBool(string key, bool defaultValue = default)
        {
            this.key = key;
            cachedValue = EditorPrefs.GetBool(key, defaultValue);
        }
    }
    
}
