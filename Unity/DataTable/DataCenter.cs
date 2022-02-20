using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Prota.Unity;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Prota.Unity
{
    // 全局数据表管理器.
    public class DataCenter : MonoBehaviour
    {
        public static DataCenter instance;
        
        public DataSchemaPresetList schemaPresets;
        
        DataSchemaPresetList appliedPresets;
        
        readonly Dictionary<string, DataTableComponent> tableMap = new Dictionary<string, DataTableComponent>();
        
        readonly Dictionary<string, int> nameToIndex = new Dictionary<string, int>();
        
        readonly List<DataTableComponent> tableComponents = new List<DataTableComponent>();
        
        public DataTable this[string name] => tableMap[name].table;
        
        public DataTable this[int i] => tableComponents[i].table;
        
        void Awake()
        {
            instance = this;
            Reset();
        }
        
        public void ClearAll()
        {
            this.transform.ClearSub();
            tableMap.Clear();
            nameToIndex.Clear();
            tableComponents.Clear();
        }
        
        public void Reset()
        {
            if(appliedPresets == schemaPresets) return;
            
            ClearAll();
            
            appliedPresets = schemaPresets;
            
            foreach(var schemaPreset in schemaPresets.presets)
            {
                var g = new GameObject();
                g.transform.SetParent(this.transform, false);
                var component = g.GetOrCreate<DataTableComponent>();
                
                var table = component.table = new DataTable(schemaPreset.tableName, schemaPreset.schema);
                
                tableMap.Add(table.name, component);
                tableComponents.Add(component);
                nameToIndex.Add(table.name, tableComponents.Count - 1);
            }
        }
        
        void OnDestroy()
        {
            if(instance == this) instance = null;
        }
        
    }
    
}