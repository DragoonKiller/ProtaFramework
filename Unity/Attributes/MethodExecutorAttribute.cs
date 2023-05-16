using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

namespace Prota.Unity
{
    
    /*
    用法示例:
    
    [SerializeField, MethodExecutor] Nothing.Struct a;
    
    [MethodExecutor]
    void Test1()
    {
        print("1");
    }
    
    [MethodExecutor]
    void Test2()
    {
        print("2");
    }
    
    */
    
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MethodExecutor : PropertyAttribute
    {
        
    }
 
}
