using UnityEngine;
using UnityEditor;
using Prota.Editor;

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
        
        public EditorPrefEntryInt(string key)
        {
            this.key = key;
            cachedValue = EditorPrefs.GetInt(key);
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
        
        public EditorPrefEntryFloat(string key)
        {
            this.key = key;
            cachedValue = EditorPrefs.GetFloat(key);
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
        
        public EditorPrefEntryString(string key)
        {
            this.key = key;
            cachedValue = EditorPrefs.GetString(key);
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
        
        public EditorPrefEntryBool(string key)
        {
            this.key = key;
            cachedValue = EditorPrefs.GetBool(key);
        }
    }
    
}
