using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using XLua;

namespace Prota.Net
{
    [LuaCallCSharp]
    public class Host : ClientBase
    {
        protected override void Start()
        {
            base.Start();
            
            listener.PeerConnectedEvent += peer => {
                
            };
        }
        
    }
}