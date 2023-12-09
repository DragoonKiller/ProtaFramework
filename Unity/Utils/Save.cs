using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    [Serializable]
    struct SaveEntry
    {
        public string key;
        public string value;
    }
    
    [Serializable]
    class SaveJson
    {
        [SerializeField] public List<SaveEntry> list;
    }
    
    // 简简单单存档管理类.
    public class Save : Singleton<Save>
    {
        public static DirectoryInfo dir;
        public static FileInfo saveFile;
        public static FileInfo backupFile;
        
        public bool dataLoaded { get; private set; }
        
        // 数据更新了, 需要保存.
        bool pending = true;
        
        // 数据正在保存中.
        bool saving = false;
        
        readonly Dictionary<string, string> data = new Dictionary<string, string>();
        
        // ====================================================================================================
        // ====================================================================================================
        
        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            dir = new DirectoryInfo(Application.persistentDataPath);
            saveFile = dir.SubFile("save.json");
            backupFile = dir.SubFile("save.json.back");
        }
        
        protected override void Awake()
        {
            base.Awake();
            dir.EnsureExists();
            if (!saveFile.Exists) saveFile.Create().Dispose();
            LoadData();
        }
        
        public void LoadData()
        {
            var json = File.ReadAllText(saveFile.FullName);
            Debug.LogError(json);
            var jsonObject = JsonUtility.FromJson<SaveJson>(json);
            Debug.LogError(JsonUtility.ToJson(jsonObject));
            data.Clear();
            foreach (var entry in jsonObject.list) data[entry.key] = entry.value;
            dataLoaded = true;
        }
        
        void SaveInstantly()
        {
            Backup();
            
            using var _ = TempList.Get<SaveEntry>(out var entries);
            foreach (var kv in data)
            {
                entries.Add(new SaveEntry { key = kv.Key, value = kv.Value });
            }
            
            var jsonObject = new SaveJson { list = entries };
            
            var json = JsonUtility.ToJson(jsonObject, true);
            
            File.WriteAllText(saveFile.FullName, json);
        }
        
        void Backup()
        {
            if (backupFile.Exists) backupFile.Delete();
            saveFile.CopyTo(backupFile.FullName);
        }
        
        public void Update()
        {
            if(pending)
            {
                if(!saving)
                {
                    pending = false;
                    saving = true;
                    Task.Run(() => {
                        try
                        {
                            SaveInstantly();
                            saving = false;
                        }
                        catch(Exception e)
                        {
                            Debug.LogException(e);
                        }
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
        
        public bool Erase(string key)
        {
            key = ToStandardKey(key);
            var res = data.Remove(key);
            pending = true;
            return res;
        }
        
        public void Write(string key, string value)
        {
            key = ToStandardKey(key);
            data[key] = value;
            pending = true;
        }
        
        public bool Read(string key, out string value)
        {
            key = ToStandardKey(key);
            if (data.TryGetValue(key, out var str))
            {
                value = str;
                return true;
            }
            value = default;
            return false;
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static bool Get(string key, out int value, int defaultValue = default)
        {
            var res = instance.Read(key, out string str);
            value = res ? int.Parse(str) : defaultValue;
            return res;
        }
        
        public static bool Get(string key, out float value, float defaultValue = default)
        {
            var res = instance.Read(key, out string str);
            value = res ? float.Parse(str) : defaultValue;
            return res;
        }
        
        public static bool Get(string key, out long value, long defaultValue = default)
        {
            var res = instance.Read(key, out string str);
            value = res ? int.Parse(str) : defaultValue;
            return res;
        }
        
        public static bool Get(string key, out double value, double defaultValue = default)
        {
            var res = instance.Read(key, out string str);
            value = res ? double.Parse(str) : defaultValue;
            return res;
        }
        
        public static bool Get(string key, out bool value, bool defaultValue = default)
        {
            var res = instance.Read(key, out string str);
            value = res ? bool.Parse(str) : defaultValue;
            return res;
        }
        
        public static bool Get(string key, out string value, string defaultValue = default)
        {
            var res = instance.Read(key, out value);
            value = res ? value : defaultValue;
            return res;
        }
        
        public static bool GetObject<T>(string key, T value) where T: class
        {
            var res = instance.Read(key, out var str);
            if (res) JsonUtility.FromJsonOverwrite(str, value);
            return res;
        }
        
        
        
        public static bool Set<T>(string key, T value)
        {
            instance.Write(key, value.ToString());
            return true;
        }
        
        public static bool SetObject<T>(string key, T value) where T: class
        {
            var json = JsonUtility.ToJson(value);
            instance.Write(key, json);
            return true;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        string ToStandardKey(string a)
        {
            return a.ToLower().Replace(" ", "_");
        }
        
    }
    

}
