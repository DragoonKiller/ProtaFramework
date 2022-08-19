using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public struct GizmosContext : IDisposable
    {
        Color color;
        
        public static GizmosContext Get()
        {
            return new GizmosContext() {
                color = Gizmos.color
            };
        }
        
        public void Dispose()
        {
            Gizmos.color = color;
        }
    }



}
