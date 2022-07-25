using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prota.Editor
{
    public struct HandleContext : IDisposable
    {
        Color color;
        
        public static HandleContext Get()
        {
            return new HandleContext() {
                color = Gizmos.color
            };
        }
        
        public void Dispose()
        {
            Handles.color = color;
        }
    }



}
