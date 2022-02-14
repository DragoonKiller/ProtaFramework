using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using XLua;

namespace Prota.Net
{
    [LuaCallCSharp]
    public class Client : MonoBehaviour
    {
        Action<LuaTable> callbacks;
        
        
        NetManager client;
        
        void Awake()
        {
            var listener = new EventBasedNetListener();
            client = new NetManager(listener);
            client.Start();
            client.Connect("localhost", 9050, "ProtaClient");
            
            listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                Debug.LogErrorFormat("We got: {0}", dataReader.GetString(100));
                dataReader.Recycle();
            };
        }
        
        void Update()
        {
            client.PollEvents();
        }
        
        void OnDestroy()
        {
            client.Stop();
        }
        
        public void AddCallback(Action<LuaTable> callback)
        {
            callbacks += callback;
        }
        
        public void RemoveCallback(Action<LuaTable> callback)
        {
            callback -= callback;
        }
        
        
        
        
    }
}