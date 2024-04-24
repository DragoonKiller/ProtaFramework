using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Prota;
using System.Collections.Concurrent;

namespace Prota.Unity
{
    // 存储关于一个tilemap的额外信息.
    public class ExTilemap
    {
        // 有内容的格子.
        public readonly List<Vector2Int> validCells = new();
        
        // 每个格子的tile.
        public readonly ConcurrentDictionary<Vector2Int, TileBase> tilemap = new();
        
        // 格子的各种距离.
        public readonly ConcurrentDictionary<Vector2Int, int> distanceToTop = new();
        public readonly ConcurrentDictionary<Vector2Int, int> distanceToBottom = new();
        public readonly ConcurrentDictionary<Vector2Int, int> distanceToLeft = new();
        public readonly ConcurrentDictionary<Vector2Int, int> distanceToRight = new();
        
        
        public void UpdateAll()
        {
            
        }
    }
}
