using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.Threading;
using JetBrains.Annotations;

namespace Prota.Unity
{
    [Serializable]
    struct SaveEntry
    {
        public string key;
        public string value;
    }
    
    // 简简单单存档管理类.
    public class Save : Singleton<Save>
    {
        public static DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        
        public static FileInfo saveFile = dir.SubFile("save.json");
        
        public static FileInfo backupFile = dir.SubFile("save.json.back");
        
        // 数据更新了, 需要保存.
        [SerializeField, Readonly] bool pending = true;
        
        // 数据正在保存中.
        [SerializeField, Readonly] bool saving = false;
        
        readonly Dictionary<string, string> data = new Dictionary<string, string>();
        
        // ====================================================================================================
        // ====================================================================================================
        
        protected override void Awake()
        {
            base.Awake();
            
            dir.EnsureExists();
            if (!saveFile.Exists) saveFile.Create().Dispose();
            ReadData();
        }
        
        void ReadData()
        {
            using var _ = TempList.Get<SaveEntry>(out var entries);
            var json = File.ReadAllText(saveFile.FullName);
            JsonUtility.FromJsonOverwrite(json, entries);
            data.Clear();
            foreach (var entry in entries) data[entry.key] = entry.value;
        }
        
        void SaveInstantly()
        {
            Backup();
            
            using var _ = TempList.Get<SaveEntry>(out var entries);
            foreach (var kv in data)
            {
                entries.Add(new SaveEntry { key = kv.Key, value = kv.Value });
            }
            
            var json = JsonUtility.ToJson(entries);
            
            File.WriteAllText(saveFile.FullName, json);
        }
        
        void Backup()
        {
            if (backupFile.Exists) backupFile.Delete();
            saveFile.CopyTo(backupFile.FullName);
        }
        
        void LateUpdate()
        {
            if(pending)
            {
                if(!saving)
                {
                    pending = false;
                    Task.Run(() => {
                        saving = true;
                        SaveInstantly();
                        saving = false;
                    });
                }
                else
                {
                    // 正在保存, 但是数据有更新, 要等到 save 结束后再次保存.
                    // 保持 pending = true 不变即可.
                }
            }
        }
        
        public void ClearAllSave()
        {
            Backup();
            backupFile.Delete();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public void Write<T>(string key, T value)
        {
            key = ToStandardKey(key);
            data[key] = JsonUtility.ToJson(value);
            pending = true;
        }
        
        public bool Read<T>(string key, out T value) where T: struct
        {
            key = ToStandardKey(key);
            
            if (data.TryGetValue(key, out var json))
            {
                value = JsonUtility.FromJson<T>(json);
                return true;
            }
            
            value = default;
            return false;
        }
        
        public bool Read<T>(string key, T target) where T: class
        {
            if(target == null) return false;
            
            key = ToStandardKey(key);
            
            if (data.TryGetValue(key, out var json))
            {
                JsonUtility.FromJsonOverwrite(json, target);
                return true;
            }
            
            return false;
        }
        
        string ToStandardKey(string a)
        {
            return a.ToLower().Replace(" ", "_");
        }
        
        
        public static void Test()
        {
            
            
            
        }
        
    }
    

}
