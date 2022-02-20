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
    
    // 引入了 client-host 机制的 ClientBase.
    public class Client : ClientBase
    {
        [NonSerialized]
        public bool isHost;
        
        protected override void Start()
        {
            base.Start();
        }
        
        public readonly List<NetId> subClients = new List<NetId>();
        
        public NetId hostId = NetId.None;
        
        // ============================================================================================================
        // Client
        // ============================================================================================================
        
        public void ConnectToHost(NetId id)
        {
            Clear();
            isHost = false;
            DisconnectAllClient();
            
            // 把自己标记为非主机.
            SendToServer(w => {
                w.Put(BuiltinMsgId.C2SRequestHost);
                w.Put(false);
            });
            
            var clientInfo = serverConnections.Find(x => x.id == id);
            AddCallback(BuiltinMsgId.C2CResponseClientConnection, OnHostResponse);
            if(clientInfo != null)
            {
                Log.Info($"尝试连接到主机: { id }[{ clientInfo.endpoint }]");
                ConnectClientByServer(id);
                Action<NetPeer> callback = null;
                callback = peer => {
                    if(TryGetIdByEndpoint(peer.EndPoint, out var connId) && id == connId)
                    {
                        Log.Info($"连接完成, 向主机 { connId }[{ peer.EndPoint }] 发送建立客户端连接的消息");
                        onConnect -= callback;
                        SendToPeer(peer, w => {
                            w.Put(BuiltinMsgId.C2CRequestClientConnection);
                        });
                    }
                };
                onConnect += callback;
            }
            else
            {
                Log.Error($"找不到 id = [{ id }] 的连接.");
            }
        }
        
        public void RequestDisconnectToHost()
        {
            if(isHost) return;
            if(!hostId.valid) return;
            DisconnectTo(hostId);
        }
        
        void OnHostResponse(NetPeer peer, NetDataReader reader, DeliveryMethod method)
        {
            var success = reader.GetBool();
            Log.Info($"收到连接的主机的响应: [{ peer.EndPoint }] succcess:{ success }");
            if(!TryGetIdByEndpoint(peer.EndPoint, out hostId))
            {
                Log.Warning($"与主机 [{ peer.EndPoint }] 建立了连接, 但是找不到对应的 id.");
            }
            
            // 什么都不做.
            // 主机会同步所有状态配表, 这时游戏世界就起来了.
        }
        
        void OnHostDisconnect(NetPeer peer)
        {
            Log.Info($"与主机的连接中断了 [{ peer.EndPoint }]");
            hostId = NetId.None;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        public void StartHost()
        {
            Clear();
            isHost = true;
            hostId = myId;
            SendToServer(w => {
                w.Put(BuiltinMsgId.C2SRequestHost);
                w.Put(true);
            });
            AddCallback(BuiltinMsgId.C2CRequestClientConnection, OnClinetRequest);
            onDisconnect += OnClientDisconnect;
        }
        
        public void StopHost()
        {
            DisconnectAllClient();
            SendToServer(w => {
                w.Put(BuiltinMsgId.C2SRequestHost);
                w.Put(false);
            });
        }
        
        void OnClinetRequest(NetPeer peer, NetDataReader reader, DeliveryMethod method)
        {
            TryGetIdByEndpoint(peer.EndPoint, out var id);
            Log.Info($"收到客户端连接主机的请求: { id }[{ peer.EndPoint }]");
            subClients.Add(id);
            SendToPeer(peer, w => {
                w.Put(BuiltinMsgId.C2CResponseClientConnection);
                w.Put(true);       // 确认成功.
                // TODO 同步配表.
            });
        }
        
        void OnClientDisconnect(NetPeer peer)
        {
            Log.Info($"与客户端的连接中断了 [{ peer.EndPoint }]");
            TryGetIdByEndpoint(peer.EndPoint, out var id);
            // TODO 清理该客户端所持有的数据.
        }
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        public void Clear()
        {
            StopHost();
            hostId = NetId.None;
            subClients.Clear();
            DisconnectAllClient();
            RemoveCallback(BuiltinMsgId.C2CRequestClientConnection, OnClinetRequest);
            RemoveCallback(BuiltinMsgId.C2CResponseClientConnection, OnHostResponse);
            onDisconnect -= OnClientDisconnect;
            onDisconnect -= OnHostDisconnect;
        }
        
    }
}