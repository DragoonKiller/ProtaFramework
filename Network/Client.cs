using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using System.Net;
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
            RegiserUniversalOperation();
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
                Action<NetId> callback = null;
                callback = id => {
                    Log.Info($"连接完成, 向主机 { id } 发送建立客户端连接的消息");
                    onConnect -= callback;
                    SendToClient(id, w => {
                        w.Put(BuiltinMsgId.C2CRequestClientConnection);
                    });
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
        
        void OnHostResponse(NetId id, NetDataReader reader, DeliveryMethod method)
        {
            var success = reader.GetBool();
            Log.Info($"收到连接的主机的响应: [{ id }] succcess:{ success }");
            // 什么都不做.
            // 主机会同步所有状态配表, 这时游戏世界就起来了.
        }
        
        void OnHostDisconnect(NetId id)
        {
            Log.Info($"与主机的连接中断了 [{ id }]");
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
        
        void OnClinetRequest(NetId id, NetDataReader reader, DeliveryMethod method)
        {
            Log.Info($"收到客户端连接主机的请求: { id }");
            subClients.Add(id);
            SendToClient(id, w => {
                w.Put(BuiltinMsgId.C2CResponseClientConnection);
                w.Put(true);       // 确认成功.
                // TODO 同步配表.
            });
        }
        
        void OnClientDisconnect(NetId id)
        {
            Log.Info($"与客户端的连接中断了 { id }");
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
        
        // ============================================================================================================
        // 通用用户操作
        // ============================================================================================================
        
        
        void RegiserUniversalOperation()
        {
            AddCallback(MsgId.C2CLog, (id, reader, method) => {
                var s = reader.GetString();
                var t = new DateTime(reader.GetLong());
                var now = DateTime.Now;
                var delay = now - t;
                Log.Info($" C2C Log { id } [{ delay.TotalMilliseconds.ToString(".00") }ms] { s }");
            });
        }
        
        public void LogTo(NetId id, string message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            SendToClient(id, w => {
                w.Put(MsgId.C2CLog);
                w.Put(message);
                w.Put(DateTime.Now.Ticks);
            }, deliveryMethod);
        }
        
        
        
    }
}
