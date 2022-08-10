using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using LiteNetLib.Utils;

namespace Prota.Net
{
    
    [ProtaProtocol(-1)]
    public struct C2SReqEnterRoom
    {
        public readonly int roomId;
        public C2SReqEnterRoom(int roomId) => this.roomId = roomId;
    }
    
    [ProtaProtocol(-2)]
    public struct C2SReqExitRoom
    {
    }
    
    [ProtaProtocol(-3)]
    public struct S2CRspExitRoom
    {
        public readonly bool success;
        public S2CRspExitRoom(bool success) => this.success = success;
    }
    
    [ProtaProtocol(-4)]
    public struct S2CNtfOtherEnterExitRoom
    {
        public readonly bool isEnter;
        
        [IgnoreSerialize]
        public bool isLeave => !isEnter;
        
        public S2CNtfOtherEnterExitRoom(bool isEnter) => this.isEnter = isEnter;
    }
    
    [ProtaProtocol(-5)]
    public struct S2CRspEnterRoom
    {
        public readonly int id;
        public readonly NetId[] players;
        public readonly bool success;
        
        public S2CRspEnterRoom(int roomId, NetId[] players)
        {
            this.id = roomId;
            this.players = players;
            success = true;
        }
        
        [MessagePack.SerializationConstructor]
        public S2CRspEnterRoom(int id, NetId[] players, bool success)
        {
            this.id = id;
            this.players = players;
            this.success = success;
        }

        public static S2CRspEnterRoom fail => new S2CRspEnterRoom();
    }

    
}