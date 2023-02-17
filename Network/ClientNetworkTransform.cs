using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

namespace Prota.Network
{
    // https://docs-multiplayer.unity3d.com/netcode/current/components/networktransform#clientnetworktransform
    // NetworkTransform 默认从服务器同步数据到客户端, 不允许在客户端修改数据.
    // 把 OnIsServerAuthoritative 改成返回 false 使其数据从 Owner 同步到服务器(和其他客户端).
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
