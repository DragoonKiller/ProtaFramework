using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prota.Unity;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Prota.Unity
{
    // 存储了多层tilemap信息的块.
    // 主要用于自动存储和合并tilemap.
    [CreateAssetMenu(fileName = "ChunkTilemap", menuName = "Game/Chunk Tilemap")]
    public class ChunkTilemap : ScriptableObject, ISerializationCallbackReceiver
    {
        public const int chunkSize = OverworldTilemap.chunkSize;
        
        public Vector2Int chunkCoord;
        
        // 左下角的坐标
        public Vector2Int chunkPosMin => chunkCoord * chunkSize;
        
        // 右上角的坐标
        public Vector2Int chunkPosMax => chunkPosMin + new Vector2Int(chunkSize, chunkSize);
        
        public string chunkObjectName => $"[{chunkCoord.x}:{chunkCoord.y}]";
        
        // tilemap层名字; tile位置; tile内容.
        public HashMapDict<string, Vector2Int, TileBase> tilemapInfo = null;
        
        // ====================================================================================================
        // Serialization
        // ====================================================================================================
        
        [Serializable]
        struct SerializedCell
        {
            public Vector2Int localPos;
            public TileBase tilesAtGrid;
        }
        
        [Serializable]
        struct SerializedLayer
        {
            public string layerName;
            public SerializedCell[] cells;
        }
        
        [SerializeField] SerializedLayer[] serializedChunk;
        
        public void OnBeforeSerialize()
        {
            if(tilemapInfo == null)
            {
                serializedChunk = Array.Empty<SerializedLayer>();
                return;
            }
            
            serializedChunk = new SerializedLayer[tilemapInfo.Count];
            var i = 0;
            foreach(var (layerName, cells) in tilemapInfo)
            {
                var serializedCells = new SerializedCell[cells.Count];
                var j = 0;
                foreach(var (localPos, tilesAtGrid) in cells)
                {
                    serializedCells[j] = new SerializedCell
                    {
                        localPos = localPos,
                        tilesAtGrid = tilesAtGrid
                    };
                    j++;
                }
                
                serializedChunk[i] = new SerializedLayer
                {
                    layerName = layerName,
                    cells = serializedCells
                };
                i++;
            }
                        
            tilemapInfo = null;
        }

        public void OnAfterDeserialize()
        {
            if(serializedChunk == null)
            {
                tilemapInfo = new HashMapDict<string, Vector2Int, TileBase>();
                return;
            }
            
            tilemapInfo = new HashMapDict<string, Vector2Int, TileBase>();
            foreach(var layer in serializedChunk)
            {
                foreach(var cell in layer.cells)
                {
                    tilemapInfo.AddElement(layer.layerName, cell.localPos, cell.tilesAtGrid);
                }
            }
            
            serializedChunk = null;
        }

        
        
        
    }
}
