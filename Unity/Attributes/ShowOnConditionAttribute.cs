using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Prota.Unity
{
    /// <summary>
    /// 规则:
    /// 字符串表示bool值或object值或返回bool的函数.
    /// "&&"表示与, "||"表示或, "!"表示非. 值为真时显示. 
    /// </summary>
    public class ShowWhenAttribute : PropertyAttribute
    {
        public string[] names;
        
        public ShowWhenAttribute(params string[] names)
        {
            this.names = names;
        }
    }
 
}
