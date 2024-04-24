using System;
using System.Collections.Generic;
using System.Linq;
using Prota.Unity;
using UnityEngine;


namespace Prota.Unity
{


    // 每 100 x 100 个地块分成一个 chunk.
    // 地块的范围从 (0, 0) 开始, 到 (99, 99).

    [CreateAssetMenu(fileName = "OverworldTilemap", menuName = "Game/Overworld Tilemap")]
    public class OverworldTilemap : ScriptableObject
    {
        public const int chunkSize = 100;
        
        
        [NonSerialized] public readonly CoordMap coordMap = new();
        
        [SerializeField] public List<ChunkTilemap> chunks = new();
        
        
        
        
        ChunkTilemap CreateChunkObjectAtCoord(Vector2Int coord)
        {
            var chunk = ScriptableObject.CreateInstance<ChunkTilemap>();
            chunk.chunkCoord = coord;
            chunk.name = chunk.chunkObjectName;
            return chunk;
        }
        
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public class CoordMap
        {
            HashSet<ChunkTilemap> submittedChunks;
            readonly Dictionary<Vector2Int, ChunkTilemap> coordToChunk;
            
            bool Validate(List<ChunkTilemap> chunks)
                => coordToChunk != null && chunks.All(x => submittedChunks.Contains(x));
            
            public void Update(List<ChunkTilemap> chunks)
            {
                submittedChunks = new HashSet<ChunkTilemap>(chunks);
                coordToChunk.Clear();
                foreach (var chunk in chunks) coordToChunk[chunk.chunkCoord] = chunk;
            }
            
            public bool Get(Vector2Int coord, out ChunkTilemap chunk)
                => coordToChunk.TryGetValue(coord, out chunk);
            
            public bool Has(Vector2Int coord)
                => coordToChunk.ContainsKey(coord);
            
            public ChunkTilemap Get(Vector2Int coord)
            {
                if (coordToChunk.TryGetValue(coord, out var chunk)) return chunk;
                throw new Exception($"Chunk at [{coord}] not found.");
            }
        }
        
        
        
    }
}
