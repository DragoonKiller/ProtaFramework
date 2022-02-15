using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using XLua;
using System.Net;

using Prota.Lua;
using System.Collections.Generic;

namespace Prota.Net
{
    [LuaCallCSharp]
    public class Client : ClientBase
    {
        readonly static IPEndPoint gateEndpoint = new IPEndPoint(IPAddress.Parse("106.54.195.7"), 33200);
        
        NetPeer peerToHost;
        
        public List<Action<LuaTable>> callbacks = new List<Action<LuaTable>>();
        
        public string host;
        
        
        protected override void Start()
        {
            base.Start();
            
            listener.PeerConnectedEvent += peer => {
                if(peer.EndPoint.Equals(gateEndpoint)) return;
                Debug.Assert(peerToHost == null || peer.EndPoint.Equals(peerToHost.EndPoint));
                Log.Info($"{ this } 连接到主机 { peer.EndPoint }");
                peerToHost = peer;
                host = peer.EndPoint.ToString();
            };
            
            listener.PeerDisconnectedEvent += (peer, info) => {
                if(peer.EndPoint.Equals(gateEndpoint)) return;
                Log.Info($"{ this } 断开与主机的连接: { peer.EndPoint }");
                host = "";
            };
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        public void ConnectToHost(IPEndPoint endpoint)
        {
            DisconnectToHost();
            peerToHost = client.Connect(endpoint, "ProtaClient");
        }
        
        public void ConnectToHost(int a1, int a2, int a3, int a4, int port)
        {
            ConnectToHost(new IPEndPoint(IPAddress.Parse($"{a1}.{a2}.{a3}.{a4}"), port));
        }
        
        public void DisconnectToHost()
        {
            if(peerToHost != null) client.DisconnectPeer(peerToHost);
            peerToHost = null;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        public void AddCallback(Action<LuaTable> callback)
        {
            callbacks.Add(callback);
        }
        
        public void RemoveCallback(Action<LuaTable> callback)
        {
            callbacks.Remove(callback);
        }
        
    }
}