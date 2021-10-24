using UnityEngine;
using UnityEditor;

namespace Prota.Data
{

    public class SubCoreBindings : DataBlock
    {
        public DataCore subCore => this.GetComponent<DataCore>();
    }
    
}