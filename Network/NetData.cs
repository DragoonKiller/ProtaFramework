using System;
using System.Collections.Generic;
using LiteNetLib.Utils;
using Prota.Unity;
using UnityEngine;
using XLua;


/*


===============================================================================
Ownership 机制:

数据表里的每一行都有一个 owner 字段.
这个字段指定*哪里的数据是权威数据*. owner 字段的值是服务器下发的 id.
* owner = 0: 场景公共数据. 权威数据在 host.
* owner = clientId: 权威数据在 clientId 对应的玩家那里. 这个 clientId 也可能对应到host.

本地修改 myId != owner 的数据, 会被直接屏蔽.
本地修改 myId == owner 的数据, 会在本地直接修改, 再将最新数据上报主机, 由主机同步到其它客户端. (不可靠, 保序)
本地修改 0 == owner 的数据, 不允许修改, 但是会在拦截之后向主机上报差值, 由主机模拟数据后发送给其它客户端. (可靠, 不保序)

添加和删除表中条目, 应全部由主机完成, 并通知所有客户端. (可靠, 保序)


===============================================================================
协议分类:

从主机发送给客户端:
(1) [创建/删除数据条目 & 接收owner = 表owner 或 自己] (可靠 保序) 主机通知其下所有客户端, 表条目的增删.
(2) [修改数据表 & 接收owner = 表owner 或 自己 或 0] (不可靠 不保序 重发) 主机将条目的数据下发给客户端.

从客户端发送给主机:
(1) [创建/删除数据条目 & owner = 自己] (可靠 保序) 向主机申请创建/删除条目. (比如挂在自己角色身上的buff).
(2) [修改数据表 & owner = 主机 或 0] (可靠 不保序) 向主机申请值的改变(字符串禁止). (比如给怪物扣血, 给自己回血; 角色的血量的 owner 是 0, 因为所有玩家都有能力操作这个值).
(3) [修改数据表 & owner = 自己] (不可靠 保序) 本地先修改, 上报给主机. (比如自己改变坐标进行移动).

不论是不是主机, 所有*接收*到的协议都要如实处理.

现在可以全部采用(可靠, 保序)做测试.

===============================================================================
连接机制:
clinet-host 连接建立完毕后, host 立即向 client 发送初始化数据表的特殊协议. (可靠, 保序).
client 收到数据表后, 清理原有数据表, 构建新的数据表.
client 主动退出 或 掉线 时, host 向其它所有还连着的客户端发送协议, 删除所有 owner = client 的条目.
host 主动切断连接 或 掉线 时, client 保留本地所有数据, 等待重连.


===============================================================================
表同步:
所有客户端必须持有同一份表结构. 根据表结构可以写出对应的网络通信代码.
由于网络有延迟, 数据表出现丢失条目的情况是正常的.



*/

namespace Prota.Net
{
    
    public sealed class NetDataSchema : DataSchema
    {
        public NetDataSchema(List<DataEntry> entires) : base(entires)
        {
            
        }
    }

    public sealed class NetDataTable : DataTable
    {
        public NetDataTable(string name, DataSchema schema) : base(name, schema)
        {
            
        }
    }
    
    [LuaCallCSharp]
    public sealed class NetData : DataTableComponent
    {
        public const string ownerKey = "owner";
        
        public Client client;
        
        void Awake()
        {
            client = GetComponentInParent<Client>(true);
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        public void ClientAdd(long owner, List<DataValue> values)
        {
            if(client.isHost)
            {
                // TODO
                return;
            }
            
            if(owner == 0)
            {
                C2CRequestNewLine(owner, table.name, values);
            }
            else if(owner == client.myId)
            {
                var newLineIndex = table.AddRecord(values);
                C2CRequestNewLine(owner, table.name, values);
            }
            else
            {
                // Do Nothing.
            }
        }

        public void Modify(int keyColumnId, DataValue key, int columnId, DataValue value)
        {
            var i = table.data[keyColumnId].IndexOfValue(key);
            if(client.isHost)
            {
                // TODO
                return;
            }
            
            if(i == -1)
            {
                Log.Error($"想要修改数据 列[{ keyColumnId }] key[{ key }] 但是找不到 key");
                return;
            }
            var owner = table.DataByName(ownerKey)[i].i64;
            if(owner == 0 || client.isHost)
            {
                C2CRequestModify(owner, table.name, value);
            }
            else if(owner == client.myId)
            {
                var newLineIndex = table.data[columnId][i] = value;
                C2CRequestModify(owner, table.name, value);
            }
            else
            {
                // Do Nothing.
            }
        }

        public void Remove(string keyColumnName, DataValue key) => Remove(table.ColumnNameToId(keyColumnName), key);
        public void Remove(int keyColumnId, DataValue key)
        {
            // Host 由通用数据表同步机制同步给其它玩家.
            var i = table.data[keyColumnId].IndexOfValue(key);
            if(client.isHost)
            {
                
                return;
            }
            
            if(i == -1)
            {
                Log.Error($"想要修改数据 列[{ keyColumnId }] key[{ key }] 但是找不到 key");
                return;
            }
            var owner = table.DataByName(ownerKey)[i].i64;
            if(owner == 0 || client.isHost)
            {
                C2CRequestRemoveKey(owner, table.name, key);
            }
            else if(owner == client.myId)
            {
                var newLineIndex = table.RemoveRecord(i);
                C2CRequestRemoveKey(owner, table.name, key);
            }
            else
            {
                // Do Nothing.
            }
        }
        
        private void C2CRequestNewLine(long owner, string name, List<DataValue> values)
        {
            
        }

        private void C2CRequestModify(long owner, string name, DataValue value)
        {
            
        }
        
        private void C2CRequestRemoveKey(long owner, string name, DataValue key)
        {
            
        }
        
    }
    
    
    

}