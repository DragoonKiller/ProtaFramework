using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using LiteNetLib.Utils;

namespace Prota.Net
{
    public static partial class ProtoId
    {
        // 客户端进入和退出房间.
        public const int C2SReqEnterRoom = -3;
        public const int S2CRspEnterRoom = -4;
        public const int S2CNtfOtherEnterExitRoom = -5;
        public const int C2SReqExitRoom = -6;
        public const int S2CRspExitRoom = -7;
    }
    
    public struct C2SReqEnterRoom : INetSerializable
    {
        public int roomId { get; private set; }

        public C2SReqEnterRoom(int roomId) => this.roomId = roomId;

        public void Deserialize(NetDataReader reader) => roomId = reader.GetInt();

        public void Serialize(NetDataWriter writer) => writer.Put(roomId);
    }
    
    public struct C2SReqExitRoom : INetSerializable
    {
        public void Deserialize(NetDataReader reader) { }

        public void Serialize(NetDataWriter writer) { }
    }
    
    public struct S2CRspExitRoom : INetSerializable
    {
        public bool success { get; private set; }
        
        public S2CRspExitRoom(bool success)
        {
            this.success = success;
        }

        public void Deserialize(NetDataReader reader) => success = reader.GetBool();

        public void Serialize(NetDataWriter writer) => writer.Put(success);
    }
    
    public struct S2CNtfOtherEnterExitRoom : INetSerializable
    {
        public bool isEnter { get; private set; }
        
        public bool isLeave => !isEnter;
        
        public S2CNtfOtherEnterExitRoom(bool enter)
        {
            this.isEnter = enter;
        }

        public void Deserialize(NetDataReader reader)
        {
            isEnter = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(isEnter);
        }
    }
    
    public struct S2CRspEnterRoom : INetSerializable
    {
        public int id { get; private set; }
        
        public NetId[] players { get; private set; }
        
        public bool success { get; private set; }
        
        public S2CRspEnterRoom(int roomId, NetId[] players)
        {
            this.id = roomId;
            this.players = players;
            success = true;
        }
        
        public static S2CRspEnterRoom fail => new S2CRspEnterRoom() { success = false };
        
        public void Deserialize(NetDataReader reader)
        {
            players = new NetId[reader.GetInt()];
            for(int i = 0; i < players.Length; i++)
            {
                players[i] = reader.Get<NetId>();
            }
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(players.Length);
            for(int i = 0; i < players.Length; i++)
            {
                writer.Put(players[i]);
            }
        }
    }

    
}