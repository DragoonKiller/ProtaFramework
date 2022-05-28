using System.Collections.Generic;
using UnityEngine;

namespace Prota.Data
{
    [CreateAssetMenu(menuName = "ProtaFramework/Data/Data Schema Preset List", fileName = "SchemaPresetList")]
    public class DataSchemaPresetList : ScriptableObject
    {
        public List<DataSchemaPreset> presets = new List<DataSchemaPreset>();
    }
}