using UnityEngine;
using System;

namespace Prota.Unity
{
    // 运行时存储数据表.
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class DataTableComponent : MonoBehaviour
    {
        [NonSerialized]
        public DataTable table;        
        public const string noDataName = "DataTable:NoData";
        
        public void Update()
        {
            if(table == null)
            {
                this.name = noDataName;
                return;
            }
            {
                this.name = table.name;
            }
        }
    }
    
}