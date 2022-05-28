using System.Collections.Generic;
using UnityEngine;

namespace Prota.Data
{
    [CreateAssetMenu(menuName = "ProtaFramework/Data/Data Schema Preset", fileName = "SchemaPreset")]
    public class DataSchemaPreset : ScriptableObject
    {
        public string tableName;
        
        public List<DataEntry> dataEntries = new List<DataEntry>();
        
        public DataSchema schema
        {
            get
            {
                return new DataSchema(dataEntries);
            }
        }
        
    }
}