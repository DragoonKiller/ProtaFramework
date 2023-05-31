using System;
using System.Collections.Generic;
using System.IO;
using Prota.Unity;
using UnityEngine;

namespace Prota.Editor
{
    public static class AssetTree
    {
        static FileTree _instance;
        public static FileTree instance
        {
            get
            {
                if(_instance == null)
                {
                    var context = SwitchToMainThread.context;
                    if(context == null) throw new Exception("initialized too early, try registering callbask to it.");
                    
                    _instance = new FileTree(Application.dataPath, context);
                }
                return _instance;
            }
        }
    }
    
}
